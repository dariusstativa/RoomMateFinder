using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Features.RoomListings.DeleteListing;
using RoomMateFinder.Infrastructure.Persistence;
using Xunit;

namespace RoomMateFinderTests.UnitTests.Handlers;

public class DeleteListingHandlerTests
{
    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task Handle_ListingExistsAndBelongsToUser_Deletes_And_Returns_True()
    {
        
        using var db = CreateDbContext();

        var userId = Guid.NewGuid();
        var listingId = Guid.NewGuid();

        
        db.RoomListings.Add(new RoomListing
        {
            Id = listingId,
            OwnerId = userId,
            Title = "Test listing",
            Description = "Desc",
            Address = "Some street",
            Price = 300m,
            RoommatesCount = 2,
            GenderPreference = "Any",
            IsAvailable = true,
            CreatedAt = DateTime.UtcNow
        });

        
        db.RoomListings.Add(new RoomListing
        {
            Id = Guid.NewGuid(),
            OwnerId = Guid.NewGuid(),
            Title = "Other listing",
            Description = "Other",
            Address = "Other street",
            Price = 400m,
            RoommatesCount = 1,
            GenderPreference = "Any",
            IsAvailable = true,
            CreatedAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync();

        var handler = new DeleteListingHandler(db);
       var command = new DeleteListingCommand(listingId, userId);


      
        var result = await handler.Handle(command, CancellationToken.None);

        
        Assert.True(result);

        var deletedListing = await db.RoomListings.FindAsync(listingId);
        Assert.Null(deletedListing); 

        var remainingCount = await db.RoomListings.CountAsync();
        Assert.Equal(1, remainingCount); 
    }

    [Fact]
    public async Task Handle_ListingDoesNotExistOrNotOwnedByUser_Returns_False_And_DoesNotDelete()
    {
        
        using var db = CreateDbContext();

        var userId = Guid.NewGuid();
        var listingId = Guid.NewGuid();

        
        db.RoomListings.Add(new RoomListing
        {
            Id = listingId,
            OwnerId = Guid.NewGuid(),   // alt owner
            Title = "Not my listing",
            Description = "Desc",
            Address = "Some street",
            Price = 300m,
            RoommatesCount = 2,
            GenderPreference = "Any",
            IsAvailable = true,
            CreatedAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync();

        var handler = new DeleteListingHandler(db);
        var command = new DeleteListingCommand(userId, listingId);

       
        var result = await handler.Handle(command, CancellationToken.None);

        
        Assert.False(result);

        
        var listing = await db.RoomListings.FindAsync(listingId);
        Assert.NotNull(listing);

        var count = await db.RoomListings.CountAsync();
        Assert.Equal(1, count);
    }
}
