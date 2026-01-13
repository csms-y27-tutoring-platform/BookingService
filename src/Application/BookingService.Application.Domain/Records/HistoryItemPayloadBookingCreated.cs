namespace BookingService.Application.Domain.Records;

public record HistoryItemPayloadBookingCreated(string CreatedBy) : HistoryItemPayload;