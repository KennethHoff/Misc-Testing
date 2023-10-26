using KH.Htmx.Domain.Shared;

namespace KH.Htmx.Domain.People;

public readonly record struct PersonId(Guid Value) : ITypedId<PersonId>
{
    public static readonly PersonId Admin = new(Guid.Parse("00000000-0000-0000-0000-000000000001"));
    public static PersonId New() => new(Guid.NewGuid());
    public static PersonId From(Guid value) => new(value);
}
