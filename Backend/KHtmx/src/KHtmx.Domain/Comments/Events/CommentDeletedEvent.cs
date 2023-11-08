using MediatR;

namespace KHtmx.Domain.Comments.Events;

public sealed record class CommentDeletedEvent(Comment Comment) : INotification;
