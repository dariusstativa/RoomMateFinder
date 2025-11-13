using FluentValidation;

namespace RoomMateFinder.Features.Login.LoginUser;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Request.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress();

        RuleFor(x => x.Request.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}