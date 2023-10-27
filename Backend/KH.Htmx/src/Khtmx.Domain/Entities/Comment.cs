using Khtmx.Domain.Primitives;
using Khtmx.Domain.Shared;

namespace Khtmx.Domain.Entities;

public sealed class Comment : Entity<Comment, CommentId>
{
    public string Text { get; }
    public DateTimeOffset Timestamp { get; }
    public PersonId AuthorId { get; }

    private Comment(CommentId id, PersonId authorId, string text, DateTimeOffset timestamp) : base(id)
    {
        AuthorId = authorId;
        Text = text;
        Timestamp = timestamp;
    }

    public static Result<Comment> Create(CommentId id, PersonId authorId, string text, DateTimeOffset timestamp)
    {
        return new Comment(id, authorId, text, timestamp);
    }
}

public readonly record struct CommentId(Guid Value) : ITypedId<CommentId>
{
    public static CommentId New() => new(Guid.NewGuid());
    public static CommentId From(Guid value) => new(value);
}
