namespace Khtmx.Persistence.Outbox;

public sealed class OutboxMessage
{
    public required OutboxMessageId Id { get; init; }
    public required string? Type { get; init; }
    public required string Payload { get; init; }
    public required DateTimeOffset OccurredOnUtc { get; init; }
    public DateTimeOffset? ProcessedOnUtc { get; set; }
}
