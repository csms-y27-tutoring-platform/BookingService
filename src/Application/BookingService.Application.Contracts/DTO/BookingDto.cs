using BookingService.Application.Domain.Enums;

namespace BookingService.Application.Contracts.DTO;

public class BookingDto
{
    public Guid BookingId { get; init; }

    public Guid TutorId { get; init; }

    public Guid TimeSlotId { get; init; }

    public Guid SubjectId { get; init; }

    public BookingStatus BookingStatus { get; init; }

    public DateTimeOffset BookingCreatedAt { get; init; }

    public required string BookingCreatedBy { get; init; }
}