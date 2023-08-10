namespace IdentityTesting.API.Models;

/// <summary>
///  A model representing an error returned by the API.
/// </summary>
public sealed record class ApiDetails
{
    /// <summary>
    ///    A machine-readable error code.
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    ///   A human-readable error message.
    /// </summary>
    public required string Detail { get; init; }
}
