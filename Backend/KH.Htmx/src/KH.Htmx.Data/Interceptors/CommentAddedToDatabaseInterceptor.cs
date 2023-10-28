using KH.Htmx.Domain.Comments;
using KH.Htmx.Domain.Comments.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace KH.Htmx.Data.Interceptors;

internal sealed class CommentAddedToDatabaseInterceptor(IMediator mediator) : SaveChangesInterceptor
{
    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result,
        CancellationToken cancellationToken = new())
    {
        if (eventData.Context is not { } context)
        {
            return await base.SavedChangesAsync(eventData, result, cancellationToken);
        }
        
        var entries = context.ChangeTracker.Entries<Comment>()
            .Where(e => e.State is EntityState.Added)
            .ToList();

        await mediator.Publish(new CommentsAddedEvent(entries.Select(x => x.Entity)), cancellationToken);
        
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }
}
