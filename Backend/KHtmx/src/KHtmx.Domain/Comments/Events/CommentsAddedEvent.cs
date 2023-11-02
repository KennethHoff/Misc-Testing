using MediatR;

namespace KHtmx.Domain.Comments.Events;

public sealed record class CommentsAddedEvent(IEnumerable<Comment> Comments) : INotification;
