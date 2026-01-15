using FluentValidation.TestHelper;
using RoomMateFinder.Features.Profiles.CreateProfile;
using Xunit;

namespace RoomMateFinderTests.UnitTests.new_tests;

public class CreateProfileValidatorTests
{
    private readonly CreateProfileValidator _validator = new();

    private static CreateProfileRequest ValidRequest() => new(
        FullName: "John Doe",
        Age: 22,
        Gender: "Male",
        University: "UAIC",
        Bio: "Bio",
        SleepSchedule: "Night",
        Cleanliness: "High",
        NoiseTolerance: "Medium",
        SmokingPreference: "No",
        PetPreference: "No",
        StudyHabits: "Often"
    );

    [Fact]
    public void Empty_FullName_Fails()
    {
        var req = ValidRequest() with { FullName = "" };
        _validator.TestValidate(req).ShouldHaveValidationErrorFor(x => x.FullName);
    }

    [Fact]
    public void Age_Out_Of_Range_Fails()
    {
        var req = ValidRequest() with { Age = 10 };
        _validator.TestValidate(req).ShouldHaveValidationErrorFor(x => x.Age);
    }
}