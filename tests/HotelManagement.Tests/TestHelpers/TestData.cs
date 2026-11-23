using HotelManagement.Api.Models;
using HotelManagement.Api.Models.Enums;

namespace HotelManagement.Tests.TestHelpers;

public static class TestData
{
    public static User CreateUser(
        int id = 1,
        string fullName = "Test User",
        string email = "test@example.com",
        string passwordHash = "hashed-password",
        UserRole role = UserRole.Guest,
        bool isActive = true,
        DateTime? createdAt = null)
    {
        return new User
        {
            Id = id,
            FullName = fullName,
            Email = email,
            PasswordHash = passwordHash,
            Role = role,
            IsActive = isActive,
            CreatedAt = createdAt ?? new DateTime(2026, 11, 1, 10, 0, 0)
        };
    }

    public static RoomType CreateRoomType(
        int id = 1,
        string name = "Standard Room",
        decimal pricePerNight = 1000m,
        int maxGuests = 2,
        bool isActive = true)
    {
        return new RoomType
        {
            Id = id,
            Name = name,
            PricePerNight = pricePerNight,
            MaxGuests = maxGuests,
            IsActive = isActive
        };
    }

    public static Room CreateRoom(
        int id = 1,
        string roomNumber = "101",
        int floor = 1,
        int roomTypeId = 1,
        RoomStatus status = RoomStatus.Available)
    {
        return new Room
        {
            Id = id,
            RoomNumber = roomNumber,
            Floor = floor,
            RoomTypeId = roomTypeId,
            Status = status
        };
    }
}