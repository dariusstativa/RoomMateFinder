using FluentValidation.TestHelper;
using RoomMateFinder.Features.Profiles.UpdateProfile;
using Xunit;

namespace RoomMateFinderTests.UnitTests.new_tests;

public class UpdateProfileValidatorTests
{
    private readonly UpdateProfileValidator _validator = new();

    private static UpdateProfileRequest ValidRequest() => new(
        SleepSchedule: "Night",
        Cleanliness: "High",
        NoiseTolerance: "Medium",
        SmokingPreference: "No",
        PetPreference: "No",
        StudyHabits: "Often",
        Bio: "Bio"
    );

    [Fact]
    public void Valid_Request_Passes()
    {
        _validator.TestValidate(ValidRequest())
            .ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Missing_Field_Fails()
    {
        var req = ValidRequest() with { Cleanliness = "" };
        _validator.TestValidate(req)
            .ShouldHaveValidationErrorFor(x => x.Cleanliness);
    }
}