using KHtmx.Domain.Comments;
using KHtmx.Domain.Comments.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace KHtmx.Persistence.Interceptors;

// TODO: Replace with Outbox pattern
internal sealed class CommentsSavingChangesInterceptor(
    IMediator mediator,
    ILogger<CommentsSavingChangesInterceptor> logger
) : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("SaveChangesAsync: {Context}", eventData.Context?.GetType().FullName ?? "null");
        if (eventData.Context is not { } context)
        {
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        var entries = context.ChangeTracker.Entries<Comment>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();
        
        logger.LogInformation("Found {Count} entries", entries.Count);

        foreach (var entry in entries)
        {
            INotification @event = entry.State switch
            {
                EntityState.Added => new CommentAddedEvent(entry.Entity),
                EntityState.Modified => new CommentUpdatedEvent(entry.Entity),
                EntityState.Deleted => new CommentDeletedEvent(entry.Entity),
                _ => throw new ArgumentOutOfRangeException(),
            };
            logger.LogInformation("Publishing event {Event}", @event);

            await mediator.Publish(@event, cancellationToken);
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
