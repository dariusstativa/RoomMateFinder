using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Infrastructure.Persistence;
using System.Security.Cryptography;
using System.Text;

namespace RoomMateFinder.Features.Login.RegisterUser;

public class RegisterHandler : IRequestHandler<RegisterCommand, Guid>
{
    private readonly AppDbContext _db;

    public RegisterHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Guid> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // verificăm dacă email-ul există
        if (await _db.Users.AnyAsync(x => x.Email == request.Request.Email, cancellationToken))
            throw new Exception("Email already registered.");

        // generăm salt
        var salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));

        // hash la parolă
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

        return user.Id;
    }

    private string HashPassword(string password, string salt)
    {
        using var sha256 = SHA256.Create();
        var combined = Encoding.UTF8.GetBytes(password + salt);
        var hash = sha256.ComputeHash(combined);
        return Convert.ToBase64String(hash);
    }
}