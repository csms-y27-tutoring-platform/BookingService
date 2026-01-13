using FluentMigrator;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;

namespace BookingService.Infrastructure.Persistence.Migrations;

public class InitialMigration : IMigration
{
    public void GetUpExpressions(IMigrationContext context)
    {
        context.Expressions.Add(new ExecuteSqlStatementExpression
        {
            SqlStatement = """
                           create type if not exists booking_status as enum ('created', 'cancelled', 'completed');
                           
                           create table if not exists bookings
                           (
                               booking_id         bigint primary key generated always as identity,
                               
                               tutor_id           bigint                   not null,
                               time_slot_id       bigint                   not null,
                               subject_id         bigint                   not null,
                               booking_status     booking_status           not null,
                               booking_created_by text                     not null,
                               booking_created_at timestamp with time zone not null,
                           );
                           
                           create type if not exists booking_history_item_kind as enum ('created', 'cancelled', 'completed');

                           create table if not exists booking_history
                           (
                               booking_history_item_id         bigint primary key generated always as identity,
                               
                               booking_id                      bigint                    not null references bookings (booking_id),
                               booking_history_item_kind       booking_history_item_kind not null,
                               booking_history_item_created_at timestamp with time zone  not null,
                               booking_history_item_payload    jsonb                     not null
                           );
                           """
        });
    }

    public void GetDownExpressions(IMigrationContext context)
    {
        context.Expressions.Add(new ExecuteSqlStatementExpression
        {
            SqlStatement = """
                           drop type  if exists booking_status;
                           drop table if exists bookings;
                           drop type  if exists booking_history_item_kind;
                           drop table if exists booking_history;
                           """
        });
    }

    public string ConnectionString => throw new NotSupportedException();
}