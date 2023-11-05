using FluentValidation;
using KHtmx.Account;
using KHtmx.Comments;
using KHtmx.Components;
using KHtmx.Constants;
using KHtmx.Domain.People;
using KHtmx.HostedServices;
using KHtmx.Persistence;
using KHtmx.Persistence.Extensions;
using Lib.AspNetCore.ServerSentEvents;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddKhData(builder.Configuration);
builder.Services.AddIdentity<KhtmxUser, KhtmxRole>()
    .AddEntityFrameworkStores<KhDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddComments();

builder.Services.AddRazorComponents();

builder.Services.AddServerSentEvents();
// builder.Services.AddHostedService<AdminCommentSpamEventWorker>();
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
app.MapAccount();
app.MapServerSentEvents(ServerSentEventNames.SseEndpoint);

app.MapRazorComponents<App>();

app.Run();
