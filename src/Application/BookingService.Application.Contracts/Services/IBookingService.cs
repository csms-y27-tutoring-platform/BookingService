using BookingService.Application.Contracts.DTO;
using BookingService.Application.Domain.Enums;

namespace BookingService.Application.Contracts.Services;

public interface IBookingService
{
    Task<Guid> CreateBookingAsync(BookingCreatingDto dto);

    Task<int> CancelBookingAsync(Guid bookingId, string cancelledBy, string reason);

    Task<int> CompleteBookingAsync(Guid bookingId);

    Task<BookingDto> GetBookingByIdAsync(Guid bookingId);

    IAsyncEnumerable<BookingDto> QueryBookingsAsync(Guid[] ids, Guid? tutorId, Guid? subjectId, BookingStatus? status, string? bookingCreatedBy, Guid cursor, int pageSize);

    IAsyncEnumerable<BookingHistoryDto> QueryBookingHistoryAsync(Guid[] bookingIds, BookingHistoryItemKind? kind, Guid cursor, int pageSize);
}