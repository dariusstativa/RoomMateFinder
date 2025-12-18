using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using RoomMateFinder.Features.RoomListings.CreateListing;
using RoomMateFinder.Infrastructure.Persistence;
using Xunit;

namespace RoomMateFinderTests.UnitTests.Handlers;

public class CreateRoomListingHandlerTests
{
    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static CreateListingRequest ValidRequest =>
        new CreateListingRequest
        {
            Title = "Nice room close to center",
            Description = "Large room with balcony",
            Address = "Some Street 123",
            Price = 300m,
            RoommatesCount = 2,
            GenderPreference = "Any"
        };

    [Fact]
    public async Task Handle_Valid_Request_Creates_Listing_And_Returns_Id()
    {
        
        using var db = CreateDbContext();

        var validatorMock = new Mock<IValidator<CreateListingRequest>>();
        validatorMock
            .Setup(v => v.Validate(It.IsAny<CreateListingRequest>()))
            .Returns(new ValidationResult()); 

        var handler = new CreateRoomListingHandler(db, validatorMock.Object);

        var ownerId = Guid.NewGuid();
        var command = new CreateRoomListingCommand(ownerId, ValidRequest);

      
        var listingId = await handler.Handle(command, CancellationToken.None);

       
        Assert.NotEqual(Guid.Empty, listingId);

        var listing = await db.RoomListings.FindAsync(listingId);
        Assert.NotNull(listing);
        Assert.Equal(ownerId, listing!.OwnerId);
        Assert.Equal("Nice room close to center", listing.Title);
        Assert.Equal("Large room with balcony", listing.Description);
        Assert.Equal("Some Street 123", listing.Address);
        Assert.Equal(300m, listing.Price);
        Assert.Equal(2, listing.RoommatesCount);
        Assert.Equal("Any", listing.GenderPreference);
        Assert.True(listing.IsAvailable);
        Assert.NotEqual(default, listing.CreatedAt);

        
        validatorMock.Verify(v => v.Validate(It.IsAny<CreateListingRequest>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Invalid_Request_Throws_ValidationException()
    {
        
        using var db = CreateDbContext();

        var validatorMock = new Mock<IValidator<CreateListingRequest>>();

        var failures = new[]
        {
            new ValidationFailure("Title", "Title is required")
        };

        validatorMock
            .Setup(v => v.Validate(It.IsAny<CreateListingRequest>()))
            .Returns(new ValidationResult(failures)); 

        var handler = new CreateRoomListingHandler(db, validatorMock.Object);

        var invalidRequest = new CreateListingRequest
        {
            Title = "",  // invalid
            Description = "Large room with balcony",
            Address = "Some Street 123",
            Price = 300m,
            RoommatesCount = 2,
            GenderPreference = "Any"
        };

        var ownerId = Guid.NewGuid();
        var command = new CreateRoomListingCommand(ownerId, invalidRequest);

        
        await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(command, CancellationToken.None));

       
        Assert.Empty(db.RoomListings);
    }
}
