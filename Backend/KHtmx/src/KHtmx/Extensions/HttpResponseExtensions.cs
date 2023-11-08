namespace KHtmx.Extensions;

public static class HttpResponseExtensions
{
    public static IHeaderDictionary AddHtmxHeader(this IHeaderDictionary headerDictionary, IHtmxHeader htmxHeader)
    {
        htmxHeader.ApplyTo(headerDictionary);
        return headerDictionary;
    }
}
/// <summary>
/// See the <a href="https://htmx.org/docs/#response-headers">HTMX documentation</a> for all the available headers and their meanings.
/// </summary>
public interface IHtmxHeader
{
    public void ApplyTo(IHeaderDictionary headerDictionary);
}

/// <summary>
/// Apply this header to the response to trigger a refresh of the page after the request completes.
/// </summary>
public readonly record struct HtmxRefreshHeader : IHtmxHeader
{
    public void ApplyTo(IHeaderDictionary headerDictionary)
    {
        headerDictionary["HX-Refresh"] = "true";
    }
}
