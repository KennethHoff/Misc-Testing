using IdentityTesting.API.Identity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IdentityTesting.API.Identity.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKhIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<RoleService>();
        services.AddScoped<UserService>();
        services.AddAuthentication();
        services.AddAuthorizationBuilder();

        services.AddDbContext<KhDbContext>(contextOptions =>
        {
            contextOptions.UseNpgsql(configuration.GetConnectionString("Npgsql"), npgsqlOptions =>
            {
                npgsqlOptions.MigrationsHistoryTable("__IdentityMigrationsHistory");
            });
        });

        services.AddSingleton<IEmailSender, LoggingEmailSender>();

        services.AddIdentityApiEndpoints<KhApplicationUser>()
            .AddDefaultTokenProviders()
            .AddRoles<KhApplicationRole>()
            .AddUserManager<KhUserManager>()
            .AddRoleManager<KhRoleManager>()
            .AddEntityFrameworkStores<KhDbContext>()
            .AddSignInManager<KhSignInManager>();

        return services;
    }
}

file sealed class LoggingEmailSender(ILogger<LoggingEmailSender> logger) : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        logger.LogInformation("Sending email to {Email} with subject {Subject} and message {Message}", email, subject, htmlMessage);
        return Task.CompletedTask;
    }
}
