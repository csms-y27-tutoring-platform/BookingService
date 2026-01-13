namespace BookingService.Application.Domain.Records;

public record HistoryItemPayloadBookingCancelled(string CancelledBy, string Reason) : HistoryItemPayload;