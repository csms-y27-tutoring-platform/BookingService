using BookingService.Application.Abstractions.Queries;
using BookingService.Application.Contracts.DTO;
using BookingService.Application.Domain.Entities;

namespace BookingService.Application.Abstractions.Repositories;

public interface IBookingHistoryRepository
{
    Task<Guid> CreateBookingHistoryAsync(BookingHistory bookingHistory);

    IAsyncEnumerable<BookingHistoryDto> QueryBookingHistoryAsync(BookingHistoryQuery query);
}