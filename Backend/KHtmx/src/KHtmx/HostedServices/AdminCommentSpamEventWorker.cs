using KHtmx.Data;
using KHtmx.Domain.Comments;
using KHtmx.Domain.People;
using Microsoft.EntityFrameworkCore;

namespace KHtmx.HostedServices;

public sealed class AdminCommentSpamEventWorker(
    IDbContextFactory<KhDbContext> dbContextFactory
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(3_000), stoppingToken);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await using var dbContext = await dbContextFactory.CreateDbContextAsync(stoppingToken);

                var admin = await dbContext.People
                    .SingleOrDefaultAsync(x => x.Id == PersonId.Admin, stoppingToken);

                if (admin is null)
                {
                    throw new InvalidOperationException("Admin not found.");
                }

                await dbContext.Comments
                    .AddAsync(new Comment
                    {
                        Author = admin,
                        Text = "Hello from the server! " + DateTimeOffset.UtcNow.ToString(""),
                        Timestamp = DateTimeOffset.UtcNow,
                    }, stoppingToken);
                
                await dbContext.SaveChangesAsync(stoppingToken);

                await Task.Delay(TimeSpan.FromMilliseconds(3_000), stoppingToken);
            }
        }
        catch (TaskCanceledException)
        {
            // ignore
        }
    }
}
