
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Features.Matching.GetRecommendations;
using RoomMateFinder.Infrastructure.Persistence;
using Xunit;

public class GetRecommendationsHandlerTests
{
    private static AppDbContext CreateDb()
    {
        var opt = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(opt);
    }

    private static User CreateUser(Guid id, int rating) => new User
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
    public async Task GetRecommendations_ReturnsSameUniversityProfiles()
    {
        var db = CreateDb();

        var me = CreateUser(Guid.NewGuid(), 1000);
        var meProfile = CreateProfile(Guid.NewGuid(), me);

        var other = CreateUser(Guid.NewGuid(), 1100);
        var otherProfile = CreateProfile(Guid.NewGuid(), other);

        db.Users.AddRange(me, other);
        db.Profiles.AddRange(meProfile, otherProfile);
        await db.SaveChangesAsync();

        var handler = new GetRecommendationsHandler(db);

        var result = await handler.Handle(new GetRecommendationsQuery(me.Id), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal(otherProfile.Id, result.First().ProfileId);
    }
}