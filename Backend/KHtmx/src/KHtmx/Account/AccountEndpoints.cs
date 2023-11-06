using KHtmx.Components.Account;
using KHtmx.Constants;
using KHtmx.Domain.People;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KHtmx.Account;

public static class AccountEndpoints
{
    public static void MapAccount(this IEndpointRouteBuilder route)
    {
        var htmxGroup = route.MapGroup(EndpointConstants.HtmxPrefix);

        htmxGroup.MapPost("login", Login.Handler)
            .WithName(Login.EndpointName);

        htmxGroup.MapPost("register", Register.Handler)
            .WithName(Register.EndpointName);

        htmxGroup.MapGet("logout", Logout.Handler)
            .WithName(Logout.EndpointName);

        htmxGroup.MapGet("loginDialog", GetLoginDialog.Handler)
            .WithName(GetLoginDialog.EndpointName);

        htmxGroup.MapGet("registerDialog", GetRegisterDialog.Handler)
            .WithName(GetRegisterDialog.EndpointName);
    }

    public class GetLoginDialog
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

    public class GetRegisterDialog
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
        public static async ValueTask<Results<RazorComponentResult<LoginDialogComponent>, Created>> Handler
        (
            ILogger<Login> logger,
            SignInManager<KhtmxUser> signInManager,
            [FromForm] LoginFormDto form,
            CancellationToken ct
        )
        {
            var result = await signInManager.PasswordSignInAsync(form.Username, form.Password, form.RememberMe, false);
            if (!result.Succeeded)
            {
                logger.LogInformation("Failed to login user: {@Errors}", result);
                return new RazorComponentResult<LoginDialogComponent>(new
                {
                    Errors = result.ToString(),
                    FormData = form
                });
            }

            logger.LogInformation("User logged in: {@User}", form.Username);
            return new RazorComponentResult<LoginDialogComponent>();
        }
    }

    public class Register
    {
        public const string EndpointName = "Register";

        // TODO: Implement register
        public static async ValueTask<Results<RazorComponentResult<RegisterDialogComponent>, Created>> Handler
        (
            ILogger<Register> logger,
            UserManager<KhtmxUser> userManager,
            [FromForm] RegisterFormDto form,
            CancellationToken ct
        )
        {
            var user = KhtmxUser.Create(form.Username, form.Email, form.FirstName, form.LastName);
            var result = await userManager.CreateAsync(user, form.Password);
            if (!result.Succeeded)
            {
                logger.LogInformation("Failed to create user: {@Errors}", result.Errors);
                return new RazorComponentResult<RegisterDialogComponent>(new
                {
                    Errors = result.Errors.ToArray(),
                    FormData = form
                });
            }

            return new RazorComponentResult<RegisterDialogComponent>();
        }
    }

    public class Logout
    {
        public const string EndpointName = "Logout";

        public static async ValueTask<Ok> Handler
        (
            ILogger<Logout> logger,
            SignInManager<KhtmxUser> signInManager,
            CancellationToken ct
        )
        {
            await signInManager.SignOutAsync();
            logger.LogInformation("User logged out");
            return TypedResults.Ok();
        }
    }
}
