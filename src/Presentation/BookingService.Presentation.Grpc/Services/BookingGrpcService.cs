using BookingService.Application.Contracts.DTO;
using BookingService.Application.Contracts.Services;
using BookingService.Presentation.Grpc.Extensions;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace BookingService.Presentation.Grpc.Services;

public class BookingGrpcService : BookingService.BookingServiceBase
{
    private readonly IBookingService _bookingService;

    public BookingGrpcService(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    public override async Task<BookingResponse> CreateBooking(BookingRequest request, ServerCallContext context)
    {
        var dto = new BookingCreatingDto
        {
            TutorId = request.TutorId,
            TimeSlotId = request.TimeSlotId,
            SubjectId = request.SubjectId,
            BookingCreatedBy = request.Name,
        };
        long bookingId = await _bookingService.CreateBookingAsync(dto);
        return new BookingResponse { BookingId = bookingId };
    }

    public override async Task<CancelBookingResponse> CancelBooking(CancelBookingRequest request, ServerCallContext context)
    {
        int result = await _bookingService.CancelBookingAsync(request.BookingId, request.Name, request.Reason);
        return new CancelBookingResponse { Result = result };
    }

    public override async Task<CompleteBookingResponse> CompleteBooking(CompleteBookingRequest request, ServerCallContext context)
    {
        int result = await _bookingService.CompleteBookingAsync(request.BookingId);
        return new CompleteBookingResponse { Result = result };
    }

    public override async Task<GetBookingResponse> GetBookingById(GetBookingRequest request, ServerCallContext context)
    {
        BookingDto dto = await _bookingService.GetBookingByIdAsync(request.BookingId);
        var booking = new Booking
        {
            BookingId = dto.BookingId,
            TutorId = dto.TutorId,
            TimeSlotId = dto.TimeSlotId,
            SubjectId = dto.SubjectId,
            Status = dto.BookingStatus.MapperToGrpc(),
            CreatedAt = dto.BookingCreatedAt.ToTimestamp(),
            Name = dto.BookingCreatedBy,
        };
        return new GetBookingResponse { Booking = booking };
    }

    public override async Task<QueryBookingsResponse> QueryBookings(QueryBookingsRequest request, ServerCallContext context)
    {
        long[] ids = request.Ids.ToArray<long>();
        long? tutorId = request.TutorId;
        long? subjectId = request.SubjectId;
        Application.Domain.Enums.BookingStatus? status = request.Status.MapperToDomain();
        string? name = request.Name;
        long cursor = request.Cursor;
        int pageSize = request.PageSize;
        IAsyncEnumerable<BookingDto> bookings = _bookingService.QueryBookingsAsync(ids, tutorId, subjectId, status, name, cursor, pageSize);
        var response = new QueryBookingsResponse();
        await foreach (BookingDto dto in bookings)
        {
            response.Bookings.Add(new Booking
            {
                BookingId = dto.BookingId,
                TutorId = dto.TutorId,
                TimeSlotId = dto.TimeSlotId,
                SubjectId = dto.SubjectId,
                Status = dto.BookingStatus.MapperToGrpc(),
                CreatedAt = dto.BookingCreatedAt.ToTimestamp(),
                Name = dto.BookingCreatedBy,
            });
        }

        return response;
    }

    public override async Task<QueryBookingHistoryResponse> QueryBookingHistory(
        QueryBookingHistoryRequest request,
        ServerCallContext context)
    {
        long[] ids = request.Ids.ToArray<long>();
        Application.Domain.Enums.BookingHistoryItemKind? kind = request.Kind.MapperToDomain();
        long cursor = request.Cursor;
        int pageSize = request.PageSize;
        IAsyncEnumerable<BookingHistoryDto> bookingHistory =
            _bookingService.QueryBookingHistoryAsync(ids, kind, cursor, pageSize);
        var response = new QueryBookingHistoryResponse();
        await foreach (BookingHistoryDto dto in bookingHistory) response.History.Add(dto.MapperToProto());
        return response;
    }
}