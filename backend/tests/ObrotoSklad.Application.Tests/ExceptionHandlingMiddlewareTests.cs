using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ObrotoSklad.Api.Middleware;

namespace ObrotoSklad.Application.Tests;

public class ExceptionHandlingMiddlewareTests
{
    private static async Task<(int Status, string Body)> RunAsync(RequestDelegate next)
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await new ExceptionHandlingMiddleware(next).InvokeAsync(context);

        context.Response.Body.Position = 0;
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        return (context.Response.StatusCode, body);
    }

    [Fact]
    public async Task InvalidOperation_Returns400_WithMessage()
    {
        var (status, body) = await RunAsync(_ => throw new InvalidOperationException("Brak towaru."));

        Assert.Equal(400, status);
        using var doc = JsonDocument.Parse(body);
        Assert.Equal("Brak towaru.", doc.RootElement.GetProperty("error").GetString());
    }

    [Fact]
    public async Task NoException_PassesThrough()
    {
        var (status, body) = await RunAsync(_ => Task.CompletedTask);

        Assert.Equal(200, status);
        Assert.Equal(string.Empty, body);
    }
}