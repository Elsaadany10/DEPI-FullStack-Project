using HotelManagement.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelManagement.Api.Data.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("Reservations", t =>
        {
            t.HasCheckConstraint("CK_Reservations_Dates", "[CheckOutDate] > [CheckInDate]");
            t.HasCheckConstraint("CK_Reservations_NumberOfGuests", "[NumberOfGuests] > 0");
            t.HasCheckConstraint("CK_Reservations_Status", "[Status] IN ('Confirmed', 'CheckedIn', 'CheckedOut', 'Cancelled')");
        });

        builder.HasKey(res => res.Id);

        builder.Property(res => res.ReservationCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(res => res.CheckInDate)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(res => res.CheckOutDate)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(res => res.NumberOfGuests)
            .IsRequired();

        builder.Property(res => res.NightlyRate)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(res => res.TotalPrice)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(res => res.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(res => res.Notes)
            .HasMaxLength(300);

        builder.Property(res => res.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.HasOne(res => res.GuestUser)
            .WithMany(u => u.Reservations)
            .HasForeignKey(res => res.GuestUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(res => res.Room)
            .WithMany(r => r.Reservations)
            .HasForeignKey(res => res.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(res => res.ReservationCode)
            .IsUnique()
            .HasDatabaseName("UX_Reservations_ReservationCode");

        builder.HasIndex(res => new { res.RoomId, res.CheckInDate, res.CheckOutDate })
            .HasDatabaseName("IX_Reservations_RoomId_Dates");

        builder.HasIndex(res => res.GuestUserId)
            .HasDatabaseName("IX_Reservations_GuestUserId");

        builder.HasIndex(res => new { res.Status, res.CheckInDate })
            .HasDatabaseName("IX_Reservations_Status_CheckInDate");
    }
}