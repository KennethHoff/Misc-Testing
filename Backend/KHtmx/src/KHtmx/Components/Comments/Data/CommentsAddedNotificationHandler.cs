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
    private static readonly TimeSpan MinTimeBetweenEvents = TimeSpan.FromMilliseconds(250);

    private static DateTimeOffset _lastEventTime = DateTimeOffset.MinValue;

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
        var now = TimeProvider.System.GetUtcNow();
        if (_lastEventTime + MinTimeBetweenEvents > now)
        {
            return;
        }

        _lastEventTime = now;
        await serverSentEventsService.SendEventAsync(new ServerSentEvent
        {
            Id = ServerSentEventNames.CommentTableUpdated,
            Type = ServerSentEventNames.CommentTableUpdated,
            Data = ["."],
        }, cancellationToken);
    }
}
