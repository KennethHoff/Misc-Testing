using FluentValidation;
using KHtmx.Components;
using KHtmx.Components.Account;
using KHtmx.Components.Comments;
using KHtmx.Constants;
using KHtmx.Domain.People;
using KHtmx.HostedServices;
using KHtmx.Persistence;
using KHtmx.Persistence.Extensions;
using KHtmx.Telemetry.Extensions;
using Lib.AspNetCore.ServerSentEvents;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Services.AddKhData(builder.Configuration);
builder.Services.AddIdentity<KhtmxUser, KhtmxRole>()
    .AddEntityFrameworkStores<KhDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddComments();
builder.Services.AddKHtmxTelemetry(builder.Configuration);

builder.Services.AddRazorComponents();

builder.Services.AddServerSentEvents();
builder.Services.AddHostedService<AdminCommentSpamEventWorker>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>(includeInternalTypes: true);
builder.Services.AddMediatR(opt =>
{
    opt.RegisterServicesFromAssemblyContaining<Program>();
});

var app = builder.Build();
app.UseKhData();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapComments();
app.MapAccount();
app.MapServerSentEvents(ServerSentEventNames.SseEndpoint);

app.MapRazorComponents<App>();

app.Run();
