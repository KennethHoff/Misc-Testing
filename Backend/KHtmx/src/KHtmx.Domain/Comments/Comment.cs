using KHtmx.Domain.People;

namespace KHtmx.Domain.Comments;

public sealed record class Comment
{
    public CommentId Id { get; private init; } = CommentId.New();

    public required string Text { get; set; }
    public required DateTimeOffset Timestamp { get; init; }
    
    public required Person Author { get; init; }
}
