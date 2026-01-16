namespace BookingService.Application.Contracts.Services;

public interface ITutorServiceClient
{
    Task ValidateSlotAsync(Guid tutorId, Guid timeSlotId, Guid subjectId);

    Task ReserveSlotAsync(Guid timeSlotId, Guid bookingId);

    Task ReleaseSlotAsync(Guid timeSlotId);
}