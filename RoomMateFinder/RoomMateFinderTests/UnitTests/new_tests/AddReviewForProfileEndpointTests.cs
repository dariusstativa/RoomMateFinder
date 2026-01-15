using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;
using RoomMateFinderTests.IntegrationTests;

namespace RoomMateFinderTests.UnitTests.new_tests;

public class AddReviewForProfileEndpointTests
{
    [Fact]
    public async Task Post_WithAuth_ReturnsOk()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Test");

        var res = await client.PostAsJsonAsync(
            $"/profiles/{Guid.NewGuid()}/reviews",
            new { rating = 5, comment = "ok" }
        );

        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }
}