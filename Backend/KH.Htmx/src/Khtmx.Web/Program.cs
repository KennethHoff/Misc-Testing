using Coravel;
using FluentValidation;
using Khtmx.Web.Components;
using Khtmx.Persistence.Extensions;
using Khtmx.Infrastructure.BackgroundJobs;
using Khtmx.Web.Comments;
using Khtmx.Web.Constants;
using Khtmx.Web.HostedServices;
using Lib.AspNetCore.ServerSentEvents;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScheduler();
builder.Services.AddTransient<ProcessOutboxMessagesInvocable>();
builder.Services.AddKhData(builder.Configuration);

builder.Services.AddComments();

// Add services to the container.
builder.Services.AddRazorComponents();

builder.Services.AddServerSentEvents();
builder.Services.AddHostedService<AdminCommentSpamEventWorker>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>(includeInternalTypes: true);
builder.Services.AddMediatR(opt =>
{
    opt.RegisterServicesFromAssemblyContaining<Program>();
});

var app = builder.Build();
app.Services.UseScheduler(scheduler =>
{
    scheduler
        .Schedule<ProcessOutboxMessagesInvocable>()
        .EverySecond();
});
app.UseKhData();

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapComments();
app.MapServerSentEvents(ServerSentEventNames.SseEndpoint);

app.MapRazorComponents<App>();

app.Run();
