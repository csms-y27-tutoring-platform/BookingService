namespace BookingService.Application.Contracts.DTO;

public class BookingCreatingDto
{
    public long TutorId { get; init; }

    public long TimeSlotId { get; init; }

    public long SubjectId { get; init; }

    public required string BookingCreatedBy { get; init; }
}