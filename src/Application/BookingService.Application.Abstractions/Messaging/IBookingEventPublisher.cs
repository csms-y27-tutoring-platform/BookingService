namespace BookingService.Application.Abstractions.Messaging;

public interface IBookingEventPublisher
{
    Task PublishBookingCreated(Guid bookingId, string createdBy, CancellationToken cancellationToken);

    Task PublishBookingCancelled(Guid bookingId, string cancelledBy, string reason, CancellationToken cancellationToken);

    Task PublishBookingCompleted(Guid bookingId, CancellationToken cancellationToken);
}