using Khtmx.Domain.Entities;
using Khtmx.Domain.Primitives;
using Khtmx.Persistence.Interceptors;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Khtmx.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKhData(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ConvertDomainEventsToOutboxMessagesInterceptor>();
        services.AddDbContextFactory<KhDbContext>((serviceProvider, opt) =>
        {
            opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            opt.AddInterceptors(serviceProvider.GetRequiredService<ConvertDomainEventsToOutboxMessagesInterceptor>());
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
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        if (dbContext.People.Any(x => x.Id == PersonId.Admin))
        {
            return app;
        }

        var personEntity = Person.Create(PersonId.Admin, new Name("Admin", "Admin"));
        dbContext.People.Add(personEntity);
        dbContext.SaveChanges();

        return app;
    }
}
