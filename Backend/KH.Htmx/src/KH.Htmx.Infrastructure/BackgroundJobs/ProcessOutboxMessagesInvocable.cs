using System.Text.Json;
using Coravel.Invocable;
using KH.Htmx.Domain.Primitives;
using KH.Htmx.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KH.Htmx.Infrastructure.BackgroundJobs;

public sealed class ProcessOutboxMessagesInvocable(
    KhDbContext dbContext,
    IPublisher publisher,
    ILogger<ProcessOutboxMessagesInvocable> logger
) : IInvocable
{
    private const int BatchSize = 100;

    public async Task Invoke()
    {
        try
        {
            var outboxMessages = await dbContext
                .OutboxMessages
                .Where(x => x.ProcessedOnUtc == null)
                .Take(BatchSize)
                .ToListAsync();

            foreach (var outboxMessage in outboxMessages)
            {
                var domainEvent = JsonSerializer.Deserialize<IDomainEvent>(outboxMessage.Payload);

                if (domainEvent is null)
                {
                    logger.LogWarning("Failed to deserialize domain event with ID {Id}", outboxMessage.Id);
                    // How did this happen?
                    continue;
                }

                await publisher.Publish(domainEvent);

                outboxMessage.ProcessedOnUtc = TimeProvider.System.GetUtcNow();
            }

            await dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to process outbox messages");
        }
    }
}
