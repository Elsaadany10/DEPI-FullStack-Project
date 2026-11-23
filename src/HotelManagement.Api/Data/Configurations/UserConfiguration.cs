using HotelManagement.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelManagement.Api.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", t =>
        {
            t.HasCheckConstraint("CK_Users_Role", "[Role] IN ('Guest', 'Receptionist', 'Admin')");
        });

        builder.HasKey(u => u.Id);

        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.Phone)
            .HasMaxLength(20);

        builder.Property(u => u.Role)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("UX_Users_Email");
    }
}