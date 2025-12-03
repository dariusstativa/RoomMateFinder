using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Infrastructure.Persistence;
using System.Security.Cryptography;
using System.Text;
using FluentValidation;
using FluentValidation.Results;

namespace RoomMateFinder.Features.Login.LoginUser;

<<<<<<< HEAD
public class LoginHandler : IRequestHandler<LoginCommand, Guid>
{
    private readonly AppDbContext _db;
    private readonly IValidator<LoginCommand> _validator;
    public LoginHandler(AppDbContext db, IValidator<LoginCommand> validator)
    {_validator = validator;
        _db = db;
    }

    public async Task<Guid> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        ValidationResult validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        var user = await _db.Users
            .FirstOrDefaultAsync(x => x.Email == request.Request.Email, cancellationToken);

        if (user == null)
            throw new Exception("Invalid email or password.");

        if (!VerifyPassword(request.Request.Password, user.Salt, user.PasswordHash))
            throw new Exception("Invalid email or password.");

        return user.Id;
=======
public class LoginHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly AppDbContext _db;
    private readonly IValidator<LoginCommand> _validator;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public LoginHandler(
        AppDbContext db, 
        IValidator<LoginCommand> validator,
        IJwtTokenGenerator tokenGenerator)
    {
        _db = db;
        _validator = validator;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        ValidationResult validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var user = await _db.Users
            .FirstOrDefaultAsync(x => x.Email == request.Request.Email, cancellationToken);

        if (user == null || !VerifyPassword(request.Request.Password, user.Salt, user.PasswordHash))
            throw new Exception("Invalid email or password.");

      
        var token = _tokenGenerator.Generate(user.Id, user.Email);

        
        return new LoginResponse(user.Id, token);
>>>>>>> DariusBranch
    }

    private bool VerifyPassword(string password, string salt, string correctHash)
    {
        using var sha256 = SHA256.Create();
        var combined = Encoding.UTF8.GetBytes(password + salt);
        var hash = sha256.ComputeHash(combined);
        var computed = Convert.ToBase64String(hash);
<<<<<<< HEAD

=======
>>>>>>> DariusBranch
        return computed == correctHash;
    }
}