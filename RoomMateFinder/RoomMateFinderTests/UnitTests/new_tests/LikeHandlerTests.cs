
using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Features.LikeProfile.LikeRequest;
using RoomMateFinder.Features.Matching.LikeProfile;
using RoomMateFinder.Infrastructure.Persistence;
using Xunit;

public class LikeHandlerTests
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
    public async Task Like_NewLike_ReturnsTrue_AndCreatesLike()
    {
        var db = CreateDb();

        var liker = CreateUser(Guid.NewGuid());
        var targetUser = CreateUser(Guid.NewGuid());
        var profile = CreateProfile(Guid.NewGuid(), targetUser);

        db.Users.AddRange(liker, targetUser);
        db.Profiles.Add(profile);
        await db.SaveChangesAsync();

        var validator = new Mock<IValidator<LikeRequest>>();
        validator.Setup(v => v.ValidateAsync(It.IsAny<LikeRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var handler = new LikeHandler(db, validator.Object);

        var result = await handler.Handle(
            new LikeCommand(liker.Id, new LikeRequest { TargetProfileId = profile.Id }),
            CancellationToken.None);

        Assert.True(result);
        Assert.Single(db.Likes);
    }
}
