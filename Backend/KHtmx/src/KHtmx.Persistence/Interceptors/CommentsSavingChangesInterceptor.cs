using KHtmx.Domain.Comments;
using KHtmx.Domain.Comments.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace KHtmx.Persistence.Interceptors;

// TODO: Replace with Outbox pattern
internal sealed class CommentsSavingChangesInterceptor(
    IMediator mediator
) : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not { } context)
        {
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        var entries = context.ChangeTracker.Entries<Comment>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();

        foreach (var entry in entries)
        {
            INotification @event = entry.State switch
            {
                EntityState.Added => new CommentAddedEvent(entry.Entity),
                EntityState.Modified => new CommentUpdatedEvent(entry.Entity),
                EntityState.Deleted => new CommentDeletedEvent(entry.Entity),
                _ => throw new ArgumentOutOfRangeException(),
            };
            await mediator.Publish(@event, cancellationToken);
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
