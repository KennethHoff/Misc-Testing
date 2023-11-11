using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KHtmx.Telemetry.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKHtmxTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenTelemetry()
            .WithMetrics(metrics => metrics
                .AddMeter(MetricNames.CommentEditFailedValidation, MetricNames.CommentDeleteFailedNotFound)
            )
            .UseAzureMonitor(options =>
            {
                // For some reason, if you use the overload with an Action<>, it stops using the default connection string
                options.ConnectionString = configuration["AzureMonitor:ConnectionString"];
            });
        return services;
    }
}
