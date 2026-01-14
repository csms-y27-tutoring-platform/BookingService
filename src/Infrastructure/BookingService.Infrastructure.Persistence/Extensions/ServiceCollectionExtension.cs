using BookingService.Application.Abstractions.Repositories;
using BookingService.Infrastructure.Persistence.Connections;
using BookingService.Infrastructure.Persistence.Migrations;
using BookingService.Infrastructure.Persistence.Repositories;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;

namespace BookingService.Infrastructure.Persistence.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddPersistence(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddOptions<PostgresOptions>().Bind(configuration.GetSection("Infrastructure:Persistence:Postgres"));
        serviceCollection.AddSingleton<NpgsqlDataSourceBuilder>(_ => new NpgsqlDataSourceBuilder());
        serviceCollection.AddSingleton<PostgresProvider>();

        serviceCollection.AddFluentMigratorCore()
            .ConfigureRunner(runner =>
                runner.AddPostgres()
                    .WithGlobalConnectionString(provider =>
                    {
                        NpgsqlDataSourceBuilder postgresBuilder =
                            provider.GetRequiredService<NpgsqlDataSourceBuilder>();
                        PostgresOptions postgresOptions =
                            provider.GetRequiredService<IOptions<PostgresOptions>>().Value;
                        NpgsqlConnectionStringBuilder connectionString = postgresBuilder.ConnectionStringBuilder;
                        connectionString.Database = postgresOptions.Database;
                        connectionString.Host = postgresOptions.Host;
                        connectionString.Port = postgresOptions.Port;
                        connectionString.Username = postgresOptions.Username;
                        connectionString.Password = postgresOptions.Password;
                        return connectionString.ToString();
                    })
                    .WithMigrationsIn(typeof(InitialMigration).Assembly));

        serviceCollection.AddScoped<IBookingHistoryRepository, BookingHistoryRepository>();
        serviceCollection.AddScoped<IBookingRepository, BookingRepository>();

        return serviceCollection;
    }
}