// UnitTests/new_tests/UpdateListingHandlerTests.cs
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Features.RoomListings.UpdateListing;
using RoomMateFinder.Infrastructure.Persistence;
using Xunit;

namespace RoomMateFinderTests.UnitTests.new_tests;

public class UpdateListingHandlerTests
{
    private static AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Valid_Update_Returns_True()
    {
        var db = CreateDb();

        var ownerId = Guid.NewGuid();
        var listing = new RoomListing
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId,
            Title = "Old",
            Description = "Old",
            Address = "Old",
            Price = 10,
            RoommatesCount = 1,
            GenderPreference = "Any",
            IsAvailable = true
        };
        db.RoomListings.Add(listing);
        db.SaveChanges();

        var validator = new Mock<IValidator<UpdateListingRequest>>();
        validator.Setup(v => v.Validate(It.IsAny<UpdateListingRequest>()))
            .Returns(new ValidationResult());

        var handler = new UpdateListingHandler(db, validator.Object);

        var cmd = new UpdateListingCommand(
            listing.Id,
            ownerId,
            new UpdateListingRequest
            {
                Title = "New",
                Description = "New",
                Address = "New",
                Price = 20,
                IsAvailable = false,
                RoommatesCount = 2,
                GenderPreference = "Male"
            }
        );

        var ok = await handler.Handle(cmd, default);

        Assert.True(ok);
    }

    [Fact]
    public async Task Listing_Not_Found_Returns_False()
    {
        var db = CreateDb();

        var validator = new Mock<IValidator<UpdateListingRequest>>();
        validator.Setup(v => v.Validate(It.IsAny<UpdateListingRequest>()))
            .Returns(new ValidationResult());

        var handler = new UpdateListingHandler(db, validator.Object);

        var cmd = new UpdateListingCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new UpdateListingRequest()
        );

        var ok = await handler.Handle(cmd, default);

        Assert.False(ok);
    }

    [Fact]
    public async Task Invalid_Request_Throws_ValidationException()
    {
        var db = CreateDb();

        var validator = new Mock<IValidator<UpdateListingRequest>>();
        validator.Setup(v => v.Validate(It.IsAny<UpdateListingRequest>()))
            .Returns(new ValidationResult(new[]
            {
                new ValidationFailure("Title", "err")
            }));

        var handler = new UpdateListingHandler(db, validator.Object);

        var cmd = new UpdateListingCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new UpdateListingRequest()
        );

        await Assert.ThrowsAsync<ValidationException>(() => handler.Handle(cmd, default));
    }
}
