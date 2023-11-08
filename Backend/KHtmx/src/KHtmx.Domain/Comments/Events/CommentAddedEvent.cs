using MediatR;

namespace KHtmx.Domain.Comments.Events;

public sealed record class CommentAddedEvent(Comment Comment) : INotification;
