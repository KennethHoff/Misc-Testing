using KHtmx.Domain.Comments;
using KHtmx.Domain.Shared;

namespace KHtmx.Domain.People;

public sealed record class Person
{
    public static readonly Person Admin = new()
    {
        Id = PersonId.Admin,
        Name = Name.Admin,
    };

    public PersonId Id { get; init; } = PersonId.New();
    public required Name Name { get; init; }

    public List<Comment> Comments { get; init; } = [];
}
