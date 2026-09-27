using System.Text.Json;

namespace ObrotoSklad.Api.Middleware;

public class ExceptionHandlingMiddleware 
{
    private readonly RequestDelegate _next;
    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (InvalidOperationException ex)
        {
            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/json";

            var errorResponse = new { error = ex.Message };
            var json = JsonSerializer.Serialize(errorResponse);

            await context.Response.WriteAsync(json);
        }
    }
}
