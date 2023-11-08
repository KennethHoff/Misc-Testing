namespace KHtmx.Components.Comments.Data;

public sealed record class CommentTableDto
{
    public required Guid Id { get; init; }
    public required string Text { get; init; }
    public required string AuthorFirstName { get; init; }
    public required string AuthorLastName { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
}
