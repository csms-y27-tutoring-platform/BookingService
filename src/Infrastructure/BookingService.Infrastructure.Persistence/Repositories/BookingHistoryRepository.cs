using BookingService.Application.Abstractions.Queries;
using BookingService.Application.Abstractions.Repositories;
using BookingService.Application.Contracts.DTO;
using BookingService.Application.Domain.Entities;
using BookingService.Application.Domain.Enums;
using BookingService.Application.Domain.Records;
using BookingService.Infrastructure.Persistence.Connections;
using Npgsql;
using System.Data;
using System.Text.Json;

namespace BookingService.Infrastructure.Persistence.Repositories;

public class BookingHistoryRepository : IBookingHistoryRepository
{
    private readonly PostgresProvider _postgresProvider;

    public BookingHistoryRepository(PostgresProvider postgresProvider)
    {
        _postgresProvider = postgresProvider;
    }

    public async Task<long> CreateBookingHistoryAsync(BookingHistory bookingHistory)
    {
        const string sql = """
                           insert into booking_history (booking_id, booking_history_item_kind, booking_history_item_created_at, booking_history_item_payload)
                           values (:booking_id, :booking_history_item_kind, :booking_history_item_created_at, :booking_history_item_payload)
                           returning booking_history_item_id;
                           """;

        await using NpgsqlConnection connection = await _postgresProvider.OpenConnection();
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("booking_id", bookingHistory.BookingId),
                new NpgsqlParameter("booking_history_item_kind", bookingHistory.BookingHistoryItemKind),
                new NpgsqlParameter("booking_history_item_created_at", bookingHistory.BookingHistoryItemCreatedAt),
                new NpgsqlParameter("booking_history_item_payload", JsonSerializer.Serialize(bookingHistory.BookingHistoryItemPayload)),
            },
        };

        NpgsqlDataReader reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return reader.GetInt64(0);
        }

        throw new InvalidOperationException();
    }

    public async IAsyncEnumerable<BookingHistoryDto> QueryBookingHistoryAsync(BookingHistoryQuery query)
    {
        const string sql = """
                           select booking_history_item_id, booking_id, booking_history_item_kind, booking_history_item_created_at, booking_history_item_payload
                           from booking_history
                           where
                               (booking_history_item_id > :cursor)
                               and (cardinality(:booking_ids) = 0 or booking_id = any(:booking_ids))
                               and (:booking_history_item_kind::booking_history_item_kind is null or booking_history_item_kind = :booking_history_item_kind)
                               limit page_size
                           """;

        await using NpgsqlConnection connection = await _postgresProvider.OpenConnection();
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("booking_ids", query.BookingIds),
                new NpgsqlParameter("booking_history_item_kind", query.Kind is null ? DBNull.Value : query.Kind),
                new NpgsqlParameter("cursor", query.Cursor),
                new NpgsqlParameter("page_size", query.PageSize),
            },
        };

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            HistoryItemPayload payload = JsonSerializer.Deserialize<HistoryItemPayload>(DataReaderExtensions.GetString(reader, "booking_history_item_payload")) ?? throw new InvalidOperationException();
            yield return new BookingHistoryDto
            {
                BookingHistoryItemId = DataReaderExtensions.GetInt64(reader, "booking_history_item_id"),
                BookingId = DataReaderExtensions.GetInt64(reader, "booking_id"),
                BookingHistoryItemKind =
                    DataReaderExtensions.GetFieldValue<BookingHistoryItemKind>(reader, "booking_history_item_kind"),
                BookingHistoryItemCreatedAt =
                    DataReaderExtensions.GetFieldValue<DateTimeOffset>(reader, "booking_history_item_created_at"),
                BookingHistoryItemPayload = payload,
            };
        }
    }
}