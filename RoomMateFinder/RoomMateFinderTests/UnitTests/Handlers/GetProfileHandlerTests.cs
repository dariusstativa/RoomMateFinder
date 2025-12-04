using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Features.Profiles.GetMyProfile;
using RoomMateFinder.Infrastructure.Persistence;
using Xunit;

namespace RoomMateFinderTests.UnitTests.Handlers;

public class GetProfileHandlerTests
{
    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task Handle_WhenProfileExists_Returns_Profile_With_User_Included()
    {
        // Arrange
        using var db = CreateDbContext();

        var userId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Email = "user@example.com",
            PasswordHash = "hash",
            Salt = "salt",
            Role = "Student"
        };

        var profile = new Profile
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            User = user,
            FullName = "John Doe",
            Age = 21,
            Gender = "M",
            University = "UAIC",
            Bio = "Some bio",
            SleepSchedule = "Night owl",
            Cleanliness = "Clean",
            NoiseTolerance = "Low",
            SmokingPreference = "Non-smoker",
            PetPreference = "No pets",
            StudyHabits = "Regular",
            IsOnboarded = true,
            OnboardedAt = DateTime.UtcNow
        };

        db.Users.Add(user);
        db.Profiles.Add(profile);
        await db.SaveChangesAsync();

        var handler = new GetProfileHandler(db);
        var query = new GetProfileQuery(userId);

        
        var result = await handler.Handle(query, CancellationToken.None);

        
        Assert.NotNull(result);
        Assert.Equal(userId, result!.UserId);
        Assert.Equal("John Doe", result.FullName);
        Assert.NotNull(result.User);
        Assert.Equal("user@example.com", result.User!.Email);
    }

    [Fact]
    public async Task Handle_WhenProfileDoesNotExist_Returns_Null()
    {
        
        using var db = CreateDbContext();
        var handler = new GetProfileHandler(db);

        var query = new GetProfileQuery(Guid.NewGuid());

       
        var result = await handler.Handle(query, CancellationToken.None);

        
        Assert.Null(result);
    }
}
