using System.Net;
using System.Net.Http.Headers;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RoomMateFinder.Features.Reviews;
using RoomMateFinder.Features.Reviews.GetReviwesProfile;
using RoomMateFinderTests.IntegrationTests;
using Xunit;

namespace RoomMateFinderTests.UnitTests.new_tests;

public class GetReviewsForProfileEndpointTests
{
    private static TestServer CreateServer()
    {
        var mediator = new Mock<IMediator>();
        mediator
            .Setup(m => m.Send(It.IsAny<GetReviewsForProfileQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ReviewDto>());

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
                app.UseEndpoints(e => e.MapGetReviewsForProfileEndpoint());
            }));
    }

    [Fact]
    public async Task Get_WithAuth_ReturnsOk()
    {
        using var server = CreateServer();
        var client = server.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");

        var res = await client.GetAsync($"/profiles/{Guid.NewGuid()}/reviews");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }
}