namespace KH.Htmx.Constants;

public static class ServerSentEventNames
{
    /// <summary>
    /// The endpoint for server-sent events.
    /// </summary>
    public const string SseEndpoint = "/sse";
    
    /// <summary>
    /// The name of the event that is sent when a comment is added.
    /// </summary>
    public const string CommentAdded = "comment-added";
}
