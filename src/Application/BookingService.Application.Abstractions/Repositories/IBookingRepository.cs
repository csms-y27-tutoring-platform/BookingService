using BookingService.Application.Abstractions.Queries;
using BookingService.Application.Contracts.DTO;
using BookingService.Application.Domain.Entities;
using BookingService.Application.Domain.Enums;

namespace BookingService.Application.Abstractions.Repositories;

public interface IBookingRepository
{
    Task<Guid> CreateAsync(Booking booking);

    Task<int> UpdateAsync(Guid bookingId, BookingStatus status);

    Task<BookingDto> GetByIdAsync(Guid bookingId);

    IAsyncEnumerable<BookingDto> QueryBookingsAsync(BookingQuery query);
}