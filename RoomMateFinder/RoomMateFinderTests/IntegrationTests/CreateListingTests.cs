using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RoomMateFinder.Features.RoomListings.CreateListing;
using RoomMateFinder.Infrastructure.Persistence;
using System.IdentityModel.Tokens.Jwt;
using Xunit;

namespace RoomMateFinderTests.Integration.RoomListings;

public class RoomListingsIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public RoomListingsIntegrationTests(CustomWebApplicationFactory factory)
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

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TEST_KEY_FOR_JWT_123456789012345"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "RoomMateFinderTests",
            audience: "RoomMateFinderTests",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [Fact]
    public async Task CreateListing_ValidRequest_Persists_And_Returns_Created()
    {
        var ownerId = Guid.NewGuid();
        var token = GenerateTestJwt(ownerId);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new CreateListingRequest
        {
            Title = "Nice test room",
            Description = "Test desc",
            Address = "Test street 1",
            Price = 350m,
            RoommatesCount = 2,
            GenderPreference = "Any"
        };

        var response = await _client.PostAsJsonAsync("/listings", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdId = await response.Content.ReadFromJsonAsync<Guid>();
        Assert.NotEqual(Guid.Empty, createdId);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var listing = await db.RoomListings.FirstOrDefaultAsync(x => x.Id == createdId);
        Assert.NotNull(listing);
        Assert.Equal(request.Title, listing!.Title);
        Assert.Equal(request.Description, listing.Description);
        Assert.Equal(request.Address, listing.Address);
        Assert.Equal(request.Price, listing.Price);
        Assert.Equal(request.RoommatesCount, listing.RoommatesCount);
        Assert.Equal(request.GenderPreference, listing.GenderPreference);
        Assert.Equal(ownerId, listing.OwnerId);
    }

    [Fact]
    public async Task CreateListing_InvalidRequest_Returns_BadRequest()
    {
        var ownerId = Guid.NewGuid();
        var token = GenerateTestJwt(ownerId);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new CreateListingRequest
        {
            Title = "",
            Description = "",
            Address = "",
            Price = 0m,
            RoommatesCount = -1,
            GenderPreference = "Other"
        };

        var response = await _client.PostAsJsonAsync("/listings", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
