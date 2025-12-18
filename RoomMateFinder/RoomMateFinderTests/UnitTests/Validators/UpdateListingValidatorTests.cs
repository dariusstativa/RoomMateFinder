using System.Linq;
using FluentValidation.TestHelper;
using RoomMateFinder.Features.RoomListings.UpdateListing;
using Xunit;

namespace RoomMateFinderTests.UnitTests.Validators;

public class UpdateListingValidatorTests
{
    private readonly UpdateListingValidator _validator = new();

    private static UpdateListingRequest CreateValidRequest() =>
        new UpdateListingRequest
        {
            Title = "Nice updated room",
            Description = "Updated description with enough detail.",
            Address = "Updated Street 123",
            Price = 450m,
            IsAvailable = true,
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
    public void Validate_EmptyTitle_And_NegativePrice_HasErrors()
    {
        
        var request = CreateValidRequest();
        request.Title = "";
        request.Price = -10m;

        
        var result = _validator.TestValidate(request);

     
        result.ShouldHaveValidationErrorFor(x => x.Title);
        result.ShouldHaveValidationErrorFor(x => x.Price);

        
        var titleError = result.Errors.FirstOrDefault(e => e.PropertyName == "Title");
        Assert.NotNull(titleError);
        Assert.Contains("required", titleError!.ErrorMessage, System.StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Validate_TitleTooLong_And_RoommatesOutOfRange_HasErrors()
    {
        
        var request = CreateValidRequest();
        request.Title = new string('a', 101); 
        request.RoommatesCount = 10;        

       
        var result = _validator.TestValidate(request);

        
        result.ShouldHaveValidationErrorFor(x => x.Title);
        result.ShouldHaveValidationErrorFor(x => x.RoommatesCount);
    }

    [Fact]
    public void Validate_EmptyGenderPreference_HasError()
    {
        
        var request = CreateValidRequest();
        request.GenderPreference = "";

        
        var result = _validator.TestValidate(request);

    
        result.ShouldHaveValidationErrorFor(x => x.GenderPreference);
    }
}
