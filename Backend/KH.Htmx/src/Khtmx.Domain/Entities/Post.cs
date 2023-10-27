using Khtmx.Domain.Primitives;

namespace Khtmx.Domain.Entities;

public sealed class Post : AggregateRoot<Post, PostId>
{
    public PersonId AuthorId { get; }
    public string Title { get; }
    public string Content { get; }

    private Post(PostId id, PersonId authorId, string title, string content) : base(id)
    {
        AuthorId = authorId;
        Title = title;
        Content = content;
    }

    public static Post Create(PostId id, PersonId authorId, string title, string content)
    {
        return new Post(id, authorId, title, content);
    }
}

public readonly record struct PostId(Guid Value) : ITypedId<PostId>
{
    public static PostId New() => new(Guid.NewGuid());
    public static PostId From(Guid value) => new(value);
}
