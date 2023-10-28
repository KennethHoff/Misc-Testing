using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.HttpResults;

namespace KH.Htmx.Extensions;

public static class TypedResultsExtensions
{
    public static RazorComponentResult<TComponent> BadRequestRazorComponentResult<TComponent>(
        this IResultExtensions _, 
        object? model = null)
        where TComponent : IComponent
    {
        return model switch
        {
            null => new RazorComponentResult<TComponent>
            {
                StatusCode = StatusCodes.Status400BadRequest,
            },
            _ => new RazorComponentResult<TComponent>(model)
            {
                StatusCode = StatusCodes.Status400BadRequest,
            }
        };
    }
}
