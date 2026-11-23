using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement.Api.Extensions;

public static class AccountServiceRegistration
{
    public static IServiceCollection AddAccountServices(this IServiceCollection services)
    {
        // P2 registers Auth, Users, RoomTypes, Rooms, Dashboard, Payments services here
        return services;
    }
}