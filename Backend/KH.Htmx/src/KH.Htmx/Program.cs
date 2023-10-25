using FluentValidation;
using KH.Htmx.Comments;
using KH.Htmx.Components;
using KH.Htmx.Constants;
using KH.Htmx.HostedServices;
using Lib.AspNetCore.ServerSentEvents;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(serviceName: "KH.Htmx")
    )
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddMeter(MetricNames.CommentsAdded)
        .AddPrometheusExporter()
    );

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

builder.Services.AddMetrics();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapComments();
app.MapServerSentEvents(ServerSentEventNames.SseEndpoint);
app.MapPrometheusScrapingEndpoint();

app.MapRazorComponents<App>();

app.Run();
