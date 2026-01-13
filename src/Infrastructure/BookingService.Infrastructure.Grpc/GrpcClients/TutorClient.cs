using BookingService.Application.Contracts.Services;
using Tutor.Service;

namespace BookingService.Infrastructure.Grpc.GrpcClients;

public class TutorClient : ITutorServiceClient
{
    private readonly ValidationService.ValidationServiceClient _validationService;
    private readonly ScheduleService.ScheduleServiceClient _scheduleService;

    public TutorClient(ValidationService.ValidationServiceClient validationService,
        ScheduleService.ScheduleServiceClient scheduleService)
    {
        _validationService = validationService;
        _scheduleService = scheduleService;
    }

    public async Task ValidateSlotAsync(long tutorId, long timeSlotId, long subjectId)
    {
        ValidateSlotRequest request = new ValidateSlotRequest
        {
            SlotId = timeSlotId.ToString(),
            TutorId = tutorId.ToString(),
            SubjectId = subjectId.ToString()
        };
        ValidateSlotResponse response = await _validationService.ValidateSlotAsync(request);
        if (!response.IsValid)
        {
            throw new InvalidOperationException("Slot is not valid for booking");
        }
    }

    public async Task ReserveSlotAsync(long timeSlotId, long bookingId)
    {
        ReserveSlotRequest request = new ReserveSlotRequest
        {
            SlotId = timeSlotId.ToString(),
            BookingId = bookingId.ToString()
        };
        await _scheduleService.ReserveSlotAsync(request);
    }

    public async Task ReleaseSlotAsync(long timeSlotId)
    {
        ReleaseSlotRequest request = new ReleaseSlotRequest
        {
            SlotId = timeSlotId.ToString()
        };
        await _scheduleService.ReleaseSlotAsync(request);
    }
}