using FluentValidation.TestHelper;
using RoomMateFinder.Features.Profiles.CreateProfile;
using Xunit;

namespace RoomMateFinderTests.UnitTests.Validators;

public class CreateProfileValidatorTests
{
    private readonly CreateProfileValidator _validator = new();

    
    private static CreateProfileRequest ValidRequest =>
        new(
            FullName: "John Doe",
            Age: 21,
            Gender: "Male",
            University: "UAIC",
            Bio: "Some description.",
            SleepSchedule: "Night owl",
            Cleanliness: "Clean",
            NoiseTolerance: "Medium",
            SmokingPreference: "Non-smoker",
            PetPreference: "No pets",
            StudyHabits: "Regular"
        );

    

    [Fact]
    public void Should_Have_Error_When_FullName_Is_Empty()
    {
        var model = ValidRequest with { FullName = "" };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.FullName);
    }

    [Fact]
    public void Should_Have_Error_When_FullName_Is_Too_Long()
    {
        var model = ValidRequest with { FullName = new string('a', 101) }; // > 100

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.FullName);
    }

    

    [Fact]
    public void Should_Have_Error_When_Age_Is_Less_Than_18()
    {
        var model = ValidRequest with { Age = 17 };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Age);
    }

    [Fact]
    public void Should_Have_Error_When_Age_Is_Greater_Than_99()
    {
        var model = ValidRequest with { Age = 100 };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Age);
    }

    [Fact]
    public void Should_Have_Error_When_Age_Is_Zero()
    {
       
        var model = ValidRequest with { Age = 0 };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Age);
    }

   

    [Fact]
    public void Should_Have_Error_When_Gender_Is_Empty()
    {
        var model = ValidRequest with { Gender = "" };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Gender);
    }

    [Fact]
    public void Should_Have_Error_When_University_Is_Empty()
    {
        var model = ValidRequest with { University = "" };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.University);
    }

    [Fact]
    public void Should_Have_Error_When_SleepSchedule_Is_Empty()
    {
        var model = ValidRequest with { SleepSchedule = "" };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.SleepSchedule);
    }

    [Fact]
    public void Should_Have_Error_When_Cleanliness_Is_Empty()
    {
        var model = ValidRequest with { Cleanliness = "" };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Cleanliness);
    }

    [Fact]
    public void Should_Have_Error_When_NoiseTolerance_Is_Empty()
    {
        var model = ValidRequest with { NoiseTolerance = "" };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.NoiseTolerance);
    }

    [Fact]
    public void Should_Have_Error_When_SmokingPreference_Is_Empty()
    {
        var model = ValidRequest with { SmokingPreference = "" };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.SmokingPreference);
    }

    [Fact]
    public void Should_Have_Error_When_PetPreference_Is_Empty()
    {
        var model = ValidRequest with { PetPreference = "" };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.PetPreference);
    }

    [Fact]
    public void Should_Have_Error_When_StudyHabits_Is_Empty()
    {
        var model = ValidRequest with { StudyHabits = "" };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.StudyHabits);
    }

   

    [Fact]
    public void Should_Not_Have_Error_For_Bio_When_Null_Or_Whitespace()
    {
        var resultNull = _validator.TestValidate(
            ValidRequest with { Bio = null! });

        resultNull.ShouldNotHaveValidationErrorFor(x => x.Bio);

        var resultEmpty = _validator.TestValidate(
            ValidRequest with { Bio = "" });

        resultEmpty.ShouldNotHaveValidationErrorFor(x => x.Bio);

        var resultWs = _validator.TestValidate(
            ValidRequest with { Bio = "   " });

        resultWs.ShouldNotHaveValidationErrorFor(x => x.Bio);
    }

    [Fact]
    public void Should_Have_Error_When_Bio_Is_Too_Long()
    {
        var model = ValidRequest with { Bio = new string('b', 501) }; 

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Bio);
    }

    

    [Fact]
    public void Should_Pass_When_Request_Is_Valid()
    {
        var model = ValidRequest;

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
