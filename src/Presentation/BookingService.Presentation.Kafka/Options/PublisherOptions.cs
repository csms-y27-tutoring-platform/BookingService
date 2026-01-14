namespace BookingService.Presentation.Kafka.Options;

public class PublisherOptions
{
    public required string BookingCreatedTopic { get; init; }

    public required string BookingCancelledTopic { get; init; }

    public required string BookingCompletedTopic { get; init; }
}