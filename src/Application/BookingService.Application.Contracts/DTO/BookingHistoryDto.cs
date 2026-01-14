using BookingService.Application.Domain.Enums;
using BookingService.Application.Domain.Records;

namespace BookingService.Application.Contracts.DTO;

public class BookingHistoryDto
{
    public long BookingHistoryItemId { get; init; }

    public long BookingId { get; init; }

    public BookingHistoryItemKind BookingHistoryItemKind { get; init; }

    public DateTimeOffset BookingHistoryItemCreatedAt { get; init; }

    public required HistoryItemPayload BookingHistoryItemPayload { get; init; }
}