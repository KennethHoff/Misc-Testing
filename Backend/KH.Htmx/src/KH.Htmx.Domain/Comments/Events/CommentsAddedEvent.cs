using MediatR;

namespace KH.Htmx.Domain.Comments.Events;

public sealed record class CommentsAddedEvent(IEnumerable<Comment> Comments) : INotification;
