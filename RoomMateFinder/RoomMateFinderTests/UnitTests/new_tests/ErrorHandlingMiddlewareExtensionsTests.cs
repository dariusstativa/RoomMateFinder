
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RoomMateFinder.Middleware;
using Xunit;

public class ErrorHandlingMiddlewareExtensionsTests
{
    [Fact]
    public void UseErrorHandling_RegistersMiddlewareInPipeline()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        var sp = services.BuildServiceProvider();

        var app = new ApplicationBuilder(sp);

        var returned = app.UseErrorHandling();

        Assert.Same(app, returned);
        Assert.NotEmpty(app.Properties);
    }
}