using System;
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

public class DeleteListingAsOwnerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public DeleteListingAsOwnerIntegrationTests(CustomWebApplicationFactory factory)
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
    public async Task DeleteListing_AsOwner_RemovesListing_And_Returns_Success()
    {
        var ownerId = Guid.NewGuid();
        var token = GenerateTestJwt(ownerId);
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        Guid listingId;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            db.RoomListings.RemoveRange(db.RoomListings);
            db.Users.RemoveRange(db.Users);
            await db.SaveChangesAsync();

            var owner = new User
            {
                Id = ownerId,
                Email = "owner-delete@test.com",
                PasswordHash = "hashed",
                Salt = "dummy-salt",
                Role = "User"
            };

            db.Users.Add(owner);

            var listing = new RoomListing
            {
                Id = Guid.NewGuid(),
                OwnerId = ownerId,
                Title = "To be deleted",
                Description = "To be deleted desc",
                Address = "Delete street",
                Price = 250m,
                RoommatesCount = 1,
                GenderPreference = "Any",
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow.AddHours(-1)
            };

            db.RoomListings.Add(listing);
            await db.SaveChangesAsync();

            listingId = listing.Id;
        }

        var response = await _client.DeleteAsync($"/listings/{listingId}");

        Assert.True(
            response.StatusCode == HttpStatusCode.NoContent ||
            response.StatusCode == HttpStatusCode.OK,
            $"Expected 204 or 200, got {response.StatusCode}");

        using (var verifyScope = _factory.Services.CreateScope())
        {
            var verifyDb = verifyScope.ServiceProvider.GetRequiredService<AppDbContext>();
            var deleted = await verifyDb.RoomListings.FirstOrDefaultAsync(x => x.Id == listingId);
            Assert.Null(deleted);
        }
    }
     [Fact]
    public async Task DeleteListing_AsNonOwner_Returns_NotFound_And_DoesNotDelete()
    {
        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        var token = GenerateTestJwt(otherUserId);
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        Guid listingId;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            db.RoomListings.RemoveRange(db.RoomListings);
            db.Users.RemoveRange(db.Users);
            await db.SaveChangesAsync();

            var owner = new User
            {
                Id = ownerId,
                Email = "owner-delete-nonowner@test.com",
                PasswordHash = "hashed",
                Salt = "dummy-salt",
                Role = "User"
            };

            db.Users.Add(owner);

            var listing = new RoomListing
            {
                Id = Guid.NewGuid(),
                OwnerId = ownerId,
                Title = "Protected listing",
                Description = "Should not be deleted",
                Address = "Protected street",
                Price = 300m,
                RoommatesCount = 2,
                GenderPreference = "Any",
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow.AddHours(-1)
            };

            db.RoomListings.Add(listing);
            await db.SaveChangesAsync();

            listingId = listing.Id;
        }

        var response = await _client.DeleteAsync($"/listings/{listingId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        using (var verifyScope = _factory.Services.CreateScope())
        {
            var verifyDb = verifyScope.ServiceProvider.GetRequiredService<AppDbContext>();
            var listing = await verifyDb.RoomListings.FirstOrDefaultAsync(x => x.Id == listingId);
            Assert.NotNull(listing);
            Assert.Equal(ownerId, listing!.OwnerId);
            Assert.Equal("Protected listing", listing.Title);
        }
    }
}

