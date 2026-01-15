// UnitTests/new_tests/UpdateListingValidatorTests.cs
using FluentValidation.TestHelper;
using RoomMateFinder.Features.RoomListings.UpdateListing;
using Xunit;

namespace RoomMateFinderTests.UnitTests.new_tests;

public class UpdateListingValidatorTests
{
    private readonly UpdateListingValidator _validator = new();

    [Fact]
    public void Valid_Request_Passes()
    {
        var req = new UpdateListingRequest
        {
            Title = "Title",
            Description = "Desc",
            Address = "Addr",
            Price = 10,
            RoommatesCount = 1,
            GenderPreference = "Any"
        };

        var result = _validator.TestValidate(req);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_Title_Fails()
    {
        var req = new UpdateListingRequest { Title = "" };
        _validator.TestValidate(req)
            .ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Invalid_Price_Fails()
    {
        var req = new UpdateListingRequest { Price = 0 };
        _validator.TestValidate(req)
            .ShouldHaveValidationErrorFor(x => x.Price);
    }
}