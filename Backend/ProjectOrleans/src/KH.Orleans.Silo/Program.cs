using Microsoft.Extensions.Hosting;
using Serilog;

var builder = Host.CreateDefaultBuilder();
builder.UseSerilog((_, _, loggerConfiguration) =>
{
    loggerConfiguration.WriteTo.Console();
});

builder.UseOrleans(siloBuilder =>
{
    siloBuilder.UseLocalhostClustering();

    siloBuilder.UseDashboard();
});

var app = builder.Build();

app.Run();
