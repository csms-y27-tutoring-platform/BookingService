using BookingService.Application.Domain.Enums;

namespace BookingService.Application.Abstractions.Queries;

public record BookingQuery(
    Guid[] Ids,
    Guid? TutorId,
    Guid? SubjectId,
    BookingStatus? Status,
    string? BookingCreatedBy,
    Guid Cursor,
    int PageSize);