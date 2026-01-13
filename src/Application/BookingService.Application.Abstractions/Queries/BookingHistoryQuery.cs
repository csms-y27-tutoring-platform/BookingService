using BookingService.Application.Domain.Enums;

namespace BookingService.Application.Abstractions.Queries;

public record BookingHistoryQuery(
    long[] BookingIds,
    BookingHistoryItemKind? Kind,
    long Cursor,
    int PageSize);