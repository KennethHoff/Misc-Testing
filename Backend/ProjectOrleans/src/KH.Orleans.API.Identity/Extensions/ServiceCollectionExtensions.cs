using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KH.Orleans.API.Identity.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKhIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication();
        services.AddAuthorization();
        services.AddDbContext<KhDbContext>(contextOptions =>
        {
            contextOptions.UseNpgsql(configuration.GetConnectionString("Npgsql"), npgsqlOptions =>
            {
                npgsqlOptions.MigrationsHistoryTable("__IdentityMigrationsHistory");
            });
        });

        services.AddSingleton<IStartupFilter, MigrationStartupFilter>();
        // services.AddScoped<IUserClaimsPrincipalFactory<KhApplicationUser>, KhUserClaimsPrincipalFactory>();

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

file sealed class MigrationStartupFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        => app =>
        {
            using var scope = app.ApplicationServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<KhDbContext>();
            dbContext.Database.Migrate();
            next(app);
        };
}
