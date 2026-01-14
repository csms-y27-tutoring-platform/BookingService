using BookingService.Application.Abstractions.Messaging;
using BookingService.Infrastructure.Kafka.Producers;
using Microsoft.Extensions.DependencyInjection;

namespace BookingService.Infrastructure.Kafka.Extension;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddKafkaProducer(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IBookingEventPublisher, BookingEventPublisher>();
        return serviceCollection;
    }
}