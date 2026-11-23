using HotelManagement.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Tests.TestHelpers;

public static class TestDb
{
    public static AppDbContext CreateInMemoryContext(string? databaseName = null)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}