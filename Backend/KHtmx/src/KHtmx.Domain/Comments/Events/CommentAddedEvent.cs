using MediatR;

namespace KHtmx.Domain.Comments.Events;

public sealed record class CommentAddedEvent(Comment Comments) : INotification;
