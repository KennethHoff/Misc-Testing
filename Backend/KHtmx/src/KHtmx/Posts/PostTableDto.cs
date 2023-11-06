namespace KHtmx.Posts;

public sealed record class PostTableDto
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required string AuthorFirstName { get; init; }
    public required string AuthorLastName { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}
