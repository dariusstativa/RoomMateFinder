using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RoomMateFinder.Features.Profiles.CompleteOnboarding;
using Xunit;

namespace RoomMateFinderTests.UnitTests.new_tests;

public class CompleteOnboardingEndpointTests
{
    private static TestServer CreateServer(bool commandResult)
    {
        var mediator = new Mock<IMediator>();
        mediator
            .Setup(m => m.Send(It.IsAny<CompleteOnboardingCommand>(), It.IsAny<CancellationToken>()))
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
                app.UseEndpoints(e => e.MapCompleteOnboardingEndpoint());
            }));
    }

    [Fact]
    public async Task Post_WhenOk_Returns204()
    {
        using var server = CreateServer(true);
        var client = server.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Test");

        var res = await client.PostAsJsonAsync(
            $"/profiles/{Guid.NewGuid()}/onboarding",
            new { }
        );

        Assert.Equal(HttpStatusCode.NoContent, res.StatusCode);
    }

    [Fact]
    public async Task Post_WhenMissing_Returns404()
    {
        using var server = CreateServer(false);
        var client = server.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Test");

        var res = await client.PostAsJsonAsync(
            $"/profiles/{Guid.NewGuid()}/onboarding",
            new { }
        );

        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
    }
}
