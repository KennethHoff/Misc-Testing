using Khtmx.Domain.Primitives;

namespace Khtmx.Persistence.Outbox;

public readonly record struct OutboxMessageId(Guid Value) : ITypedId<OutboxMessageId>
{
    public static OutboxMessageId From(Guid value) => new(value);
    public static OutboxMessageId New() => new(Guid.NewGuid());
}