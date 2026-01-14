using BookingService.Application.Contracts.Services;
using BookingService.Infrastructure.Grpc.GrpcClients;
using BookingService.Infrastructure.Grpc.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tutor.Service;

namespace BookingService.Infrastructure.Grpc.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGrpcClients(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddOptions<TutorServiceOption>().BindConfiguration("Infrastructure:GrpcClients:TutorService");
        serviceCollection.AddGrpcClient<ValidationService.ValidationServiceClient>((sp, o) =>
        {
            IOptions<TutorServiceOption> options = sp.GetRequiredService<IOptions<TutorServiceOption>>();
            o.Address = new Uri(options.Value.Address);
        });
        serviceCollection.AddGrpcClient<ScheduleService.ScheduleServiceClient>((sp, o) =>
        {
            IOptions<TutorServiceOption> options = sp.GetRequiredService<IOptions<TutorServiceOption>>();
            o.Address = new Uri(options.Value.Address);
        });
        serviceCollection.AddScoped<ITutorServiceClient, TutorClient>();
        
        return serviceCollection;
    }
}