using FluentValidation;
using KHtmx.Account;
using KHtmx.Comments;
using KHtmx.Components;
using KHtmx.Constants;
using KHtmx.Domain.People;
using KHtmx.Persistence;
using KHtmx.Persistence.Extensions;
using Lib.AspNetCore.ServerSentEvents;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddKhData(builder.Configuration);
builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<KhDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddComments();

// Add services to the container.
builder.Services.AddRazorComponents();

builder.Services.AddServerSentEvents();
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
