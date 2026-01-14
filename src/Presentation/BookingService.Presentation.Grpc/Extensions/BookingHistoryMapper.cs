using BookingService.Application.Contracts.DTO;
using Google.Protobuf.WellKnownTypes;

namespace BookingService.Presentation.Grpc.Extensions;

public static class BookingHistoryMapper
{
    public static BookingHistory MapperToProto(this BookingHistoryDto dto)
    {
        var history = new BookingHistory
        {
            BookingHistoryItemId = dto.BookingHistoryItemId,
            BookingId = dto.BookingId,
            Kind = dto.BookingHistoryItemKind.MapperToGrpc(),
            CreatedAt = dto.BookingHistoryItemCreatedAt.ToTimestamp(),
            Payload = dto.BookingHistoryItemPayload.MapperToGrpc(),
        };
        return history;
    }
}