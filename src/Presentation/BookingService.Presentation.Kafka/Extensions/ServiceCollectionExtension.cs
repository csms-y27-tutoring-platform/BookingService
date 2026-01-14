using BookingService.Presentation.Kafka.Options;
using BookingService.Presentation.Kafka.Serializer;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;

namespace BookingService.Presentation.Kafka.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddKafkaConnections(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddOptions<ConnectionOptions>().BindConfiguration("Presentation:Kafka:Connection");
        return serviceCollection;
    }

    public static IServiceCollection AddKafkaProducerOptions(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddOptions<PublisherOptions>().BindConfiguration("Presentation:Kafka:Producers");
        serviceCollection.AddSingleton<ISerializer<BookingEventKey>, KafkaSerializer<BookingEventKey>>();
        serviceCollection.AddSingleton<ISerializer<BookingEventValue>, KafkaSerializer<BookingEventValue>>();
        return serviceCollection;
    }
}