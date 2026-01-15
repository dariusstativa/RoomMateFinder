// RoomMateFinderTests/UnitTests/Validators/CreateRoomListingValidatorTests.cs
using FluentValidation.TestHelper;
using RoomMateFinder.Features.RoomListings.CreateListing;
using Xunit;

public class CreateRoomListingValidatorTests
{
    private readonly CreateRoomListingValidator _validator = new();

    [Fact]
    public void Valid_Request_Passes()
    {
        var req = new CreateListingRequest
        {
            Title = "Room",
            Description = "Nice room",
            Address = "Street 1",
            Price = 300,
            RoommatesCount = 2,
            GenderPreference = "Any"
        };

        var result = _validator.TestValidate(req);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Invalid_GenderPreference_Fails()
    {
        var req = new CreateListingRequest { GenderPreference = "Other" };
        var result = _validator.TestValidate(req);
        result.ShouldHaveValidationErrorFor(x => x.GenderPreference);
    }
}