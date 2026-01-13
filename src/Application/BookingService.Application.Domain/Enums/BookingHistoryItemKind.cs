using NpgsqlTypes;

namespace BookingService.Application.Domain.Enums;

public enum BookingHistoryItemKind
{
    [PgName("created")]
    Created,
    [PgName("cancelled")]
    Cancelled,
    [PgName("completed")]
    Completed
}