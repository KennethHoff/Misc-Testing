using KH.Htmx.Domain.Comments.Events;
using KH.Htmx.Domain.People;
using KH.Htmx.Domain.Primitives;

namespace KH.Htmx.Domain.Comments;

public sealed class Comment : AggregateRoot
{
    public CommentId Id { get; private init; } = CommentId.New();

    public required string Text { get; init; }
    public required DateTimeOffset Timestamp { get; init; }

    public required Person Author { get; init; }

    public Comment()
    {
        RaiseDomainEvent(new CommentCreatedDomainEvent(Id));
    }
}
