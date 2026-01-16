namespace BookingService.Presentation.Grpc.Extensions;

public static class MapperToDomainEnum
{
    public static Application.Domain.Enums.BookingStatus? MapperToDomain(this BookingStatus status)
    {
        Application.Domain.Enums.BookingStatus? domainStatus = status switch
        {
            BookingStatus.BookingCreated => Application.Domain.Enums.BookingStatus.Created,
            BookingStatus.BookingCancelled => Application.Domain.Enums.BookingStatus.Cancelled,
            BookingStatus.BookingCompleted => Application.Domain.Enums.BookingStatus.Completed,
            BookingStatus.BookingUnspecified => null,
            _ => throw new ArgumentOutOfRangeException(nameof(status)),
        };
        return domainStatus;
    }

    public static Application.Domain.Enums.BookingHistoryItemKind? MapperToDomain(this BookingHistoryItemKind kind)
    {
        Application.Domain.Enums.BookingHistoryItemKind? domainKind = kind switch
        {
            BookingHistoryItemKind.BookingHistoryItemCreated => Application.Domain.Enums.BookingHistoryItemKind.Created,
            BookingHistoryItemKind.BookingHistoryItemCancelled => Application.Domain.Enums.BookingHistoryItemKind.Cancelled,
            BookingHistoryItemKind.BookingHistoryItemCompleted => Application.Domain.Enums.BookingHistoryItemKind.Completed,
            BookingHistoryItemKind.BookingHistoryItemUnspecified => null,
            _ => throw new ArgumentOutOfRangeException(nameof(kind)),
        };
        return domainKind;
    }
}