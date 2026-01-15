
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using RoomMateFinder.Middleware;
using Xunit;

public class ErrorHandlingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_WhenNextThrowsValidationException_Returns400WithPayload()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        RequestDelegate next = _ =>
            throw new ValidationException(new[]
            {
                new FluentValidation.Results.ValidationFailure("Field1", "err1"),
                new FluentValidation.Results.ValidationFailure("Field2", "err2")
            });

        var logger = Mock.Of<ILogger<ErrorHandlingMiddleware>>();
        var mw = new ErrorHandlingMiddleware(next, logger);

        await mw.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        Assert.StartsWith("application/json", context.Response.ContentType);

        context.Response.Body.Position = 0;
        var json = await new StreamReader(context.Response.Body).ReadToEndAsync();

        using var doc = JsonDocument.Parse(json);
        Assert.Equal("Validation failed", doc.RootElement.GetProperty("error").GetString());
        Assert.Equal(2, doc.RootElement.GetProperty("errors").GetArrayLength());
    }

    [Fact]
    public async Task InvokeAsync_WhenNextThrowsException_Returns500WithPayload()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        RequestDelegate next = _ => throw new System.Exception("boom");

        var logger = Mock.Of<ILogger<ErrorHandlingMiddleware>>();
        var mw = new ErrorHandlingMiddleware(next, logger);

        await mw.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        Assert.StartsWith("application/json", context.Response.ContentType);

        context.Response.Body.Position = 0;
        var json = await new StreamReader(context.Response.Body).ReadToEndAsync();

        using var doc = JsonDocument.Parse(json);
        Assert.Equal("Internal server error", doc.RootElement.GetProperty("error").GetString());
        Assert.Equal("boom", doc.RootElement.GetProperty("details").GetString());
    }

    [Fact]
    public async Task InvokeAsync_WhenNextSucceeds_DoesNotChangeStatusCode()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        RequestDelegate next = _ =>
        {
            context.Response.StatusCode = StatusCodes.Status204NoContent;
            return Task.CompletedTask;
        };

        var logger = Mock.Of<ILogger<ErrorHandlingMiddleware>>();
        var mw = new ErrorHandlingMiddleware(next, logger);

        await mw.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status204NoContent, context.Response.StatusCode);
    }
}
