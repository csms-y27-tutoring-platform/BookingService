using BookingService.Application.Abstractions.Messaging;
using BookingService.Presentation.Kafka;
using BookingService.Presentation.Kafka.Options;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace BookingService.Infrastructure.Kafka.Producers;

public class BookingEventPublisher : IBookingEventPublisher
{
    private readonly IProducer<BookingEventKey, BookingEventValue> _producer;
    private readonly ConnectionOptions _connectionOptions;
    private readonly PublisherOptions _publisherOptions;

    public BookingEventPublisher(IOptions<PublisherOptions> publisherOptions, IOptions<ConnectionOptions> connectionOptions, ISerializer<BookingEventKey> key, ISerializer<BookingEventValue> value)
    {
        _publisherOptions = publisherOptions.Value;
        _connectionOptions = connectionOptions.Value;

        var config = new ProducerConfig
        {
            BootstrapServers = _connectionOptions.Host,
            SecurityProtocol = _connectionOptions.SecurityProtocol,
            Acks = Acks.All,
        };
        _producer = new ProducerBuilder<BookingEventKey, BookingEventValue>(config).SetKeySerializer(key).SetValueSerializer(value).Build();
    }

    public async Task PublishBookingCreated(long bookingId, string createdBy, CancellationToken cancellationToken)
    {
        var key = new BookingEventKey { BookingId = bookingId };
        var value = new BookingEventValue
        {
            BookingCreated = new BookingCreated
            {
                BookingId = bookingId,
                CreatedBy = createdBy,
            },
        };
        var producerMessage = new Message<BookingEventKey, BookingEventValue>
        {
            Key = key,
            Value = value,
        };

        await _producer.ProduceAsync(_publisherOptions.BookingCreatedTopic, producerMessage, cancellationToken);
    }

    public async Task PublishBookingCancelled(long bookingId, string cancelledBy, string reason, CancellationToken cancellationToken)
    {
        var key = new BookingEventKey { BookingId = bookingId };
        var value = new BookingEventValue
        {
            BookingCancelled = new BookingCancelled
            {
                BookingId = bookingId,
                CancelledBy = cancelledBy,
                Reason = reason,
            },
        };
        var producerMessage = new Message<BookingEventKey, BookingEventValue>
        {
            Key = key,
            Value = value,
        };

        await _producer.ProduceAsync(_publisherOptions.BookingCancelledTopic, producerMessage, cancellationToken);
    }

    public async Task PublishBookingCompleted(long bookingId, CancellationToken cancellationToken)
    {
        var key = new BookingEventKey { BookingId = bookingId };
        var value = new BookingEventValue
        {
            BookingCompleted = new BookingCompleted
            {
                BookingId = bookingId,
            },
        };
        var producerMessage = new Message<BookingEventKey, BookingEventValue>
        {
            Key = key,
            Value = value,
        };

        await _producer.ProduceAsync(_publisherOptions.BookingCompletedTopic, producerMessage, cancellationToken);
    }
}