using FluentValidation.TestHelper;
using RoomMateFinder.Features.Profiles.UpdateProfile;
using Xunit;

namespace RoomMateFinderTests.UnitTests.Validators;

public class UpdateProfileValidatorTests
{
    private readonly UpdateProfileValidator _validator = new();

    private static UpdateProfileRequest ValidRequest =>
        new(   Bio:"Asta e bio-ul meu",
            SleepSchedule: "Night owl",
            Cleanliness: "Clean",
            NoiseTolerance: "Medium",
            SmokingPreference: "Non-smoker",
            PetPreference: "No pets",
            StudyHabits: "Regular"
        );

    

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
    public void Should_Pass_When_Request_Is_Valid()
    {
        var model = ValidRequest;

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
