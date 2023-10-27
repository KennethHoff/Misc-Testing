using System.Text.Json;
using Khtmx.Domain.Primitives;
using Khtmx.Persistence.Outbox;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Khtmx.Persistence.Interceptors;

public sealed class ConvertDomainEventsToOutboxMessagesInterceptor : SaveChangesInterceptor
{
    private static readonly TimeProvider TimeProvider = TimeProvider.System;

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not KhDbContext context)
        {
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        var outboxMessages = context.ChangeTracker.Entries<IAggregateRoot>()
            .Select(x => x.Entity)
            .SelectMany(aggregateRoot =>
            {
                var domainEvents = aggregateRoot.GetDomainEvents();

                aggregateRoot.ClearDomainEvents();

                return domainEvents;
            })
            .Select(domainEvent =>
            {
                var serialize = JsonSerializer.Serialize(domainEvent);
                return new OutboxMessage
                {
                    Id = OutboxMessageId.New(),
                    OccurredOnUtc = TimeProvider.GetUtcNow(),
                    Type = domainEvent.GetType().Name,
                    Payload = serialize,
                };
            })
            .ToList();

        context.OutboxMessages.AddRange(outboxMessages);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
