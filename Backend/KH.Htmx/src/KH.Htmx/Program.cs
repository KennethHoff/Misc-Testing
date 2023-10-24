using KH.Htmx.Components;
using KH.Htmx.Components.Components;
using KH.Htmx.HostedServices;
using Lib.AspNetCore.ServerSentEvents;
using Microsoft.AspNetCore.Components.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents();

builder.Services.AddServerSentEvents();
builder.Services.AddHostedService<ServerEventsWorker>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapGet("/clock", () => new RazorComponentResult<Clock>());
app.MapServerSentEvents("/rn-updates");

app.MapRazorComponents<App>();

app.Run();
