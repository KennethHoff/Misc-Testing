using MediatR;

namespace KH.Htmx.Comments.Events;

public readonly record struct CommentAddedEvent(Comment Comment) : INotification;
