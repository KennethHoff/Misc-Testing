using KH.Htmx.Domain.Primitives;

namespace KH.Htmx.Domain.Comments;

public readonly record struct CommentId(Guid Value) : ITypedId<CommentId>
{
    public static CommentId New() => new(Guid.NewGuid());
    public static CommentId From(Guid value) => new(value);
}
