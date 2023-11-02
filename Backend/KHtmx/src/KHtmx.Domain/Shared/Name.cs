namespace KHtmx.Domain.Shared;

public readonly record struct Name(string First, string Last)
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

    public override string ToString()
    {
        return $"{First} {Last}";
    }
}
