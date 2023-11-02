using FluentValidation;
using KHtmx.Comments;
using KHtmx.Components;
using KHtmx.Constants;
using KHtmx.Data.Extensions;
using KHtmx.HostedServices;
using Lib.AspNetCore.ServerSentEvents;

var builder = WebApplication.CreateBuilder(args);
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
app.UseKhData();

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapComments();
app.MapServerSentEvents(ServerSentEventNames.SseEndpoint);

app.MapRazorComponents<App>();

app.Run();
