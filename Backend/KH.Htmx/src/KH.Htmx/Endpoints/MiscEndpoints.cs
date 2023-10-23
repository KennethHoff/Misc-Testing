using KH.Htmx.Components.Components;
using Microsoft.AspNetCore.Http.HttpResults;

namespace KH.Htmx.Endpoints;

public static class MiscEndpoints
{
    public static IEndpointRouteBuilder MapMiscEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/clock", () => new RazorComponentResult<Clock>());

        return app;
    }
}
