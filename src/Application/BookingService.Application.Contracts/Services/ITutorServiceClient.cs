namespace BookingService.Application.Contracts.Services;

public interface ITutorServiceClient
{
    Task ValidateSlotAsync(long tutorId, long timeSlotId, long subjectId);

    Task ReserveSlotAsync(long timeSlotId, long bookingId);
    
    Task ReleaseSlotAsync(long timeSlotId);
}