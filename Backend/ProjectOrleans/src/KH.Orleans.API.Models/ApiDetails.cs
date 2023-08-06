namespace KH.Orleans.API.Models;

public sealed record class ApiDetails
{
    public required string Title { get; init; }
    public required string Detail { get; init; }
}
