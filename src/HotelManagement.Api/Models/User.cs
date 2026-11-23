using HotelManagement.Api.Models.Enums;

namespace HotelManagement.Api.Models;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public ICollection<Payment> RecordedPayments { get; set; } = new List<Payment>();
}