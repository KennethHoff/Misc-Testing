using KH.Htmx.Comments;

namespace KH.Htmx.HostedServices;

public sealed class AdminCommentSpamEventWorker(ICommentService commentService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                commentService.AddComment(new Comment
                {
                    Author = Person.Admin,
                    Text = "Hello from the server!",
                    Timestamp = DateTimeOffset.UtcNow,
                });
                await Task.Delay(TimeSpan.FromMilliseconds(1000), stoppingToken);
            }
        }
        catch (TaskCanceledException)
        {
            // ignore
        }
    }
}
