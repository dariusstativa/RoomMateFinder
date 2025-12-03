using FluentValidation.TestHelper;
using RoomMateFinder.Features.Login.RegisterUser;
using Xunit;

namespace RoomMateFinderTests.Validators;

public class RegisterValidatorTests
{
    private readonly RegisterValidator _validator;

    public RegisterValidatorTests()
    {
        _validator = new RegisterValidator();
    }

   

    [Fact]
    public void Should_Have_Error_When_Email_Is_Empty()
    {
        var request = new RegisterRequest
        {
            Email = "",
            Password = "validPass123"
        };

        var command = new RegisterCommand(request);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Request.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Invalid()
    {
        var request = new RegisterRequest
        {
            Email = "not-an-email",
            Password = "validPass123"
        };

        var command = new RegisterCommand(request);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Request.Email);
    }

    

    [Fact]
    public void Should_Have_Error_When_Password_Is_Empty()
    {
        var request = new RegisterRequest
        {
            Email = "user@example.com",
            Password = ""
        };

        var command = new RegisterCommand(request);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Request.Password);
    }

    [Fact]
    public void Should_Have_Error_When_Password_Is_Too_Short()
    {
        var request = new RegisterRequest
        {
            Email = "user@example.com",
            Password = "123"
        };

        var command = new RegisterCommand(request);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Request.Password);
    }

    

    [Fact]
    public void Should_Pass_When_Request_Is_Valid()
    {
        var request = new RegisterRequest
        {
            Email = "user@example.com",
            Password = "validPassword123"
        };

        var command = new RegisterCommand(request);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
