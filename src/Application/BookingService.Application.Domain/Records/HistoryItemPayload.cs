using System.Text.Json.Serialization;

namespace BookingService.Application.Domain.Records;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(HistoryItemPayloadBookingCreated), typeDiscriminator: nameof(HistoryItemPayloadBookingCreated))]
[JsonDerivedType(typeof(HistoryItemPayloadBookingCancelled), typeDiscriminator: nameof(HistoryItemPayloadBookingCancelled))]
[JsonDerivedType(typeof(HistoryItemPayloadBookingCompleted), typeDiscriminator: nameof(HistoryItemPayloadBookingCompleted))]
public abstract record HistoryItemPayload();