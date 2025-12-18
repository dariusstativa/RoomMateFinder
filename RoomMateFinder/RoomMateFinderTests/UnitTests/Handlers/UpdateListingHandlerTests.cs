using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Features.RoomListings.UpdateListing;
using RoomMateFinder.Infrastructure.Persistence;
using Xunit;

namespace RoomMateFinderTests.UnitTests.Handlers;

public class UpdateListingHandlerTests
{
    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static UpdateListingRequest ValidRequest =>
        new UpdateListingRequest
        {
            Title = "Updated title",
            Description = "Updated description",
            Address = "Updated address",
            Price = 450m,
            IsAvailable = false,
            RoommatesCount = 3,
            GenderPreference = "Female"
        };

    [Fact]
    public async Task Handle_ValidRequest_ListingExistsAndBelongsToUser_Updates_And_Returns_True()
    {
       
        using var db = CreateDbContext();

        var validatorMock = new Mock<IValidator<UpdateListingRequest>>();
        validatorMock
            .Setup(v => v.Validate(It.IsAny<UpdateListingRequest>()))
            .Returns(new ValidationResult()); 

        var handler = new UpdateListingHandler(db, validatorMock.Object);

        var userId = Guid.NewGuid();
        var listingId = Guid.NewGuid();

        var originalListing = new RoomListing
        {
            Id = listingId,
            OwnerId = userId,
            Title = "Old title",
            Description = "Old description",
            Address = "Old address",
            Price = 300m,
            RoommatesCount = 1,
            GenderPreference = "Any",
            IsAvailable = true,
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        db.RoomListings.Add(originalListing);
        await db.SaveChangesAsync();

        var command = new UpdateListingCommand(
            UserId: userId,
            ListingId: listingId,
            Request: ValidRequest
        );

      
        var result = await handler.Handle(command, CancellationToken.None);

      
        Assert.True(result);

        var listing = await db.RoomListings.FindAsync(listingId);
        Assert.NotNull(listing);

        Assert.Equal(ValidRequest.Title, listing!.Title);
        Assert.Equal(ValidRequest.Description, listing.Description);
        Assert.Equal(ValidRequest.Address, listing.Address);
        Assert.Equal(ValidRequest.Price, listing.Price);
        Assert.Equal(ValidRequest.IsAvailable, listing.IsAvailable);
        Assert.Equal(ValidRequest.RoommatesCount, listing.RoommatesCount);
        Assert.Equal(ValidRequest.GenderPreference, listing.GenderPreference);

        validatorMock.Verify(v => v.Validate(It.IsAny<UpdateListingRequest>()), Times.Once);

    }

    [Fact]
    public async Task Handle_ListingDoesNotExistOrNotOwnedByUser_Returns_False_And_DoesNotUpdate()
    {
        
        using var db = CreateDbContext();

        var validatorMock = new Mock<IValidator<UpdateListingRequest>>();
        validatorMock
            .Setup(v => v.Validate(It.IsAny<UpdateListingRequest>()))
            .Returns(new ValidationResult()); 

        var handler = new UpdateListingHandler(db, validatorMock.Object);

        var realOwnerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var listingId = Guid.NewGuid();

        var listing = new RoomListing
        {
            Id = listingId,
            OwnerId = realOwnerId, 
            Title = "Old title",
            Description = "Old description",
            Address = "Old address",
            Price = 300m,
            RoommatesCount = 1,
            GenderPreference = "Any",
            IsAvailable = true,
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        db.RoomListings.Add(listing);
        await db.SaveChangesAsync();

        var command = new UpdateListingCommand(
            UserId: otherUserId,  
            ListingId: listingId,
            Request: ValidRequest
        );

      
        var result = await handler.Handle(command, CancellationToken.None);

      
        Assert.False(result);

        
        var listingAfter = await db.RoomListings.FindAsync(listingId);
        Assert.NotNull(listingAfter);
        Assert.Equal("Old title", listingAfter!.Title);
        Assert.Equal("Old description", listingAfter.Description);
        Assert.Equal("Old address", listingAfter.Address);
        Assert.Equal(300m, listingAfter.Price);
        Assert.Equal(1, listingAfter.RoommatesCount);
        Assert.Equal("Any", listingAfter.GenderPreference);
        Assert.True(listingAfter.IsAvailable);
    }

    [Fact]
    public async Task Handle_InvalidRequest_Throws_ValidationException()
    {
        
        using var db = CreateDbContext();

        var validatorMock = new Mock<IValidator<UpdateListingRequest>>();

        var failures = new[]
        {
            new ValidationFailure("Title", "Title is required")
        };

        validatorMock
            .Setup(v => v.Validate(It.IsAny<UpdateListingRequest>()))
            .Returns(new ValidationResult(failures)); // invalid

        var handler = new UpdateListingHandler(db, validatorMock.Object);

        var userId = Guid.NewGuid();
        var listingId = Guid.NewGuid();

        var command = new UpdateListingCommand(
            UserId: userId,
            ListingId: listingId,
            Request: ValidRequest 
        );

       
        await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(command, CancellationToken.None));

       
        Assert.Empty(db.RoomListings);
    }
}
