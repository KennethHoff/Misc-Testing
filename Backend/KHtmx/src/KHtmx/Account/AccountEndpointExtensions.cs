using KHtmx.Components.Account;
using KHtmx.Constants;
using Microsoft.AspNetCore.Http.HttpResults;

namespace KHtmx.Account;

public static class AccountEndpointExtensions
{
    public const string GetLoginEndpointName = "GetLogin";

    public static void MapAccount(this IEndpointRouteBuilder route)
    {
        var htmxGroup = route.MapGroup(EndpointConstants.HtmxPrefix);

        htmxGroup.MapGet(GetLoginEndpointName, GetLoginEndpointHandler)
            .WithName(GetLoginEndpointName);
    }


    private static RazorComponentResult<LoginForm> GetLoginEndpointHandler
    (
        CancellationToken ct
    )
    {
        return new RazorComponentResult<LoginForm>();
    }
}
