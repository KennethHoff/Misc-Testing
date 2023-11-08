using KHtmx.Constants;
using KHtmx.Domain.Comments.Events;
using Lib.AspNetCore.ServerSentEvents;
using MediatR;

namespace KHtmx.Components.Comments.Data;

public sealed class CommentsAddedNotificationHandler(
    IServerSentEventsService serverSentEventsService
) : INotificationHandler<CommentAddedEvent>,
    INotificationHandler<CommentUpdatedEvent>,
    INotificationHandler<CommentDeletedEvent>
{
    public Task Handle(CommentAddedEvent notification, CancellationToken cancellationToken)
    {
        return SendEventAsync(cancellationToken);
    }

    public Task Handle(CommentUpdatedEvent notification, CancellationToken cancellationToken)
    {
        return SendEventAsync(cancellationToken);
    }

    public Task Handle(CommentDeletedEvent notification, CancellationToken cancellationToken)
    {
        return SendEventAsync(cancellationToken);
    }

    private async Task SendEventAsync(CancellationToken cancellationToken)
    {
        await serverSentEventsService.SendEventAsync(new ServerSentEvent
        {
            Id = ServerSentEventNames.CommentTableUpdated,
            Type = ServerSentEventNames.CommentTableUpdated,
            Data = ["."],
        }, cancellationToken);
    }
}
