using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Infrastructure.Persistence;
using System.Security.Cryptography;
using System.Text;
using FluentValidation;
using RoomMateFinder.Features.Login.LoginUser;

namespace RoomMateFinder.Features.Login.RegisterUser;

public class RegisterHandler : IRequestHandler<RegisterCommand, LoginResponse>
{
    private readonly AppDbContext _db;
    private readonly IValidator<RegisterCommand> _validator;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RegisterHandler(
        AppDbContext db,
        IValidator<RegisterCommand> validator,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _db = db;
        _validator = validator;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(request);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        if (await _db.Users.AnyAsync(x => x.Email == request.Request.Email, cancellationToken))
            throw new Exception("Email already registered.");

        var salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
        var hashedPassword = HashPassword(request.Request.Password, salt);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Request.Email,
            PasswordHash = hashedPassword,
            Salt = salt,
            Role = "Student"
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);

        var token = _jwtTokenGenerator.Generate(user.Id, user.Email);
        return new LoginResponse(user.Id, token);
    }

    private string HashPassword(string password, string salt)
    {
        using var sha256 = SHA256.Create();
        var combined = Encoding.UTF8.GetBytes(password + salt);
        var hash = sha256.ComputeHash(combined);
        return Convert.ToBase64String(hash);
    }
}
