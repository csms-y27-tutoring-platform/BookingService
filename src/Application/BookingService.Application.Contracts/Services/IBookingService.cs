using BookingService.Application.Contracts.DTO;
using BookingService.Application.Domain.Enums;

namespace BookingService.Application.Contracts.Services;

public interface IBookingService
{
    Task<long> CreateBookingAsync(BookingCreatingDto dto);
    
    Task<int> CancelBookingAsync(long bookingId, string cancelledBy, string reason);
    
    Task<int> CompleteBookingAsync(long bookingId);
    
    Task<BookingDto> GetBookingByIdAsync(long bookingId);
    
    IAsyncEnumerable<BookingDto> QueryBookingsAsync(long[] ids, long tutorId, long subjectId, BookingStatus? status, string? bookingCreatedBy, long cursor, int pageSize);
    
    IAsyncEnumerable<BookingHistoryDto> QueryBookingHistoryAsync(long[] bookingIds, BookingHistoryItemKind? kind, long cursor, int pageSize);
}