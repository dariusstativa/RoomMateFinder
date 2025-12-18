using System;
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
using RoomMateFinder.Features.RoomListings.UpdateListing;
using RoomMateFinder.Infrastructure.Persistence;
using Xunit;

namespace RoomMateFinderTests.Integration.RoomListings;

public class UpdateListingAsOwnerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public UpdateListingAsOwnerIntegrationTests(CustomWebApplicationFactory factory)
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
    public async Task UpdateListing_AsOwner_ValidRequest_Updates_And_Returns_Success()
    {
        var ownerId = Guid.NewGuid();
        var token = GenerateTestJwt(ownerId);
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // seed cu un scope separat
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            db.RoomListings.RemoveRange(db.RoomListings);
            db.Users.RemoveRange(db.Users);
            await db.SaveChangesAsync();

            var owner = new User
            {
                Id = ownerId,
                Email = "owner-update@test.com",
                PasswordHash = "hashed",
                Salt = "dummy-salt",
                Role = "User"
            };

            db.Users.Add(owner);

            var listing = new RoomListing
            {
                Id = Guid.NewGuid(),
                OwnerId = ownerId,
                Title = "Old title",
                Description = "Old desc",
                Address = "Old address",
                Price = 200m,
                RoommatesCount = 1,
                GenderPreference = "Any",
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow.AddHours(-3)
            };

            db.RoomListings.Add(listing);
            await db.SaveChangesAsync();
        }

        
        Guid listingId;

       
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            listingId = await db.RoomListings
                .AsNoTracking()
                .Where(x => x.OwnerId == ownerId)
                .Select(x => x.Id)
                .FirstAsync();
        }

        var updateRequest = new UpdateListingRequest
        {
            Title = "New title",
            Description = "New description",
            Address = "New address 99",
            Price = 450m,
            IsAvailable = false,
            RoommatesCount = 3,
            GenderPreference = "Female"
        };

        var response = await _client.PutAsJsonAsync($"/listings/{listingId}", updateRequest);

        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.NoContent,
            $"Expected 200 or 204, got {response.StatusCode}");

       
        using (var verifyScope = _factory.Services.CreateScope())
        {
            var verifyDb = verifyScope.ServiceProvider.GetRequiredService<AppDbContext>();

            var updated = await verifyDb.RoomListings.FirstOrDefaultAsync(x => x.Id == listingId);
            Assert.NotNull(updated);

            Assert.Equal(updateRequest.Title, updated!.Title);
            Assert.Equal(updateRequest.Description, updated.Description);
            Assert.Equal(updateRequest.Address, updated.Address);
            Assert.Equal(updateRequest.Price, updated.Price);
            Assert.Equal(updateRequest.IsAvailable, updated.IsAvailable);
            Assert.Equal(updateRequest.RoommatesCount, updated.RoommatesCount);
            Assert.Equal(updateRequest.GenderPreference, updated.GenderPreference);
            Assert.Equal(ownerId, updated.OwnerId);
        }
    }
      [Fact]
    public async Task UpdateListing_AsNonOwner_Returns_NotFound_And_DoesNotChangeListing()
    {
        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        var token = GenerateTestJwt(otherUserId);
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        Guid listingId;

        var oldTitle = "Old title";
        var oldDescription = "Old desc";
        var oldAddress = "Old address";
        var oldPrice = 200m;
        var oldRoommates = 1;
        var oldGender = "Any";
        var oldAvailable = true;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            db.RoomListings.RemoveRange(db.RoomListings);
            db.Users.RemoveRange(db.Users);
            await db.SaveChangesAsync();

            var owner = new User
            {
                Id = ownerId,
                Email = "owner-nonowner@test.com",
                PasswordHash = "hashed",
                Salt = "dummy-salt",
                Role = "User"
            };

            db.Users.Add(owner);

            var listing = new RoomListing
            {
                Id = Guid.NewGuid(),
                OwnerId = ownerId,
                Title = oldTitle,
                Description = oldDescription,
                Address = oldAddress,
                Price = oldPrice,
                RoommatesCount = oldRoommates,
                GenderPreference = oldGender,
                IsAvailable = oldAvailable,
                CreatedAt = DateTime.UtcNow.AddHours(-2)
            };

            db.RoomListings.Add(listing);
            await db.SaveChangesAsync();

            listingId = listing.Id;
        }

        var updateRequest = new UpdateListingRequest
        {
            Title = "Hacker new title",
            Description = "Hacker desc",
            Address = "Hacker address",
            Price = 999m,
            IsAvailable = false,
            RoommatesCount = 4,
            GenderPreference = "Female"
        };

        var response = await _client.PutAsJsonAsync($"/listings/{listingId}", updateRequest);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        using (var verifyScope = _factory.Services.CreateScope())
        {
            var verifyDb = verifyScope.ServiceProvider.GetRequiredService<AppDbContext>();

            var listing = await verifyDb.RoomListings.FirstOrDefaultAsync(x => x.Id == listingId);
            Assert.NotNull(listing);

            Assert.Equal(oldTitle, listing!.Title);
            Assert.Equal(oldDescription, listing.Description);
            Assert.Equal(oldAddress, listing.Address);
            Assert.Equal(oldPrice, listing.Price);
            Assert.Equal(oldAvailable, listing.IsAvailable);
            Assert.Equal(oldRoommates, listing.RoommatesCount);
            Assert.Equal(oldGender, listing.GenderPreference);
            Assert.Equal(ownerId, listing.OwnerId);
        }
    }
}
