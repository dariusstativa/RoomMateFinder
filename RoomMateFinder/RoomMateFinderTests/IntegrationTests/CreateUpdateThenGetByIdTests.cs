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
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Features.RoomListings.CreateListing;
using RoomMateFinder.Features.RoomListings.UpdateListing;
using RoomMateFinder.Infrastructure.Persistence;
using Xunit;

namespace RoomMateFinderTests.Integration.RoomListings;

public class Flow_CreateUpdateThenGetByIdIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public Flow_CreateUpdateThenGetByIdIntegrationTests(CustomWebApplicationFactory factory)
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
    public async Task Create_Then_Update_Then_GetById_Reflects_Changes()
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
                Email = "flow-update-owner@test.com",
                PasswordHash = "hashed",
                Salt = "dummy-salt",
                Role = "User"
            };

            db.Users.Add(owner);
            await db.SaveChangesAsync();
        }

        var createRequest = new CreateListingRequest
        {
            Title = "Initial title",
            Description = "Initial desc",
            Address = "Initial address",
            Price = 300m,
            RoommatesCount = 1,
            GenderPreference = "Any"
        };

        var createResponse = await _client.PostAsJsonAsync("/listings", createRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var location = createResponse.Headers.Location?.ToString();
        Assert.False(string.IsNullOrWhiteSpace(location));

        var idPart = location!.Split('/', StringSplitOptions.RemoveEmptyEntries).Last();
        var listingId = Guid.Parse(idPart);

        var updateRequest = new UpdateListingRequest
        {
            Title = "Updated title",
            Description = "Updated description",
            Address = "Updated address 22",
            Price = 550m,
            IsAvailable = false,
            RoommatesCount = 3,
            GenderPreference = "Female"
        };

        var updateResponse = await _client.PutAsJsonAsync($"/listings/{listingId}", updateRequest);
        Assert.True(
            updateResponse.StatusCode == HttpStatusCode.OK ||
            updateResponse.StatusCode == HttpStatusCode.NoContent,
            $"Expected 200 or 204, got {updateResponse.StatusCode}");

        var getResponse = await _client.GetAsync($"/listings/{listingId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var listing = await getResponse.Content.ReadFromJsonAsync<RoomListing>();
        Assert.NotNull(listing);

        Assert.Equal(listingId, listing!.Id);
        Assert.Equal(ownerId, listing.OwnerId);
        Assert.Equal(updateRequest.Title, listing.Title);
        Assert.Equal(updateRequest.Description, listing.Description);
        Assert.Equal(updateRequest.Address, listing.Address);
        Assert.Equal(updateRequest.Price, listing.Price);
        Assert.Equal(updateRequest.RoommatesCount, listing.RoommatesCount);
        Assert.Equal(updateRequest.GenderPreference, listing.GenderPreference);
        Assert.Equal(updateRequest.IsAvailable, listing.IsAvailable);
    }
}
