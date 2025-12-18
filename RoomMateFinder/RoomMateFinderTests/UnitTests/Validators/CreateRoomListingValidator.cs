using FluentValidation.TestHelper;
using RoomMateFinder.Features.RoomListings.CreateListing;
using Xunit;

namespace RoomMateFinderTests.UnitTests.Validators;

public class CreateRoomListingValidatorTests
{
    private readonly CreateRoomListingValidator _validator = new();

    private static CreateListingRequest CreateValidRequest() =>
        new()
        {
            Title = "Nice room close to center",
            Description = "Large room with balcony",
            Address = "Some Street 123",
            Price = 300m,
            RoommatesCount = 2,
            GenderPreference = "Any"
        };

    [Fact]
    public void Validate_ValidRequest_IsValid()
    {
        var request = CreateValidRequest();
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyTitle_HasError()
    {
        var request = CreateValidRequest();
        request.Title = "";
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_TitleTooLong_HasError()
    {
        var request = CreateValidRequest();
        request.Title = new string('a', 101);
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_EmptyDescription_HasError()
    {
        var request = CreateValidRequest();
        request.Description = "";
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_EmptyAddress_HasError()
    {
        var request = CreateValidRequest();
        request.Address = "";
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Address);
    }

    [Fact]
    public void Validate_NonPositivePrice_HasError()
    {
        var request = CreateValidRequest();
        request.Price = 0m;
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Price);

        request.Price = -10m;
        result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void Validate_RoommatesCount_OutOfRange_HasError()
    {
        var request = CreateValidRequest();
        request.RoommatesCount = -1;
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.RoommatesCount);

        request.RoommatesCount = 5;
        result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.RoommatesCount);
    }

    [Theory]
    [InlineData("Male")]
    [InlineData("Female")]
    [InlineData("Any")]
    public void Validate_ValidGenderPreference_IsValid(string gender)
    {
        var request = CreateValidRequest();
        request.GenderPreference = gender;
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.GenderPreference);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Other")]
    [InlineData("male")]
    public void Validate_InvalidGenderPreference_HasError(string gender)
    {
        var request = CreateValidRequest();
        request.GenderPreference = gender;
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.GenderPreference);
    }
}
