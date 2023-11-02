using KHtmx.Domain.Primitives;
using KHtmx.Domain.Shared;

namespace KHtmx.Domain.Comments;

public readonly record struct CommentId(Guid Value) : ITypedId<CommentId>
{
    public static CommentId New() => new(Guid.NewGuid());
    public static CommentId From(Guid value) => new(value);
}
