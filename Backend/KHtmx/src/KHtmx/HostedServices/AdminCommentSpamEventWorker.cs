using Bogus;
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

                var numberOfComments = Random.Shared.Next(1, 4);

                var comments = Enumerable.Range(0, numberOfComments)
                    .Select(_ => Comment.Create(GetRandomString(2, 20), DateTimeOffset.UtcNow, admin.Id))
                    .ToReadOnlyList();

                await dbContext.Comments.AddRangeAsync(comments, stoppingToken);
                await dbContext.SaveChangesAsync(stoppingToken);

                await Task.Delay(TimeSpan.FromMilliseconds(250), stoppingToken);
            }
        }
        catch (TaskCanceledException)
        {
            // ignore
        }
    }

    private static string GetRandomString(int minLength, int maxLength)
    {
        var faker = new Faker();

        return faker.Lorem.Sentence(Random.Shared.Next(minLength, maxLength));
    }
}
