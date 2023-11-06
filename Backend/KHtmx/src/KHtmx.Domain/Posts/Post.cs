using System.ComponentModel.DataAnnotations.Schema;
using KHtmx.Domain.People;

namespace KHtmx.Domain.Posts;

public sealed class Post
{
    public Guid Id { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public string Title { get; init; } = null!;
    public string Content { get; init; } = null!;

    public Guid AuthorId { get; init; }
    [ForeignKey(nameof(AuthorId))] public KhtmxUser Author { get; init; } = null!;

    public static Post Create(string text, DateTimeOffset createdAt, Guid authorId)
    {
        return new Post
        {
            Id = Guid.NewGuid(),
            Content = text,
            CreatedAt = createdAt,
            AuthorId = authorId,
        };
    }
}
