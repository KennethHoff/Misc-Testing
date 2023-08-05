using KH.Orleans.GrainInterfaces;
using KH.Orleans.Identity.Extensions;
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

app.UseAuthentication();
app.UseAuthorization();

app.UseOpenApi();
app.UseSwaggerUi3(opt =>
{
    // Makes TryItOut the default
    opt.AdditionalSettings["tryItOutEnabled"] = true;
    
    // Makes all top-level folders expanded by default
    opt.DocExpansion = "list";
});

var greetingGroup = app.MapGroup("/Greeting");
greetingGroup.MapGet("/1", async ValueTask<Results<Ok<string>, UnauthorizedHttpResult, NotFound>> (IGrainFactory grainFactory, string name, HttpContext context) =>
{
    var identity = context.User.Identity;
    
    if (!identity?.IsAuthenticated ?? false)
    {
        return TypedResults.Unauthorized();
    }
    
    if (string.IsNullOrWhiteSpace(name))
    {
        return TypedResults.NotFound();
    }
    
    var helloGrain = grainFactory.GetGrain<IHelloWorldGrain>(0);
    var greeting = await helloGrain.SayHello(name).ConfigureAwait(false);
    
    return TypedResults.Ok(greeting);
});


app.Run();
