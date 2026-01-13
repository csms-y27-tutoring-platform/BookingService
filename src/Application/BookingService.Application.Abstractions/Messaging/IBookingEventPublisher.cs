namespace BookingService.Application.Abstractions.Messaging;

public interface IBookingEventPublisher
{
    Task PublishBookingCreated(long bookingId, string createdBy, CancellationToken cancellationToken);
    
    Task PublishBookingCancelled(long bookingId, string cancelledBy, string reason, CancellationToken cancellationToken);

    Task PublishBookingCompleted(long bookingId, CancellationToken cancellationToken);
}