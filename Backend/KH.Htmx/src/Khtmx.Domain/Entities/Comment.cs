// using Khtmx.Domain.Comments.Events;

using Khtmx.Domain.Primitives;

namespace Khtmx.Domain.Entities;

public sealed class Comment(CommentId id) : Entity<Comment, CommentId>(id)
{
    public required string Text { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public required PersonId AuthorId { get; init; }
}

public readonly record struct CommentId(Guid Value) : ITypedId<CommentId>
{
    public static CommentId New() => new(Guid.NewGuid());
    public static CommentId From(Guid value) => new(value);
}
