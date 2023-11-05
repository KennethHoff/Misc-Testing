using KHtmx.Domain.People;
using KHtmx.Persistence.Interceptors;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KHtmx.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKhData(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<CommentsSavingChangesInterceptor>();
        services.AddDbContextFactory<KhDbContext>((serviceProvider, opt) =>
        {
            opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            opt.AddInterceptors(serviceProvider.GetRequiredService<CommentsSavingChangesInterceptor>());
        });

        return services;
    }
}

public static class WebApplicationExtensions
{
    public static WebApplication UseKhData(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<KhDbContext>>();
        using var dbContext = dbContextFactory.CreateDbContext();
        // dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        if (dbContext.Users.Any(x => x.UserName == "admin"))
        {
            return app;
        }

        var adminUser = User.Create("Admin", "Admin", "admin", "admin@betweennames.dev");

        dbContext.Users.Add(adminUser);
        dbContext.SaveChanges();

        return app;
    }
}
