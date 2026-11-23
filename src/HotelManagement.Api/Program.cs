using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using HotelManagement.Api.Common;
using HotelManagement.Api.Common.Middleware;
using HotelManagement.Api.Data;
using HotelManagement.Api.Data.Seed;
using HotelManagement.Api.Dtos.Common;
using HotelManagement.Api.Extensions;
using HotelManagement.Api.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Bind & validate JwtSettings (fail-fast if Key is missing or shorter than 32 chars)
var jwtSection = builder.Configuration.GetSection(JwtSettings.SectionName);
builder.Services.Configure<JwtSettings>(jwtSection);

var jwtSettings = jwtSection.Get<JwtSettings>() ?? new JwtSettings();
if (string.IsNullOrWhiteSpace(jwtSettings.Key) || jwtSettings.Key.Length < 32)
{
    throw new InvalidOperationException(
        "Jwt:Key is missing or shorter than 32 characters. Configure it using 'dotnet user-secrets set \"Jwt:Key\" \"<32+ chars>\"'.");
}

// 2. Database & Clock & Layer Registrations
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "ConnectionStrings:DefaultConnection is missing. Configure it using 'dotnet user-secrets set \"ConnectionStrings:DefaultConnection\" \"...\"'.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();

// Owner-specific service registrations (P2 & P3)
builder.Services.AddAccountServices();
builder.Services.AddBookingServices();

// 3. Controllers + camelCase + string enums + unified 400 validation error shape
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => JsonNamingPolicy.CamelCase.ConvertName(kvp.Key),
                    kvp => kvp.Value!.Errors.Select(err =>
                        string.IsNullOrWhiteSpace(err.ErrorMessage) ? "Invalid value." : err.ErrorMessage).ToArray()
                );

            var errorResponse = new ErrorResponse
            {
                Status = StatusCodes.Status400BadRequest,
                Message = "Validation failed.",
                Errors = errors.Count > 0 ? errors : null
            };

            return new BadRequestObjectResult(errorResponse);
        };
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// 4. CORS from configuration
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 5. JWT Authentication + Role Policies + Unified 401/403 Error Shape
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            RoleClaimType = System.Security.Claims.ClaimTypes.Role
        };

        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var body = new ErrorResponse
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "Authentication is required.",
                    Errors = null
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(body, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }));
            },
            OnForbidden = async context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";

                var body = new ErrorResponse
                {
                    Status = StatusCodes.Status403Forbidden,
                    Message = "You do not have permission to perform this action.",
                    Errors = null
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(body, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }));
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policies.GuestUp, policy =>
        policy.RequireRole(Roles.Guest, Roles.Receptionist, Roles.Admin));

    options.AddPolicy(Policies.StaffOnly, policy =>
        policy.RequireRole(Roles.Receptionist, Roles.Admin));

    options.AddPolicy(Policies.AdminOnly, policy =>
        policy.RequireRole(Roles.Admin));
});

// 6. Swagger with Bearer Authorize button
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Hotel Management API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Middleware pipeline order matters:
// ExceptionMiddleware -> CORS -> Authentication -> Authorization
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("FrontendPolicy");
app.UseAuthentication();
app.UseAuthorization();

if (app.Configuration.GetValue<bool>("Seed:Enabled"))
{
    await DbInitializer.SeedAsync(app.Services);
}

app.MapControllers();

app.Run();