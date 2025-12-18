using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Features.RoomListings.GetAllListings;
using RoomMateFinder.Infrastructure.Persistence;
using Xunit;

namespace RoomMateFinderTests.UnitTests.Handlers;

public class GetAllListingsHandlerTests
{
    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task Handle_WhenListingsExist_Returns_Ordered_By_CreatedAt_Descending()
    {
        
        using var db = CreateDbContext();

        var now = DateTime.UtcNow;

        var olderListing = new RoomListing
        {
            Id = Guid.NewGuid(),
            OwnerId = Guid.NewGuid(),
            Title = "Older listing",
            Description = "Old desc",
            Address = "Old street",
            Price = 200m,
            RoommatesCount = 1,
            GenderPreference = "Any",
            IsAvailable = true,
            CreatedAt = now.AddMinutes(-30)
        };

        var middleListing = new RoomListing
        {
            Id = Guid.NewGuid(),
            OwnerId = Guid.NewGuid(),
            Title = "Middle listing",
            Description = "Middle desc",
            Address = "Middle street",
            Price = 250m,
            RoommatesCount = 2,
            GenderPreference = "Any",
            IsAvailable = true,
            CreatedAt = now.AddMinutes(-10)
        };

        var newestListing = new RoomListing
        {
            Id = Guid.NewGuid(),
            OwnerId = Guid.NewGuid(),
            Title = "Newest listing",
            Description = "New desc",
            Address = "New street",
            Price = 300m,
            RoommatesCount = 3,
            GenderPreference = "Any",
            IsAvailable = true,
            CreatedAt = now
        };

        db.RoomListings.AddRange(olderListing, middleListing, newestListing);
        await db.SaveChangesAsync();

        var handler = new GetAllListingsHandler(db);
        var query = new GetAllListingsQuery();

     
        var result = await handler.Handle(query, CancellationToken.None);

     
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);

      
        Assert.Equal(newestListing.Id, result[0].Id);
        Assert.Equal(middleListing.Id, result[1].Id);
        Assert.Equal(olderListing.Id, result[2].Id);
    }

    [Fact]
    public async Task Handle_WhenNoListingsExist_Returns_Empty_List()
    {
       
        using var db = CreateDbContext();

        var handler = new GetAllListingsHandler(db);
        var query = new GetAllListingsQuery();

       
        var result = await handler.Handle(query, CancellationToken.None);

       
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
