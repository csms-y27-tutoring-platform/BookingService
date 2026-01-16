using BookingService.Application.Abstractions.Queries;
using BookingService.Application.Abstractions.Repositories;
using BookingService.Application.Contracts.DTO;
using BookingService.Application.Domain.Entities;
using BookingService.Application.Domain.Enums;
using BookingService.Infrastructure.Persistence.Connections;
using Npgsql;
using NpgsqlTypes;
using System.Data;

namespace BookingService.Infrastructure.Persistence.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly PostgresProvider _postgresProvider;

    public BookingRepository(PostgresProvider postgresProvider)
    {
        _postgresProvider = postgresProvider;
    }

    public async Task<long> CreateAsync(Booking booking)
    {
        const string sql = """
                           insert into bookings (tutor_id, time_slot_id, subject_id, booking_status, booking_created_by, booking_created_at)
                           values (:tutor_id, :time_slot_id, :subject_id, :booking_status, :booking_created_by, :booking_created_at)
                           returning booking_id;
                           """;
        await using NpgsqlConnection connection = await _postgresProvider.OpenConnection();

        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("tutor_id", booking.TutorId),
                new NpgsqlParameter("time_slot_id", booking.TimeSlotId),
                new NpgsqlParameter("subject_id", booking.SubjectId),
                new NpgsqlParameter("booking_status", booking.BookingStatus),
                new NpgsqlParameter("booking_created_by", booking.BookingCreatedBy),
                new NpgsqlParameter("booking_created_at", booking.BookingCreatedAt),
            },
        };

        NpgsqlDataReader reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return DataReaderExtensions.GetInt64(reader, "booking_id");
        }

        throw new InvalidOperationException();
    }

    public async Task<int> UpdateAsync(long bookingId, BookingStatus status)
    {
        const string sql = """
                           update bookings
                           set booking_status = :booking_status
                           where booking_id = :booking_id
                           """;
        await using NpgsqlConnection connection = await _postgresProvider.OpenConnection();

        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("booking_id", bookingId),
                new NpgsqlParameter("booking_status", status),
            },
        };

        NpgsqlDataReader reader = await command.ExecuteReaderAsync();
        return reader.RecordsAffected;
    }

    public async Task<BookingDto> GetByIdAsync(long bookingId)
    {
        const string sql = """
                           select booking_id, tutor_id, time_slot_id, subject_id, booking_status, booking_created_by, booking_created_at
                           from bookings
                           where booking_id = :booking_id;
                           """;
        await using NpgsqlConnection connection = await _postgresProvider.OpenConnection();

        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("booking_id", bookingId),
            },
        };

        NpgsqlDataReader reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new BookingDto
            {
                BookingId = DataReaderExtensions.GetInt64(reader, "booking_id"),
                TutorId = DataReaderExtensions.GetInt64(reader, "tutor_id"),
                TimeSlotId = DataReaderExtensions.GetInt64(reader, "time_slot_id"),
                SubjectId = DataReaderExtensions.GetInt64(reader, "subject_id"),
                BookingStatus = DataReaderExtensions.GetFieldValue<BookingStatus>(reader, "booking_status"),
                BookingCreatedBy = DataReaderExtensions.GetString(reader, "booking_created_by"),
                BookingCreatedAt = DataReaderExtensions.GetFieldValue<DateTimeOffset>(reader, "booking_created_at"),
            };
        }

        throw new InvalidOperationException();
    }

    public async IAsyncEnumerable<BookingDto> QueryBookingsAsync(BookingQuery query)
    {
        const string sql = """
                           select booking_id, tutor_id, time_slot_id, subject_id, booking_status, booking_created_by, booking_created_at
                           from bookings
                           where
                               (booking_id > :cursor)
                               and (booking_id = any (:ids))
                               and (:tutor_id is null or tutor_id = :tutor_id)
                               and (:subject_id is null or subject_id = :subject_id)
                               and (:status::booking_status is null or booking_status = :status)
                               and (:author is null or :author ='' or booking_created_by = :author)
                           limit :page_size;
                           """;
        await using NpgsqlConnection connection = await _postgresProvider.OpenConnection();

        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("ids", query.Ids),
                new NpgsqlParameter("tutor_id", query.TutorId is null ? DBNull.Value : query.TutorId),
                new NpgsqlParameter("subject_id", query.SubjectId is null ? DBNull.Value : query.SubjectId),
                new NpgsqlParameter("status", query.Status is null ? DBNull.Value : query.Status),
                new NpgsqlParameter("author", NpgsqlDbType.Text)
                {
                    Value = string.IsNullOrEmpty(query.BookingCreatedBy)
                    ? DBNull.Value : query.BookingCreatedBy,
                },
                new NpgsqlParameter("cursor", query.Cursor),
                new NpgsqlParameter("page_size", query.PageSize),
            },
        };

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            yield return new BookingDto
            {
                BookingId = DataReaderExtensions.GetInt64(reader, "booking_id"),
                TutorId = DataReaderExtensions.GetInt64(reader, "tutor_id"),
                TimeSlotId = DataReaderExtensions.GetInt64(reader, "time_slot_id"),
                SubjectId = DataReaderExtensions.GetInt64(reader, "subject_id"),
                BookingStatus = DataReaderExtensions.GetFieldValue<BookingStatus>(reader, "booking_status"),
                BookingCreatedBy = DataReaderExtensions.GetString(reader, "booking_created_by"),
                BookingCreatedAt = DataReaderExtensions.GetFieldValue<DateTimeOffset>(reader, "booking_created_at"),
            };
        }
    }
}