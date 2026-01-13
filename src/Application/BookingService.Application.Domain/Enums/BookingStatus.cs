using NpgsqlTypes;

namespace BookingService.Application.Domain.Enums;

public enum BookingStatus
{
    [PgName("created")]
    Created,
    [PgName("cancelled")]
    Cancelled,
    [PgName("completed")]
    Completed
}