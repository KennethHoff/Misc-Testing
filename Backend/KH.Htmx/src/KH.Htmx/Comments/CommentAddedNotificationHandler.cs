using KH.Htmx.Comments.Events;
using KH.Htmx.Constants;
using Lib.AspNetCore.ServerSentEvents;
using MediatR;

namespace KH.Htmx.Comments;

public sealed class CommentAddedNotificationHandler(
        IServerSentEventsService serverSentEventsService
    )
    : INotificationHandler<CommentAddedEvent>
{
    public async Task Handle(CommentAddedEvent notification, CancellationToken cancellationToken)
    {
        await serverSentEventsService.SendEventAsync(new ServerSentEvent
        {
            Id = ServerSentEventNames.CommentAdded,
            Type = ServerSentEventNames.CommentAdded,
            Data = ["Literally anything"],
        }, cancellationToken);
    }
}
