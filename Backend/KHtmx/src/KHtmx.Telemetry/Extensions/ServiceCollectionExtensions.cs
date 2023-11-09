using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace KHtmx.Telemetry.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKHtmxTelemetry(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .UseAzureMonitor();

        return services;
    }
}
