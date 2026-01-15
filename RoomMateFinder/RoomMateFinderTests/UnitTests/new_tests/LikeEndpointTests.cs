
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RoomMateFinder.Features.LikeProfile.LikeRequest;
using Xunit;

public class LikeEndpointTests
{
    private static TestServer CreateServer()
    {
        var mediator = new Mock<IMediator>();
        mediator
            .Setup(m => m.Send(It.IsAny<IRequest<bool>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var validator = new Mock<IValidator<LikeRequest>>();
        validator
            .Setup(v => v.ValidateAsync(It.IsAny<LikeRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        return new TestServer(
            new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddRouting();
                    services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, LikeAuthHandler>("Test", _ => { });
                    services.AddAuthorization();

                    services.AddSingleton<IMediator>(mediator.Object);
                    services.AddSingleton<IValidator<LikeRequest>>(validator.Object);
                })
                .Configure(app =>
                {
                    app.UseRouting();
                    app.UseAuthentication();
                    app.UseAuthorization();

                    app.UseEndpoints(e =>
                        RoomMateFinder.Features.Matching.LikeProfile.LikeEndpoints.MapLikeEndpoints(e)
                    );
                })
        );
    }

    [Fact]
    public async Task Like_WithUser_ReturnsOk()
    {
        using var server = CreateServer();
        var client = server.CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Test", "ok");

        var res = await client.PostAsJsonAsync(
            "/matching/like",
            new LikeRequest { TargetProfileId = Guid.NewGuid() }
        );

        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }

    [Fact]
    public async Task Like_WithoutUser_ReturnsUnauthorized()
    {
        using var server = CreateServer();
        var client = server.CreateClient();

        var res = await client.PostAsJsonAsync(
            "/matching/like",
            new LikeRequest { TargetProfileId = Guid.NewGuid() }
        );

        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }
}

public class LikeAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public LikeAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) : base(options, logger, encoder, clock)
    { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
            return Task.FromResult(AuthenticateResult.NoResult());

        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
        }, "Test");

        var principal = new ClaimsPrincipal(identity);

        return Task.FromResult(
            AuthenticateResult.Success(
                new AuthenticationTicket(principal, "Test")
            )
        );
    }
}
