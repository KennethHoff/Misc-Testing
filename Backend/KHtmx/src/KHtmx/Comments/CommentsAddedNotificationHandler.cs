using KHtmx.Constants;
using KHtmx.Domain.Comments.Events;
using Lib.AspNetCore.ServerSentEvents;
using MediatR;

namespace KHtmx.Comments;

public sealed class CommentsAddedNotificationHandler(
    IServerSentEventsService serverSentEventsService
) : INotificationHandler<CommentsAddedEvent>
{
    public Task Handle(CommentsAddedEvent notification, CancellationToken cancellationToken)
    {
        return serverSentEventsService.SendEventAsync(new ServerSentEvent
        {
            Id = ServerSentEventNames.CommentAdded,
            Type = ServerSentEventNames.CommentAdded,
            Data = ["Literally anything"],
        }, cancellationToken);
    }
}
