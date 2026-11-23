namespace HotelManagement.Api.Models;

public class RoomType
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string? Description { get; set; }
	public decimal PricePerNight { get; set; }
	public int MaxGuests { get; set; }
	public string? ImageUrl { get; set; }
	public bool IsActive { get; set; } = true;

	public ICollection<Room> Rooms { get; set; } = new List<Room>();
}