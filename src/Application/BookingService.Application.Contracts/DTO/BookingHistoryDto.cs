using BookingService.Application.Domain.Enums;
using BookingService.Application.Domain.Records;

namespace BookingService.Application.Contracts.DTO;

public class BookingHistoryDto
{
    public Guid BookingHistoryItemId { get; init; }

    public Guid BookingId { get; init; }

    public BookingHistoryItemKind BookingHistoryItemKind { get; init; }

    public DateTimeOffset BookingHistoryItemCreatedAt { get; init; }

    public required HistoryItemPayload BookingHistoryItemPayload { get; init; }
}