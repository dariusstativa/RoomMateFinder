
using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Features.Matching.DislikeProfile;
using RoomMateFinder.Infrastructure.Persistence;
using Xunit;

public class DislikeHandlerTests
{
    private static AppDbContext CreateDb()
    {
        var opt = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(opt);
    }

    private static User CreateUser(Guid id, int rating = 1000) => new User
    {
        Id = id,
        Rating = rating,
        Email = $"{id}@test.com",
        PasswordHash = "hash",
        Salt = "salt"
    };

    private static Profile CreateProfile(Guid id, User user) => new Profile
    {
        Id = id,
        User = user,
        UserId = user.Id,

        FullName = "Test User",
        Age = 22,
        University = "UAIC",
        Gender = "M",
        Bio = "test bio",
        Cleanliness = "3",
        NoiseTolerance = "3",
        PetPreference = "No",
        SleepSchedule = "Night",
        SmokingPreference = "No",
        StudyHabits = "Medium"
    };

    [Fact]
    public async Task Dislike_NewDislike_ReturnsTrue()
    {
        var db = CreateDb();

        var disliker = CreateUser(Guid.NewGuid());
        var targetUser = CreateUser(Guid.NewGuid());
        var profile = CreateProfile(Guid.NewGuid(), targetUser);

        db.Users.AddRange(disliker, targetUser);
        db.Profiles.Add(profile);
        await db.SaveChangesAsync();

        var validator = new Mock<IValidator<DislikeRequest>>();
        validator.Setup(v => v.ValidateAsync(It.IsAny<DislikeRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var handler = new DislikeHandler(db, validator.Object);

        var result = await handler.Handle(
            new DislikeCommand(disliker.Id, new DislikeRequest { TargetProfileId = profile.Id }),
            CancellationToken.None);

        Assert.True(result);
        Assert.Single(db.Likes);
        Assert.False(db.Likes.First().IsLike);
    }
}
