using System.Diagnostics;
using System.Security.Claims;
using KH.Orleans.API.Identity.Extensions;
using KH.Orleans.GrainInterfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Serilog;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Host.UseSerilog((_, _, loggerConfiguration) =>
{
    loggerConfiguration.WriteTo.Console();
});

builder.Host.UseOrleansClient(clientBuilder =>
{
    clientBuilder.UseLocalhostClustering();
});

builder.Services.AddKhIdentity(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument();

var app = builder.Build();

app.UseKhIdentity();

app.UseOpenApi();
app.UseSwaggerUi3(opt =>
{
    // Makes TryItOut the default... Why is this not the default?
    opt.AdditionalSettings["tryItOutEnabled"] = true;

    // Makes all top-level folders expanded by default
    opt.DocExpansion = "list";

    // Do not expand the Schema section -- Clutters the UI
    opt.DefaultModelsExpandDepth = 0;
});

app.MapGet("/Greeting",
        async ValueTask<Ok<string>> (IGrainFactory grainFactory, ClaimsPrincipal claimsPrincipal) =>
        {
            Debug.Assert(claimsPrincipal.Identity is not null);

            var helloGrain = grainFactory.GetGrain<IGreetingGrain>(claimsPrincipal.Identity.Name);

            return TypedResults.Ok(await helloGrain.Greet());
        })
    .WithName("Greeting")
    .RequireAuthorization();

app.Run();
