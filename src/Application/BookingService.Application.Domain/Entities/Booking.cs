using BookingService.Application.Domain.Enums;

namespace BookingService.Application.Domain.Entities;

public class Booking
{
    public long BookingId { get; init; }
    
    public long TutorId { get; init; }
    
    public long TimeSlotId { get; init; }
    
    public long SubjectId { get; init; }
    
    public BookingStatus BookingStatus { get; init; }
    
    public DateTimeOffset BookingCreatedAt { get; init; }
    
    public required string BookingCreatedBy { get; init; }
}