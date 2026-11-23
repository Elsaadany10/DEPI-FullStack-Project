using HotelManagement.Api.Common;

namespace HotelManagement.Tests.TestHelpers;

public class FakeClock : IClock
{
    public DateOnly Today { get; set; }
    public DateTime Now { get; set; }

    public FakeClock(DateOnly? today = null, DateTime? now = null)
    {
        Today = today ?? new DateOnly(2026, 11, 23);
        Now = now ?? new DateTime(2026, 11, 23, 12, 0, 0);
    }
}