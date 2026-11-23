using HotelManagement.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelManagement.Api.Data.Configurations;

public class RoomTypeConfiguration : IEntityTypeConfiguration<RoomType>
{
    public void Configure(EntityTypeBuilder<RoomType> builder)
    {
        builder.ToTable("RoomTypes", t =>
        {
            t.HasCheckConstraint("CK_RoomTypes_PricePerNight", "[PricePerNight] > 0");
            t.HasCheckConstraint("CK_RoomTypes_MaxGuests", "[MaxGuests] > 0");
        });

        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.Name)
            .IsRequired()
            .HasMaxLength(60);

        builder.Property(rt => rt.Description)
            .HasMaxLength(500);

        builder.Property(rt => rt.PricePerNight)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(rt => rt.MaxGuests)
            .IsRequired();

        builder.Property(rt => rt.ImageUrl)
            .HasMaxLength(300);

        builder.Property(rt => rt.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasIndex(rt => rt.Name)
            .IsUnique()
            .HasDatabaseName("UX_RoomTypes_Name");
    }
}