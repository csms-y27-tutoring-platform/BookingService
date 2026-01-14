using BookingService.Application.Extensions;
using BookingService.Infrastructure.Grpc.Extensions;
using BookingService.Infrastructure.Kafka.Extension;
using BookingService.Infrastructure.Persistence.Extensions;
using BookingService.Presentation.Grpc.Services;
using BookingService.Presentation.Kafka.Extensions;
using FluentMigrator.Runner;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json");

builder.Services.AddServices();

builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddGrpcClients();
builder.Services.AddKafkaProducer();

builder.Services.AddKafkaConnections();
builder.Services.AddKafkaProducerOptions();

builder.Services.AddGrpcReflection();
builder.Services.AddGrpc();

WebApplication app = builder.Build();

app.MapGrpcService<BookingGrpcService>();
app.MapGrpcReflectionService();

using IServiceScope scope = app.Services.CreateScope();
IMigrationRunner runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
runner.MigrateUp();

app.Run();