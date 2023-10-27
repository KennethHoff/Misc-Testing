using Khtmx.Domain.Primitives;

namespace Khtmx.Domain.Entities;

public sealed class Person(PersonId id) : AggregateRoot<Person, PersonId>(id)
{
    public static readonly Person Admin = new(PersonId.Admin)
    {
        Name = Name.Admin,
    };

    public required Name Name { get; init; }

    public List<Comment> Comments { get; init; } = [];
}

public readonly record struct PersonId(Guid Value) : ITypedId<PersonId>
{
    public static readonly PersonId Admin = new(Guid.Parse("00000000-0000-0000-0000-000000000001"));
    public static PersonId New() => new(Guid.NewGuid());
    public static PersonId From(Guid value) => new(value);
}
