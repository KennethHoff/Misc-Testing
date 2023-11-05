namespace KHtmx.Comments;

public sealed record class CommentDialogDto
{
    public required Guid Id { get; init; }
    public required Guid AuthorId { get; init; }
    public required string Text { get; init; }
    public required string AuthorFirstName { get; init; }
    public required string AuthorLastName { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
}
