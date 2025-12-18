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

public class GetAllListingsIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public GetAllListingsIntegrationTests(CustomWebApplicationFactory factory)
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
    public async Task GetAllListings_WhenListingsExist_Returns_Ordered_By_CreatedAt_Desc()
    {
        var userId = Guid.NewGuid();
        var token = GenerateTestJwt(userId);
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.RoomListings.RemoveRange(db.RoomListings);
        await db.SaveChangesAsync();

        var now = DateTime.UtcNow;

        var older = new RoomListing
        {
            Id = Guid.NewGuid(),
            OwnerId = userId,
            Title = "Older",
            Description = "Older desc",
            Address = "Old street",
            Price = 200m,
            RoommatesCount = 1,
            GenderPreference = "Any",
            IsAvailable = true,
            CreatedAt = now.AddHours(-2)
        };

        var middle = new RoomListing
        {
            Id = Guid.NewGuid(),
            OwnerId = userId,
            Title = "Middle",
            Description = "Middle desc",
            Address = "Middle street",
            Price = 250m,
            RoommatesCount = 2,
            GenderPreference = "Any",
            IsAvailable = true,
            CreatedAt = now.AddHours(-1)
        };

        var newest = new RoomListing
        {
            Id = Guid.NewGuid(),
            OwnerId = userId,
            Title = "Newest",
            Description = "Newest desc",
            Address = "New street",
            Price = 300m,
            RoommatesCount = 3,
            GenderPreference = "Any",
            IsAvailable = true,
            CreatedAt = now
        };

        db.RoomListings.AddRange(older, middle, newest);
        await db.SaveChangesAsync();

        var response = await _client.GetAsync("/listings");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var listings = await response.Content.ReadFromJsonAsync<List<RoomListing>>();
        Assert.NotNull(listings);
        Assert.Equal(3, listings!.Count);

        Assert.Equal(newest.Id, listings[0].Id);
        Assert.Equal(middle.Id, listings[1].Id);
        Assert.Equal(older.Id, listings[2].Id);
    }
}
