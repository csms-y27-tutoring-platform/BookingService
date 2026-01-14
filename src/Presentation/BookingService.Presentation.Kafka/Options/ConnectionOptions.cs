using Confluent.Kafka;

namespace BookingService.Presentation.Kafka.Options;

public class ConnectionOptions
{
    public required string Host { get; init; }

    public required SecurityProtocol SecurityProtocol { get; init; }
}