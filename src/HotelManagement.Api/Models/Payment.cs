using HotelManagement.Api.Models.Enums;

namespace HotelManagement.Api.Models;

public class Payment
{
    public int Id { get; set; }
    public int ReservationId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public DateTime PaidAt { get; set; }
    public int RecordedByUserId { get; set; }
    public string? Notes { get; set; }

    public Reservation Reservation { get; set; } = null!;
    public User RecordedByUser { get; set; } = null!;
}