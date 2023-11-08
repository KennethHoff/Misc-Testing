using KHtmx.Constants;
using KHtmx.Domain.Comments.Events;
using Lib.AspNetCore.ServerSentEvents;
using MediatR;

namespace KHtmx.Components.Comments.Data;

public sealed class CommentsAddedNotificationHandler(
    IServerSentEventsService serverSentEventsService
) : INotificationHandler<CommentAddedEvent>
{
    public Task Handle(CommentAddedEvent notification, CancellationToken cancellationToken)
    {
        return serverSentEventsService.SendEventAsync(new ServerSentEvent
        {
            Id = ServerSentEventNames.CommentAdded,
            Type = ServerSentEventNames.CommentAdded,
            Data = ["Literally anything"],
        }, cancellationToken);
    }
}

public sealed class CommentUpdatedNotificationHandler(
    IServerSentEventsService serverSentEventsService
) : INotificationHandler<CommentUpdatedEvent>
{
    public Task Handle(CommentUpdatedEvent notification, CancellationToken cancellationToken)
    {
        return serverSentEventsService.SendEventAsync(new ServerSentEvent
        {
            Id = ServerSentEventNames.CommentUpdated,
            Type = ServerSentEventNames.CommentUpdated,
            Data = ["Literally anything"],
        }, cancellationToken);
    }
}

public sealed class CommentDeletedNotificationHandler(
    IServerSentEventsService serverSentEventsService
) : INotificationHandler<CommentDeletedEvent>
{
    public Task Handle(CommentDeletedEvent notification, CancellationToken cancellationToken)
    {
        return serverSentEventsService.SendEventAsync(new ServerSentEvent
        {
            Id = ServerSentEventNames.CommentDeleted,
            Type = ServerSentEventNames.CommentDeleted,
            Data = ["Literally anything"],
        }, cancellationToken);
    }
}
