using BookingService.Application.Contracts.Services;
using BookingService.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BookingService.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IBookingService, BookingsService>();
        return serviceCollection;
    }
}