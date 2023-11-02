using KHtmx.Domain.Primitives;

namespace KHtmx.Domain.People;

public readonly record struct PersonId(Guid Value) : ITypedId<PersonId>
{
    public static readonly PersonId Admin = new(Guid.Parse("00000000-0000-0000-0000-000000000001"));
    public static PersonId New() => new(Guid.NewGuid());
    public static PersonId From(Guid value) => new(value);

    public override string ToString()
    {
        return Value.ToString();
    }

    public static PersonId Parse(string s, IFormatProvider? provider)
    {
        return TryParse(s, provider, out var result)
            ? result
            : throw new FormatException($"The value '{s}' is not a valid person id.");
    }

    public static bool TryParse(string? s, IFormatProvider? provider, out PersonId result)
    {
        if (Guid.TryParse(s, out var guid))
        {
            result = From(guid);
            return true;
        }

        result = default;
        return false;
    }
}
