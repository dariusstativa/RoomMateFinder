using FluentValidation.TestHelper;
using RoomMateFinder.Features.Login.LoginUser;
using Xunit;

namespace RoomMateFinderTests.Validators;

public class LoginValidatorTests
{
    private readonly LoginValidator _validator;

    public LoginValidatorTests()
    {
        _validator = new LoginValidator();
    }

    

    [Fact]
    public void Should_Have_Error_When_Email_Is_Empty()
    {
        var request = new LoginRequest
        {
            Email = "",
            Password = "validPassword123"
        };

        var command = new LoginCommand(request);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Request.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Invalid()
    {
        var request = new LoginRequest
        {
            Email = "not-an-email",
            Password = "validPassword123"
        };

        var command = new LoginCommand(request);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Request.Email);
    }

    

    [Fact]
    public void Should_Have_Error_When_Password_Is_Empty()
    {
        var request = new LoginRequest
        {
            Email = "user@example.com",
            Password = ""
        };

        var command = new LoginCommand(request);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Request.Password);
    }

    

    [Fact]
    public void Should_Pass_When_Request_Is_Valid()
    {
        var request = new LoginRequest
        {
            Email = "user@example.com",
            Password = "validPassword123"
        };

        var command = new LoginCommand(request);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}