using KH.Orleans.API.Identity;
using KH.Orleans.GrainInterfaces;
using KH.Orleans.API.Identity.Extensions;
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

app.MapGroup("/Identity").MapIdentityApi<KhApplicationUser>();

app.MapGet("/Greeting",
        async ValueTask<Results<Ok<string>, UnauthorizedHttpResult>> (IGrainFactory grainFactory,
            HttpContext context) =>
        {
            if (context.User.Identity is null or { IsAuthenticated: false } or { Name: null })
            {
                return TypedResults.Unauthorized();
            }

            var helloGrain = grainFactory.GetGrain<IHelloWorldGrain>(0);
            var greeting = await helloGrain.SayHello(context.User.Identity.Name).ConfigureAwait(false);

            return TypedResults.Ok(greeting);
        })
    .RequireAuthorization();


app.Run();
