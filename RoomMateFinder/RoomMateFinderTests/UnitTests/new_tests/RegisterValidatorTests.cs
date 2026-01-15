// RoomMateFinderTests/UnitTests/Validators/RegisterValidatorTests.cs
using FluentValidation.TestHelper;
using RoomMateFinder.Features.Login.RegisterUser;
using Xunit;

public class RegisterValidatorTests
{
    private readonly RegisterValidator _validator = new();

    [Fact]
    public void Valid_Register_Passes()
    {
        var cmd = new RegisterCommand(new RegisterRequest
        {
            Email = "test@test.com",
            Password = "123456"
        });

        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Short_Password_Fails()
    {
        var cmd = new RegisterCommand(new RegisterRequest
        {
            Email = "test@test.com",
            Password = "123"
        });

        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Request.Password);
    }
}