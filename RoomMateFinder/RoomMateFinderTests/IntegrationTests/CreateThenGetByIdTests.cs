using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Features.RoomListings.CreateListing;
using RoomMateFinder.Infrastructure.Persistence;
using Xunit;

namespace RoomMateFinderTests.Integration.RoomListings;

public class Flow_CreateThenGetByIdIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public Flow_CreateThenGetByIdIntegrationTests(CustomWebApplicationFactory factory)
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
    public async Task Create_Then_GetById_Returns_SameData()
    {
        var ownerId = Guid.NewGuid();
        var token = GenerateTestJwt(ownerId);
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            db.RoomListings.RemoveRange(db.RoomListings);
            db.Users.RemoveRange(db.Users);
            await db.SaveChangesAsync();

            var owner = new User
            {
                Id = ownerId,
                Email = "flow-owner@test.com",
                PasswordHash = "hashed",
                Salt = "dummy-salt",
                Role = "User"
            };

            db.Users.Add(owner);
            await db.SaveChangesAsync();
        }

        var createRequest = new CreateListingRequest
        {
            Title = "Flow room",
            Description = "Flow description",
            Address = "Flow street 1",
            Price = 400m,
            RoommatesCount = 2,
            GenderPreference = "Any"
        };

        var createResponse = await _client.PostAsJsonAsync("/listings", createRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var location = createResponse.Headers.Location?.ToString();
        Assert.False(string.IsNullOrWhiteSpace(location));

        var idPart = location!.Split('/', StringSplitOptions.RemoveEmptyEntries).Last();
        var listingId = Guid.Parse(idPart);

        var getResponse = await _client.GetAsync($"/listings/{listingId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var listing = await getResponse.Content.ReadFromJsonAsync<RoomListing>();
        Assert.NotNull(listing);

        Assert.Equal(listingId, listing!.Id);
        Assert.Equal(ownerId, listing.OwnerId);
        Assert.Equal(createRequest.Title, listing.Title);
        Assert.Equal(createRequest.Description, listing.Description);
        Assert.Equal(createRequest.Address, listing.Address);
        Assert.Equal(createRequest.Price, listing.Price);
        Assert.Equal(createRequest.RoommatesCount, listing.RoommatesCount);
        Assert.Equal(createRequest.GenderPreference, listing.GenderPreference);
    }
     [Fact]
    public async Task Create_Then_GetAll_Contains_CreatedListing()
    {
        var ownerId = Guid.NewGuid();
        var token = GenerateTestJwt(ownerId);
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.RoomListings.RemoveRange(db.RoomListings);
            db.Users.RemoveRange(db.Users);
            await db.SaveChangesAsync();

            var owner = new User
            {
                Id = ownerId,
                Email = "flow-all-owner@test.com",
                PasswordHash = "hashed",
                Salt = "dummy-salt",
                Role = "User"
            };

            db.Users.Add(owner);
            await db.SaveChangesAsync();
        }

        var createRequest = new CreateListingRequest
        {
            Title = "Flow all room",
            Description = "Flow all description",
            Address = "Flow all street",
            Price = 420m,
            RoommatesCount = 2,
            GenderPreference = "Any"
        };

        var createResponse = await _client.PostAsJsonAsync("/listings", createRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var location = createResponse.Headers.Location?.ToString();
        Assert.False(string.IsNullOrWhiteSpace(location));

        var idPart = location!.Split('/', StringSplitOptions.RemoveEmptyEntries).Last();
        var listingId = Guid.Parse(idPart);

        var getAllResponse = await _client.GetAsync("/listings");
        Assert.Equal(HttpStatusCode.OK, getAllResponse.StatusCode);

        var listings = await getAllResponse.Content.ReadFromJsonAsync<List<RoomListing>>();
        Assert.NotNull(listings);
        Assert.NotEmpty(listings!);

        var created = listings!.FirstOrDefault(l => l.Id == listingId);
        Assert.NotNull(created);

        Assert.Equal(ownerId, created!.OwnerId);
        Assert.Equal(createRequest.Title, created.Title);
        Assert.Equal(createRequest.Description, created.Description);
        Assert.Equal(createRequest.Address, created.Address);
        Assert.Equal(createRequest.Price, created.Price);
        Assert.Equal(createRequest.RoommatesCount, created.RoommatesCount);
        Assert.Equal(createRequest.GenderPreference, created.GenderPreference);
    }
}
