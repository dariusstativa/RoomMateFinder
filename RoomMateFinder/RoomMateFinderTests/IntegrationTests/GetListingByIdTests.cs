using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Infrastructure.Persistence;
using Xunit;

namespace RoomMateFinderTests.Integration.RoomListings;

public class GetListingByIdIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public GetListingByIdIntegrationTests(CustomWebApplicationFactory factory)
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
    public async Task GetListingById_ExistingListing_Returns_Listing_With_Owner()
    {
        var authUserId = Guid.NewGuid();
        var token = GenerateTestJwt(authUserId);
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.RoomListings.RemoveRange(db.RoomListings);
        db.Users.RemoveRange(db.Users);
        await db.SaveChangesAsync();

        var ownerId = Guid.NewGuid();

        var owner = new User
        {
            Id = ownerId,
            Email = "owner@test.com",
            PasswordHash = "hashed",
            Salt = "dummy-salt",
            Role = "User"
        };

        db.Users.Add(owner);

        var listingId = Guid.NewGuid();

        var listing = new RoomListing
        {
            Id = listingId,
            OwnerId = ownerId,
            Title = "Test Listing",
            Description = "Test description",
            Address = "Test street 42",
            Price = 500m,
            RoommatesCount = 2,
            GenderPreference = "Any",
            IsAvailable = true,
            CreatedAt = DateTime.UtcNow
        };

        db.RoomListings.Add(listing);
        await db.SaveChangesAsync();

        var response = await _client.GetAsync($"/listings/{listingId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<RoomListing>();
        Assert.NotNull(result);

        Assert.Equal(listingId, result!.Id);
        Assert.Equal(ownerId, result.OwnerId);
        Assert.Equal("Test Listing", result.Title);
        Assert.Equal("Test description", result.Description);
        Assert.Equal("Test street 42", result.Address);
        Assert.Equal(500m, result.Price);
        Assert.Equal(2, result.RoommatesCount);
        Assert.Equal("Any", result.GenderPreference);

        Assert.NotNull(result.Owner);
        Assert.Equal(ownerId, result.Owner!.Id);
        Assert.Equal("owner@test.com", result.Owner.Email);
    }
    [Fact]
    public async Task GetListingById_NonExistingListing_Returns_NotFound()
    {
        var authUserId = Guid.NewGuid();
        var token = GenerateTestJwt(authUserId);
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.RoomListings.RemoveRange(db.RoomListings);
        await db.SaveChangesAsync();

        var randomId = Guid.NewGuid();

        var response = await _client.GetAsync($"/listings/{randomId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
