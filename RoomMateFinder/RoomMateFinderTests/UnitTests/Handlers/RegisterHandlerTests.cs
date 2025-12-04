using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Features.Login.RegisterUser;
using RoomMateFinder.Infrastructure.Persistence;
using RoomMateFinder.Features.Login;
using Xunit;

namespace RoomMateFinderTests.UnitTests.Handlers
{
    

    public class RegisterHandlerTests
    {
        private static AppDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task Handle_ValidRequest_CreatesUser_And_Returns_Response()
        {
            using var db = CreateDbContext();
            var validator = new RegisterValidator();
            var jwt = new FakeJwtTokenGenerator();
            var handler = new RegisterHandler(db, validator, jwt);

            var command = new RegisterCommand(
                new RegisterRequest
                {
                    Email = "test@example.com",
                    Password = "Password123!"
                });

            var response = await handler.Handle(command, CancellationToken.None);

            Assert.NotEqual(Guid.Empty, response.UserId);
            Assert.NotNull(response.Token);

            var user = await db.Users.FindAsync(response.UserId);
            Assert.NotNull(user);
            Assert.Equal("test@example.com", user!.Email);
            Assert.Equal("Student", user.Role);
            Assert.False(string.IsNullOrWhiteSpace(user.Salt));
            Assert.False(string.IsNullOrWhiteSpace(user.PasswordHash));
            Assert.NotEqual("Password123!", user.PasswordHash);
        }

        [Fact]
        public async Task Handle_DuplicateEmail_Throws_Exception()
        {
            using var db = CreateDbContext();

            db.Users.Add(new User
            {
                Id = Guid.NewGuid(),
                Email = "duplicate@example.com",
                PasswordHash = "hash",
                Salt = "salt",
                Role = "Student"
            });
            await db.SaveChangesAsync();

            var validator = new RegisterValidator();
            var jwt = new FakeJwtTokenGenerator();
            var handler = new RegisterHandler(db, validator, jwt);

            var command = new RegisterCommand(
                new RegisterRequest
                {
                    Email = "duplicate@example.com",
                    Password = "Password123!"
                });

            await Assert.ThrowsAsync<Exception>(
                () => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_InvalidRequest_Throws_ValidationException()
        {
            using var db = CreateDbContext();
            var validator = new RegisterValidator();
            var jwt = new FakeJwtTokenGenerator();
            var handler = new RegisterHandler(db, validator, jwt);

            var command = new RegisterCommand(
                new RegisterRequest
                {
                    Email = "",
                    Password = "123"
                });

            await Assert.ThrowsAsync<ValidationException>(
                () => handler.Handle(command, CancellationToken.None));
        }
    }
}
