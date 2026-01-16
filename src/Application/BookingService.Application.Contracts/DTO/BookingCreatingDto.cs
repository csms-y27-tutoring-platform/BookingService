namespace BookingService.Application.Contracts.DTO;

public class BookingCreatingDto
{
    public Guid TutorId { get; init; }

    public Guid TimeSlotId { get; init; }

    public Guid SubjectId { get; init; }

    public required string BookingCreatedBy { get; init; }
}