using System.Net;
using System.Net.Http.Headers;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RoomMateFinder.Features.Reviews.DeleteReview;
using Xunit;

namespace RoomMateFinderTests.UnitTests.new_tests;

public class DeleteReviewEndpointTests
{
    private static TestServer CreateServer(bool commandResult)
    {
        var mediator = new Mock<IMediator>();
        mediator
            .Setup(m => m.Send(It.IsAny<DeleteReviewCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(commandResult);

        return new TestServer(new WebHostBuilder()
            .ConfigureServices(s =>
            {
                s.AddRouting();
                s.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
                s.AddAuthorization();
                s.AddSingleton(mediator.Object);
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseAuthentication();
                app.UseAuthorization();
                app.UseEndpoints(e => e.MapDeleteReviewEndpoint());
            }));
    }

    [Fact]
    public async Task Delete_WhenFound_ReturnsOk()
    {
        using var server = CreateServer(true);
        var client = server.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");

        var res = await client.DeleteAsync($"/reviews/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenNotFound_Returns404()
    {
        using var server = CreateServer(false);
        var client = server.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");

        var res = await client.DeleteAsync($"/reviews/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
    }
}
