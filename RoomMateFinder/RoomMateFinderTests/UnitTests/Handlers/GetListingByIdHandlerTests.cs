using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Features.RoomListings.GetListingById;
using RoomMateFinder.Infrastructure.Persistence;
using Xunit;

namespace RoomMateFinderTests.UnitTests.Handlers;

public class GetListingByIdHandlerTests
{
    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task Handle_ListingExists_Returns_Listing_With_Owner()
    {
      
        using var db = CreateDbContext();

        var ownerId = Guid.NewGuid();
        var listingId = Guid.NewGuid();

        var owner = new User
        {
            Id = ownerId,
            Email = "owner@test.com",
            PasswordHash = "hashed",
            Salt = "dummy-salt",      
            Role = "User"
        };

        db.Users.Add(owner);


        var listing = new RoomListing
        {
            Id = listingId,
            OwnerId = ownerId,
            Title = "Test Listing",
            Description = "Test description",
            Address = "Some street",
            Price = 300m,
            RoommatesCount = 2,
            GenderPreference = "Any",
            IsAvailable = true,
            CreatedAt = DateTime.UtcNow
        };

        db.RoomListings.Add(listing);
        await db.SaveChangesAsync();

        var handler = new GetListingByIdHandler(db);
        var query = new GetListingByIdQuery(listingId);

     
        var result = await handler.Handle(query, CancellationToken.None);

       
        Assert.NotNull(result);
        Assert.Equal(listingId, result!.Id);
        Assert.Equal(ownerId, result.OwnerId);

       
        Assert.NotNull(result.Owner);
        Assert.Equal(ownerId, result.Owner!.Id);
        Assert.Equal("owner@test.com", result.Owner.Email);
    }

    [Fact]
    public async Task Handle_ListingDoesNotExist_Returns_Null()
    {
     
        using var db = CreateDbContext();

        var handler = new GetListingByIdHandler(db);
        var query = new GetListingByIdQuery(Guid.NewGuid());

     
        var result = await handler.Handle(query, CancellationToken.None);

      
        Assert.Null(result);
    }
}
