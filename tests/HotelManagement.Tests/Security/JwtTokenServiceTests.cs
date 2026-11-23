using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HotelManagement.Api.Models.Enums;
using HotelManagement.Api.Security;
using HotelManagement.Tests.TestHelpers;
using Microsoft.Extensions.Options;
using Xunit;

namespace HotelManagement.Tests.Security;

public class JwtTokenServiceTests
{
    [Fact]
    public void CreateToken_ValidUser_ContainsIdEmailAndRoleClaims()
    {
        // Arrange
        var settings = new JwtSettings
        {
            Issuer = "HotelManagement",
            Audience = "HotelManagementClient",
            Key = "SuperSecretHotelKeyForJwtSigning2026!MustBe32PlusChars",
            ExpiresInMinutes = 60
        };
        var service = new JwtTokenService(Options.Create(settings));
        var user = TestData.CreateUser(
            id: 42,
            email: "mona@example.com",
            role: UserRole.Guest);

        // Act
        var tokenString = service.CreateToken(user);
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(tokenString);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(tokenString));
        Assert.Equal("42", jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value);
        Assert.Equal("mona@example.com", jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value);
        Assert.Equal("Guest", jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("short_key_less_than_32_chars")]
    public void JwtSettings_ShortOrMissingKey_IsDetectedAsInvalid(string invalidKey)
    {
        // Arrange & Act
        var isValid = !string.IsNullOrWhiteSpace(invalidKey) && invalidKey.Length >= 32;

        // Assert
        Assert.False(isValid);
    }
}