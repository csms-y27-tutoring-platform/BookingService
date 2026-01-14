using BookingService.Application.Domain.Enums;
using Microsoft.Extensions.Options;
using Npgsql;

namespace BookingService.Infrastructure.Persistence.Connections;

public class PostgresProvider
{
    private readonly NpgsqlDataSource _npgsqlDataSource;

    public PostgresProvider(NpgsqlDataSourceBuilder npgsqlDataSourceBuilder, IOptions<PostgresOptions> postgresOptions)
    {
        PostgresOptions options = postgresOptions.Value;
        NpgsqlConnectionStringBuilder connectionString = npgsqlDataSourceBuilder.ConnectionStringBuilder;
        connectionString.Host = options.Host;
        connectionString.Username = options.Username;
        connectionString.Password = options.Password;
        connectionString.Port = options.Port;

        npgsqlDataSourceBuilder.MapEnum<BookingStatus>(pgName: "booking_status");
        npgsqlDataSourceBuilder.MapEnum<BookingHistoryItemKind>(pgName: "booking_history_item_kind");

        _npgsqlDataSource = npgsqlDataSourceBuilder.Build();
    }

    public async Task<NpgsqlConnection> OpenConnection()
    {
        return await _npgsqlDataSource.OpenConnectionAsync();
    }
}