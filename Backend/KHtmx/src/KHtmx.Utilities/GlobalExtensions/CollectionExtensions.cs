namespace KHtmx.Utilities.GlobalExtensions;

/// <summary>
/// Collection Expressions to the rescue! Creating the most optimal collection type for the job.
/// </summary>
public static class CollectionExtensions
{
    public static IReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> source) => [..source];
}
