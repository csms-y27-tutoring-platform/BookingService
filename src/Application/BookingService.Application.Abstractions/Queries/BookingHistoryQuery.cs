using BookingService.Application.Domain.Enums;

namespace BookingService.Application.Abstractions.Queries;

public record BookingHistoryQuery(
    Guid[] BookingIds,
    BookingHistoryItemKind? Kind,
    Guid Cursor,
    int PageSize);