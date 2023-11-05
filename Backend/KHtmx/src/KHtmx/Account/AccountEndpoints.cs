using KHtmx.Components.Account;
using KHtmx.Constants;
using Microsoft.AspNetCore.Http.HttpResults;

namespace KHtmx.Account;

public static class AccountEndpoints
{
    public static void MapAccount(this IEndpointRouteBuilder route)
    {
        var htmxGroup = route.MapGroup(EndpointConstants.HtmxPrefix);

        route.MapPost("login", Login.Handler)
            .WithName(Login.EndpointName);

        route.MapPost("register", Register.Handler)
            .WithName(Register.EndpointName);

        htmxGroup.MapGet("loginDialog", GetLoginDialog.Handler)
            .WithName(GetLoginDialog.EndpointName);

        htmxGroup.MapGet("registerDialog", GetRegisterDialog.Handler)
            .WithName(GetRegisterDialog.EndpointName);
    }

    public static class GetLoginDialog
    {
        public const string EndpointName = "GetLoginDialog";

        public static RazorComponentResult<LoginDialogComponent> Handler
        (
            CancellationToken ct
        )
        {
            return new RazorComponentResult<LoginDialogComponent>();
        }
    }

    public static class GetRegisterDialog
    {
        public const string EndpointName = "GetRegisterDialog";

        public static RazorComponentResult<RegisterDialogComponent> Handler
        (
            CancellationToken ct
        )
        {
            return new RazorComponentResult<RegisterDialogComponent>();
        }
    }

    public class Login
    {
        public const string EndpointName = "Login";

        // TODO: Implement login
        public static Ok Handler
        (
            ILogger<Login> logger,
            CancellationToken ct
        )
        {
            logger.LogInformation("Login successful");
            return TypedResults.Ok();
        }
    }

    public class Register
    {
        public const string EndpointName = "Register";

        // TODO: Implement register
        public static Ok Handler
        (
            ILogger<Register> logger,
            CancellationToken ct
        )
        {
            logger.LogInformation("Register successful");
            return TypedResults.Ok();
        }
    }
}
