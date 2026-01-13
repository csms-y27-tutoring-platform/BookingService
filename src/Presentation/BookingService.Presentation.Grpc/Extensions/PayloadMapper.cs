using BookingService.Application.Domain.Records;

namespace BookingService.Presentation.Grpc.Extensions;

public static class PayloadMapper
{
    public static BookingHistoryPayload MapperToGrpc(this HistoryItemPayload payload)
    {
        BookingHistoryPayload historyPayload = payload switch
        {
            HistoryItemPayloadBookingCreated created => new BookingHistoryPayload
            {
                Created = new HistoryItemPayloadCreated
                {
                    CreatedBy = created.CreatedBy,
                },
            },

            HistoryItemPayloadBookingCancelled cancelled => new BookingHistoryPayload
            {
                Cancelled = new HistoryItemPayloadCancelled
                {
                    CancelledBy = cancelled.CancelledBy,
                    Reason = cancelled.Reason,
                },
            },

            HistoryItemPayloadBookingCompleted => new BookingHistoryPayload
            {
                Completed = new HistoryItemPayloadCompleted(),
            },
            _ => throw new ArgumentOutOfRangeException(nameof(payload), payload, null),
        };
        return historyPayload;
    }
}