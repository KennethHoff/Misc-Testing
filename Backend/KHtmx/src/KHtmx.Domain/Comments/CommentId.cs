using KHtmx.Domain.Primitives;

namespace KHtmx.Domain.Comments;

public readonly record struct CommentId(Guid Value) : ITypedId<CommentId>
{
    public static CommentId New() => new(Guid.NewGuid());
    public static CommentId From(Guid value) => new(value);

    public override string ToString()
    {
        return Value.ToString();
    }

    public static CommentId Parse(string s, IFormatProvider? provider)
    {
        return TryParse(s, provider, out var result)
            ? result
            : throw new FormatException($"The value '{s}' is not a valid comment id.");
    }

    public static bool TryParse(string? s, IFormatProvider? provider, out CommentId result)
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
