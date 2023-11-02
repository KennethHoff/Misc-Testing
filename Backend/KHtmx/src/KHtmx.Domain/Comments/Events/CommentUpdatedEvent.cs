using MediatR;

namespace KHtmx.Domain.Comments.Events;

public sealed record class CommentUpdatedEvent(Comment Comments) : INotification;
