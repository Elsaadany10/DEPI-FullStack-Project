namespace HotelManagement.Api.Common;

public interface IClock
{
    DateOnly Today { get; }
    DateTime Now { get; }
}