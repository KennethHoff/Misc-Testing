using System.Collections.Immutable;
using Lib.AspNetCore.ServerSentEvents;

namespace KH.Htmx.HostedServices;

public sealed class ServerEventsWorker(IServerSentEventsService serverSentEventsService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var clients = serverSentEventsService.GetClients();
                if (clients.Count > 0)
                {
                    await serverSentEventsService.SendEventAsync(new ServerSentEvent
                    {
                        Id = "clock",
                        Type = "clock",
                        Data = ["lol"],
                    }, stoppingToken);
                }

                await Task.Delay(TimeSpan.FromMilliseconds(1000), stoppingToken);
            }
        }
        catch (TaskCanceledException)
        {
            // ignore
        }
    }
}
