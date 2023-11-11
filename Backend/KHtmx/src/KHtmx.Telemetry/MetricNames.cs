namespace KHtmx.Telemetry;

public static class MetricNames
{
    // TODO: Figure out what the point of the meter name is.
    /// <summary>
    /// Name of the meter for KHtmx
    /// </summary>
    public const string MeterName = "khtmx";

    private const string Prefix = "khtmx_";
    public const string CommentEditFailedValidation = Prefix + "comment_edit_failed_validation";
    public const string CommentDeleteFailedNotFound = Prefix + "comment_delete_failed_not_found";
}
