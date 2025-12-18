using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Infrastructure.Persistence;
using Xunit;

namespace RoomMateFinderTests.Integration.RoomListings;

public class GetAllListingsEmptyIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public GetAllListingsEmptyIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private static string GenerateTestJwt(Guid userId)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: "RoomMateFinderTests",
            audience: "RoomMateFinderTests",
            claims: claims);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [Fact]
    public async Task GetAllListings_WhenNoListingsExist_Returns_Empty_List()
    {
        var userId = Guid.NewGuid();
        var token = GenerateTestJwt(userId);
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.RoomListings.RemoveRange(db.RoomListings);
        await db.SaveChangesAsync();

        var response = await _client.GetAsync("/listings");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var listings = await response.Content.ReadFromJsonAsync<List<RoomListing>>();
        Assert.NotNull(listings);
        Assert.Empty(listings!);
    }
}
