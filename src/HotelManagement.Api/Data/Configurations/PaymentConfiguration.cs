using HotelManagement.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelManagement.Api.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments", t =>
        {
            t.HasCheckConstraint("CK_Payments_Amount", "[Amount] > 0");
            t.HasCheckConstraint("CK_Payments_Method", "[Method] IN ('Cash', 'Card')");
        });

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Amount)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(p => p.Method)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(p => p.PaidAt)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(p => p.Notes)
            .HasMaxLength(300);

        builder.HasOne(p => p.Reservation)
            .WithMany(res => res.Payments)
            .HasForeignKey(p => p.ReservationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.RecordedByUser)
            .WithMany(u => u.RecordedPayments)
            .HasForeignKey(p => p.RecordedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.ReservationId)
            .HasDatabaseName("IX_Payments_ReservationId");

        builder.HasIndex(p => p.RecordedByUserId)
            .HasDatabaseName("IX_Payments_RecordedByUserId");
    }
}