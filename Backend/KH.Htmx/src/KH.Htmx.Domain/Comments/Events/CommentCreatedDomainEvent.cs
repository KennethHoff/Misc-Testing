using KH.Htmx.Domain.Primitives;

namespace KH.Htmx.Domain.Comments.Events;

public sealed record class CommentCreatedDomainEvent(CommentId CommentId) : IDomainEvent;
