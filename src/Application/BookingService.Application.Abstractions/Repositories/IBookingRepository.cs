using BookingService.Application.Abstractions.Queries;
using BookingService.Application.Contracts.DTO;
using BookingService.Application.Domain.Entities;
using BookingService.Application.Domain.Enums;

namespace BookingService.Application.Abstractions.Repositories;

public interface IBookingRepository
{
    Task<long> CreateAsync(Booking booking);

    Task<int> UpdateAsync(long bookingId, BookingStatus status);

    Task<BookingDto> GetByIdAsync(long bookingId);

    IAsyncEnumerable<BookingDto> QueryBookingsAsync(BookingQuery query);
}