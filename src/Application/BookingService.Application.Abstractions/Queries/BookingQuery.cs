using BookingService.Application.Domain.Enums;

namespace BookingService.Application.Abstractions.Queries;

public record BookingQuery(
    long[] Ids,
    long? TutorId,
    long? SubjectId,
    BookingStatus? Status,
    string? BookingCreatedBy,
    long Cursor,
    int PageSize);