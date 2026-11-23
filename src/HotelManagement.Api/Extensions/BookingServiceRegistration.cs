using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement.Api.Extensions;

public static class BookingServiceRegistration
{
    public static IServiceCollection AddBookingServices(this IServiceCollection services)
    {
        // P3 registers Availability and Reservations services here
        return services;
    }
}