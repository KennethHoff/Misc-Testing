using KHtmx.Persistence;
using KHtmx.Domain.Comments;
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

                var admin = await dbContext.Users
                    .SingleOrDefaultAsync(x => x.UserName == "admin", stoppingToken);

                if (admin is null)
                {
                    throw new InvalidOperationException("Admin not found.");
                }

                var newComment = Comment.Create("Hello from the server! " + DateTimeOffset.UtcNow.ToString(""), DateTimeOffset.UtcNow, admin.Id);

                await dbContext.Comments.AddAsync(newComment, stoppingToken);
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
