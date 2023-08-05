using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KH.Orleans.Identity.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKhIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication();
        services.AddAuthorization();
        services.AddDbContext<KhDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("Npgsql"));
        });
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<KhDbContext>()
            .AddDefaultTokenProviders();
        
        services.AddIdentityApiEndpoints<ApplicationUser>(opt => { });
        return services;
    }
}
