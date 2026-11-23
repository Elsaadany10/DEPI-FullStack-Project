namespace HotelManagement.Api.Common;

public class SystemClock : IClock
{
    public DateOnly Today => DateOnly.FromDateTime(DateTime.Now);
    public DateTime Now => DateTime.Now;
}