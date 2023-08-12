using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace IdentityTesting.API.Models;

/// <summary>
///     A model representing an error returned by the API.
/// </summary>
[PublicAPI("API model")]
public sealed record class ApiError
{
    /// <summary>
    ///     A machine-readable error code.
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    ///     A human-readable error message.
    /// </summary>
    public required string Detail { get; init; }

    /// <summary>
    ///     A dictionary containing additional information about the error.
    /// </summary>
    /// <example>
    ///     <code>
    /// new ApiError
    /// {
    ///      Code = "UserNotFound",
    ///      Detail = "User not found",
    ///      Extensions = {["user"] = "user"}
    /// }
    /// will result in the following JSON:
    /// {
    ///     "code": "UserNotFound",
    ///     "detail": "User not found",
    ///     "user": "user"
    /// }
    /// </code>
    /// </example>
    [JsonExtensionData]
    public IDictionary<string, object?>? Extensions { get; set; }
}
