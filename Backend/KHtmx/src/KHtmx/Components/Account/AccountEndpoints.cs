using KHtmx.Components.Account.Data;
using KHtmx.Constants;
using KHtmx.Domain.People;
using KHtmx.Extensions;
using KHtmx.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KHtmx.Components.Account;

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

    public sealed class GetLoginDialog
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

    public sealed class GetRegisterDialog
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

    public sealed class Login
    {
        public const string EndpointName = "Login";

        public static async ValueTask<Results<RazorComponentResult<LoginDialogComponent>, Ok>> Handler
        (
            ILogger<Login> logger,
            SignInManager<KhtmxUser> signInManager,
            [FromForm] LoginFormDto form,
            HttpResponse response,
            CancellationToken ct
        )
        {
            var result = await signInManager.PasswordSignInAsync(form.Username, form.Password, form.RememberMe, false);
            if (!result.Succeeded)
            {
                logger.LogInformation("Failed to login user: {@Errors}", result);
                return new RazorComponentResult<LoginDialogComponent>(new
                {
                    ValidationFailures = new ValidationFailureCollection
                    {
                        new()
                        {
                            ErrorMessage = "Incorrect username or password",
                        }
                    },
                    FormData = form,
                });
            }

            logger.LogInformation("User logged in: {@User}", form.Username);
            response.Headers.AddHtmxHeader(new HtmxRefreshHeader());
            return TypedResults.Ok();
        }
    }

    public sealed class Register
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

            if (await userManager.FindByEmailAsync(form.Email) is not null)
            {
                logger.LogInformation("Failed to create user: {@Errors}", "Email already in use");
                return new RazorComponentResult<RegisterDialogComponent>(new
                {
                    ValidationFailures = new ValidationFailureCollection
                    {
                        new()
                        {
                            PropertyName = nameof(form.Email),
                            ErrorMessage = "Email already in use",
                        }
                    },
                    FormData = form,
                });
            }

            var result = await userManager.CreateAsync(user, form.Password);
            if (!result.Succeeded)
            {
                logger.LogInformation("Failed to create user: {@Errors}", result.Errors);
                return new RazorComponentResult<RegisterDialogComponent>(new
                {
                    ValidationFailures = result.Errors.ToValidationFailures(),
                    FormData = form,
                });
            }

            return new RazorComponentResult<RegisterDialogComponent>();
        }
    }

    public sealed class Logout
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
