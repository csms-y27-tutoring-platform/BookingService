using BookingService.Application.Abstractions.Messaging;
using BookingService.Application.Abstractions.Queries;
using BookingService.Application.Abstractions.Repositories;
using BookingService.Application.Contracts.DTO;
using BookingService.Application.Contracts.Services;
using BookingService.Application.Domain.Entities;
using BookingService.Application.Domain.Enums;
using BookingService.Application.Domain.Records;
using System.Transactions;

namespace BookingService.Application.Services;

public class BookingsService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IBookingHistoryRepository _bookingHistoryRepository;
    private readonly ITutorServiceClient _tutorServiceClient;
    private readonly IBookingEventPublisher _bookingEventPublisher;

    private static TransactionScope Create(IsolationLevel isolationLevel)
    {
        return new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = isolationLevel },
            TransactionScopeAsyncFlowOption.Enabled);
    }

    public BookingsService(IBookingRepository bookingRepository, IBookingHistoryRepository bookingHistoryRepository, ITutorServiceClient tutorServiceClient, IBookingEventPublisher bookingEventPublisher)
    {
        _bookingRepository = bookingRepository;
        _bookingHistoryRepository = bookingHistoryRepository;
        _tutorServiceClient = tutorServiceClient;
        _bookingEventPublisher = bookingEventPublisher;
    }

    public async Task<long> CreateBookingAsync(BookingCreatingDto dto)
    {
        using TransactionScope transaction = Create(IsolationLevel.ReadCommitted);

        await _tutorServiceClient.ValidateSlotAsync(dto.TutorId, dto.TimeSlotId, dto.SubjectId);

        var booking = new Booking
        {
            TutorId = dto.TutorId,
            TimeSlotId = dto.TimeSlotId,
            SubjectId = dto.SubjectId,
            BookingStatus = BookingStatus.Created,
            BookingCreatedAt = DateTimeOffset.Now.UtcDateTime,
            BookingCreatedBy = dto.BookingCreatedBy,
        };
        long bookingId = await _bookingRepository.CreateAsync(booking);

        await _tutorServiceClient.ReserveSlotAsync(booking.TimeSlotId, bookingId);

        var bookingHistory = new BookingHistory
        {
            BookingId = bookingId,
            BookingHistoryItemKind = BookingHistoryItemKind.Created,
            BookingHistoryItemCreatedAt = DateTimeOffset.Now.UtcDateTime,
            BookingHistoryItemPayload = new HistoryItemPayloadBookingCreated(dto.BookingCreatedBy),
        };
        await _bookingHistoryRepository.CreateBookingHistoryAsync(bookingHistory);

        await _bookingEventPublisher.PublishBookingCreated(bookingId, dto.BookingCreatedBy, CancellationToken.None);

        transaction.Complete();
        return bookingId;
    }

    public async Task<int> CancelBookingAsync(long bookingId, string cancelledBy, string reason)
    {
        using TransactionScope transaction = Create(IsolationLevel.ReadCommitted);

        BookingDto bookingDto = await _bookingRepository.GetByIdAsync(bookingId);

        if (bookingDto == null)
        {
            throw new InvalidOperationException("Booking not found");
        }

        if (bookingDto.BookingCreatedBy != cancelledBy)
        {
            throw new InvalidOperationException("You don't have access to cancel this booking");
        }

        if (bookingDto.BookingStatus != BookingStatus.Created)
        {
            throw new InvalidOperationException("Only created booking can be cancelled");
        }

        if (DateTimeOffset.UtcNow - bookingDto.BookingCreatedAt > TimeSpan.FromHours(24))
        {
            throw new InvalidOperationException("You can't cancel your reservation after 24 hours");
        }

        int answer = await _bookingRepository.UpdateAsync(bookingId, BookingStatus.Cancelled);

        await _tutorServiceClient.ReleaseSlotAsync(bookingDto.TimeSlotId);

        var bookingHistory = new BookingHistory
        {
            BookingId = bookingId,
            BookingHistoryItemKind = BookingHistoryItemKind.Cancelled,
            BookingHistoryItemCreatedAt = DateTimeOffset.Now.UtcDateTime,
            BookingHistoryItemPayload = new HistoryItemPayloadBookingCancelled(cancelledBy, reason),
        };
        await _bookingHistoryRepository.CreateBookingHistoryAsync(bookingHistory);

        await _bookingEventPublisher.PublishBookingCancelled(bookingId, cancelledBy, reason, CancellationToken.None);

        transaction.Complete();
        return answer;
    }

    public async Task<int> CompleteBookingAsync(long bookingId)
    {
        using TransactionScope transaction = Create(IsolationLevel.ReadCommitted);

        BookingDto bookingDto = await _bookingRepository.GetByIdAsync(bookingId);
        if (bookingDto == null)
        {
            throw new InvalidOperationException("Booking not found");
        }

        if (bookingDto.BookingStatus != BookingStatus.Created)
        {
            throw new InvalidOperationException("Only created booking can be completed");
        }

        int answer = await _bookingRepository.UpdateAsync(bookingId, BookingStatus.Completed);

        var bookingHistory = new BookingHistory
        {
            BookingId = bookingId,
            BookingHistoryItemKind = BookingHistoryItemKind.Completed,
            BookingHistoryItemCreatedAt = DateTimeOffset.Now.UtcDateTime,
            BookingHistoryItemPayload = new HistoryItemPayloadBookingCompleted(),
        };
        await _bookingHistoryRepository.CreateBookingHistoryAsync(bookingHistory);

        await _bookingEventPublisher.PublishBookingCompleted(bookingId, CancellationToken.None);

        transaction.Complete();
        return answer;
    }

    public async Task<BookingDto> GetBookingByIdAsync(long bookingId)
    {
        using TransactionScope transaction = Create(IsolationLevel.ReadCommitted);

        BookingDto bookingDto = await _bookingRepository.GetByIdAsync(bookingId);

        transaction.Complete();
        return bookingDto;
    }

    public async IAsyncEnumerable<BookingDto> QueryBookingsAsync(long[] ids, long tutorId, long subjectId, BookingStatus? status, string? bookingCreatedBy, long cursor, int pageSize)
    {
        using TransactionScope transaction = Create(IsolationLevel.ReadCommitted);

        var bookingQuery = new BookingQuery(ids, tutorId, subjectId, status, bookingCreatedBy, cursor, pageSize);
        IAsyncEnumerable<BookingDto> bookings = _bookingRepository.QueryBookingsAsync(bookingQuery);

        await foreach (BookingDto bookingDto in bookings)
        {
            yield return new BookingDto
            {
                BookingId = bookingDto.BookingId,
                TutorId = bookingDto.TutorId,
                TimeSlotId = bookingDto.TimeSlotId,
                SubjectId = bookingDto.SubjectId,
                BookingStatus = bookingDto.BookingStatus,
                BookingCreatedAt = bookingDto.BookingCreatedAt,
                BookingCreatedBy = bookingDto.BookingCreatedBy,
            };
        }

        transaction.Complete();
    }

    public async IAsyncEnumerable<BookingHistoryDto> QueryBookingHistoryAsync(
        long[] bookingIds,
        BookingHistoryItemKind? kind,
        long cursor,
        int pageSize)
    {
        using TransactionScope transaction = Create(IsolationLevel.ReadCommitted);

        var bookingHistoryQuery = new BookingHistoryQuery(bookingIds, kind, cursor, pageSize);
        IAsyncEnumerable<BookingHistoryDto> bookingHistory = _bookingHistoryRepository.QueryBookingHistoryAsync(bookingHistoryQuery);

        await foreach (BookingHistoryDto bookingHistoryDto in bookingHistory)
        {
            yield return new BookingHistoryDto
            {
                BookingHistoryItemId = bookingHistoryDto.BookingHistoryItemId,
                BookingId = bookingHistoryDto.BookingId,
                BookingHistoryItemKind = bookingHistoryDto.BookingHistoryItemKind,
                BookingHistoryItemCreatedAt = bookingHistoryDto.BookingHistoryItemCreatedAt,
                BookingHistoryItemPayload = bookingHistoryDto.BookingHistoryItemPayload,
            };
        }

        transaction.Complete();
    }
}