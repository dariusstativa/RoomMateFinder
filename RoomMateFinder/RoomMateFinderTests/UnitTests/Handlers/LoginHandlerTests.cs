using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Features.Login;
using RoomMateFinder.Features.Login.LoginUser;
using RoomMateFinder.Infrastructure.Persistence;
using Xunit;

namespace RoomMateFinderTests.UnitTests.Handlers;

// Fake simplu pentru JWT – nu depindem de implementarea reală
public class FakeJwtTokenGenerator : IJwtTokenGenerator
{
    public Guid? GeneratedUserId { get; private set; }
    public string? GeneratedEmail { get; private set; }
    public string TokenToReturn { get; set; } = "fake-jwt-token";

    public string Generate(Guid userId, string email)
    {
        GeneratedUserId = userId;
        GeneratedEmail = email;
        return TokenToReturn;
    }
}

public class LoginHandlerTests
{
    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    
    private static User CreateUserWithPassword(string email, string password)
    {
        var salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));

        using var sha256 = SHA256.Create();
        var combined = Encoding.UTF8.GetBytes(password + salt);
        var hash = sha256.ComputeHash(combined);
        var passwordHash = Convert.ToBase64String(hash);

        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Salt = salt,
            PasswordHash = passwordHash,
            Role = "Student"
        };
    }

    [Fact]
    public async Task Handle_ValidCredentials_Returns_LoginResponse_With_Jwt()
    {
        
        using var db = CreateDbContext();

        const string email = "user@example.com";
        const string password = "Password123!";

        var user = CreateUserWithPassword(email, password);
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var validator = new LoginValidator();
        var fakeJwt = new FakeJwtTokenGenerator { TokenToReturn = "jwt-123" };

        var handler = new LoginHandler(db, validator, fakeJwt);

        var command = new LoginCommand(new LoginRequest
        {
            Email = email,
            Password = password
        });

      
        var response = await handler.Handle(command, CancellationToken.None);

      
        Assert.Equal(user.Id, response.UserId);
        Assert.Equal("jwt-123", response.Token);

       
        Assert.Equal(user.Id, fakeJwt.GeneratedUserId);
        Assert.Equal(email, fakeJwt.GeneratedEmail);
    }

    [Fact]
    public async Task Handle_InvalidPassword_Throws_Exception()
    {
        
        using var db = CreateDbContext();

        const string email = "user@example.com";

       
        var user = CreateUserWithPassword(email, "CorrectPassword123!");
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var validator = new LoginValidator();
        var fakeJwt = new FakeJwtTokenGenerator();

        var handler = new LoginHandler(db, validator, fakeJwt);

        var command = new LoginCommand(new LoginRequest
        {
            Email = email,
            Password = "WrongPassword"
        });

        
        await Assert.ThrowsAsync<Exception>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_InvalidRequest_Throws_ValidationException()
    {
        
        using var db = CreateDbContext();
        var validator = new LoginValidator();
        var fakeJwt = new FakeJwtTokenGenerator();

        var handler = new LoginHandler(db, validator, fakeJwt);

        var command = new LoginCommand(new LoginRequest
        {
            Email = "",         
            Password = ""       
        });

       
        await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(command, CancellationToken.None));
    }
}
