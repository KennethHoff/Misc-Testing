namespace KH.Htmx.Comments;

public readonly record struct CommentId(Guid Value)
{
    public static CommentId New() => new(Guid.NewGuid());
}

public readonly record struct Comment()
{
    public CommentId Id { get; private init; } = CommentId.New();
    public required string Text { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public required Person Author { get; init; }
}

public record class Person
{
    public static readonly Person Admin = new()
    {
        Name = Name.Admin,
    };

    public required Name Name { get; init; }
}

public readonly record struct Name
{
    public static readonly Name Anonymous = new()
    {
        First = "Anonymous",
        Last = "Anonymous",
    };

    public static readonly Name Admin = new()
    {
        First = "Admin",
        Last = "Admin",
    };

    public required string First { get; init; }
    public required string Last { get; init; }

    public override string ToString()
    {
        return $"{First} {Last}";
    }
}
