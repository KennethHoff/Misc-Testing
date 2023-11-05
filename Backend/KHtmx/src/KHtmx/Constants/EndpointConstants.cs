namespace KHtmx.Constants;

public static class EndpointConstants
{
    public const string HtmxPrefix = "/_htmx";

    public const string CommentEndpoint = "comments";

    public static string CreateHtmxEndpoint(string endpoint)
    {
        return $"{HtmxPrefix}/{endpoint}";
    }

    public static string CreateHtmxEndpoint<T1>(string endpoint, T1 arg1)
    {
        return $"{HtmxPrefix}/{endpoint}/{arg1}";
    }

    public static string CreateHtmxEndpoint<T1, T2>(string endpoint, T1 arg1, T2 arg2)
    {
        return $"{HtmxPrefix}/{endpoint}/{arg1}/{arg2}";
    }
}
