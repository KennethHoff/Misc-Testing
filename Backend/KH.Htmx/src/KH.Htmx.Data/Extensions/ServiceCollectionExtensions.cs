using KH.Htmx.Data.Interceptors;
using KH.Htmx.Domain.People;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KH.Htmx.Data.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKhData(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<CommentAddedToDatabaseInterceptor>();
        services.AddDbContextFactory<KhDbContext>((serviceProvider, opt) =>
        {
            opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            opt.AddInterceptors(serviceProvider.GetRequiredService<CommentAddedToDatabaseInterceptor>());
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
        dbContext.Database.EnsureCreated();

        if (dbContext.People.Contains(Person.Admin))
        {
            return app;
        }

        var personEntity = Person.Admin;
        dbContext.People.Add(personEntity);
        dbContext.SaveChanges();

        return app;
    }
}
