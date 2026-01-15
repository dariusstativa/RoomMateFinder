// RoomMateFinderTests/UnitTests/Validators/LoginValidatorTests.cs
using FluentValidation.TestHelper;
using RoomMateFinder.Features.Login.LoginUser;
using Xunit;

public class LoginValidatorTests
{
    private readonly LoginValidator _validator = new();

    [Fact]
    public void Valid_Login_Passes()
    {
        var cmd = new LoginCommand(new LoginRequest
        {
            Email = "test@test.com",
            Password = "pass"
        });

        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Missing_Email_Fails()
    {
        var cmd = new LoginCommand(new LoginRequest());
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Request.Email);
    }
}