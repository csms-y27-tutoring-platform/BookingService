namespace BookingService.Presentation.Grpc.Extensions;

public static class MapperToGrpcEnum
{
    public static BookingStatus MapperToGrpc(this Application.Domain.Enums.BookingStatus status)
    {
        BookingStatus grpcStatus = status switch
        {
            Application.Domain.Enums.BookingStatus.Created => BookingStatus.BookingCreated,
            Application.Domain.Enums.BookingStatus.Cancelled => BookingStatus.BookingCancelled,
            Application.Domain.Enums.BookingStatus.Completed => BookingStatus.BookingCompleted,
            _ => BookingStatus.BookingUnspecified,
        };
        return grpcStatus;
    }

    public static BookingHistoryItemKind MapperToGrpc(this Application.Domain.Enums.BookingHistoryItemKind kind)
    {
        BookingHistoryItemKind bookingHistoryItemKind = kind switch
        {
            Application.Domain.Enums.BookingHistoryItemKind.Created => BookingHistoryItemKind.BookingHistoryItemCreated,
            Application.Domain.Enums.BookingHistoryItemKind.Cancelled => BookingHistoryItemKind.BookingHistoryItemCancelled,
            Application.Domain.Enums.BookingHistoryItemKind.Completed => BookingHistoryItemKind.BookingHistoryItemCompleted,
            _ => BookingHistoryItemKind.BookingHistoryItemUnspecified,
        };
        return bookingHistoryItemKind;
    }
}