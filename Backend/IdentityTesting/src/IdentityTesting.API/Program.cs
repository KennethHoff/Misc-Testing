using IdentityTesting.API.Identity.Extensions;
using Serilog;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Host.UseSerilog((_, _, loggerConfiguration) =>
{
    loggerConfiguration.WriteTo.Console();
});
builder.Services.AddResponseCompression();

builder.Services.AddKhIdentity(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(opt =>
{
    opt.Title = "IdentityTesting API";
});

var app = builder.Build();
app.UseResponseCompression();

app.UseKhIdentity();
app.MapKhIdentity();

app.UseOpenApi();
app.UseSwaggerUi3(opt =>
{
    // Makes TryItOut the default... Why is this not the default?.. Or at least a standard option?
    opt.AdditionalSettings["tryItOutEnabled"] = true;

    // // Makes all top-level folders expanded by default
    // opt.DocExpansion = "list";

    // Do not expand the Schema section -- Clutters the UI
    opt.DefaultModelsExpandDepth = 0;
});

app.Run();
