// UnitTests/new_tests/CreateRoomListingHandlerTests.cs
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Features.RoomListings.CreateListing;
using RoomMateFinder.Infrastructure.Persistence;
using Xunit;

namespace RoomMateFinderTests.UnitTests.new_tests;

public class CreateRoomListingHandlerTests
{
    private static AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Valid_Request_Creates_Listing_And_Returns_Id()
    {
        var db = CreateDb();

        var validator = new Mock<IValidator<CreateListingRequest>>();
        validator.Setup(v => v.Validate(It.IsAny<CreateListingRequest>()))
            .Returns(new ValidationResult());

        var handler = new CreateRoomListingHandler(db, validator.Object);

        var cmd = new CreateRoomListingCommand(
            OwnerId: Guid.NewGuid(),
            Request: new CreateListingRequest
            {
                Title = "Title",
                Description = "Desc",
                Address = "Addr",
                Price = 100,
                RoommatesCount = 1,
                GenderPreference = "Any"
            }
        );

        var id = await handler.Handle(cmd, default);

        Assert.NotEqual(Guid.Empty, id);
        Assert.Single(db.RoomListings);
    }

    [Fact]
    public async Task Invalid_Request_Throws_ValidationException()
    {
        var db = CreateDb();

        var validator = new Mock<IValidator<CreateListingRequest>>();
        validator.Setup(v => v.Validate(It.IsAny<CreateListingRequest>()))
            .Returns(new ValidationResult(new[]
            {
                new ValidationFailure("Title", "err")
            }));

        var handler = new CreateRoomListingHandler(db, validator.Object);

        var cmd = new CreateRoomListingCommand(Guid.NewGuid(), new CreateListingRequest());

        await Assert.ThrowsAsync<ValidationException>(() => handler.Handle(cmd, default));
    }
}
