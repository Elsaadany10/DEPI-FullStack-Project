using HotelManagement.Api.Models.Enums;

namespace HotelManagement.Api.Models;

public class Reservation
{
    public int Id { get; set; }
    public string ReservationCode { get; set; } = string.Empty;
    public int GuestUserId { get; set; }
    public int RoomId { get; set; }
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public int NumberOfGuests { get; set; }
    public decimal NightlyRate { get; set; }
    public decimal TotalPrice { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Confirmed;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }

    public User GuestUser { get; set; } = null!;
    public Room Room { get; set; } = null!;
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}