using System.ComponentModel.DataAnnotations.Schema;
using KHtmx.Domain.People;

namespace KHtmx.Domain.Comments;

public sealed record class Comment
{
    // EF Core requires a parameterless constructor
    public Comment()
    { }

    private Comment(string text, DateTimeOffset timestamp, Guid authorId)
    {
        Text = text;
        Timestamp = timestamp;
        AuthorId = authorId;
    }

    public Guid Id { get; private init; } = Guid.NewGuid();

    public string Text { get; private set; } = null!;
    public DateTimeOffset Timestamp { get; private init; }
    public Guid AuthorId { get; private init; }

    [ForeignKey(nameof(AuthorId))]
    public User Author { get; } = null!;

    public static Comment Create(string text, DateTimeOffset timestamp, Guid authorId)
        => new(text, timestamp, authorId);

    public void ChangeText(string text)
        => Text = text;
}
