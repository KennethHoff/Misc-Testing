using KH.Htmx.Constants;
using KH.Htmx.Domain.Comments.Events;
using Lib.AspNetCore.ServerSentEvents;
using MediatR;

namespace KH.Htmx.Comments;

public sealed class CommentsAddedNotificationHandler(
        IServerSentEventsService serverSentEventsService
    )
    : INotificationHandler<CommentCreatedDomainEvent>
{
    public Task Handle(CommentCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        return serverSentEventsService.SendEventAsync(new ServerSentEvent
        {
            Id = ServerSentEventNames.CommentAdded,
            Type = ServerSentEventNames.CommentAdded,
            Data = ["Literally anything"],
        }, cancellationToken);
    }
}
