using KHtmx.Components.Account;
using KHtmx.Constants;
using Microsoft.AspNetCore.Http.HttpResults;

namespace KHtmx.Account;

public static class AccountEndpointExtensions
{
    public static void MapAccount(this IEndpointRouteBuilder route)
    {
        route.MapPost("login", Api.Login.Handler)
            .WithName(Api.Login.EndpointName);

        var htmxGroup = route.MapGroup(EndpointConstants.HtmxPrefix);

        htmxGroup.MapGet("login", Htmx.Login.Handler)
            .WithName(Htmx.Login.EndpointName);
    }

    public static class Htmx
    {
        public static class Login
        {
            public const string EndpointName = "Htmx_Login";

            public static RazorComponentResult<LoginDialogComponent> Handler
            (
                CancellationToken ct
            )
            {
                return new RazorComponentResult<LoginDialogComponent>();
            }
        }
    }

    private static class Api
    {
        public static class Login
        {
            public const string EndpointName = "Api_Login";

            // TODO: Implement login
            public static Ok Handler
            (
                CancellationToken ct
            )
            {
                return TypedResults.Ok();
            }
        }
    }
}
