
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Features.Matching.GetMatches;
using RoomMateFinder.Infrastructure.Persistence;
using Xunit;

public class GetMatchesHandlerTests
{
    private static AppDbContext CreateDb()
    {
        var opt = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(opt);
    }

    private static User CreateUser(Guid id) => new User
    {
        Id = id,
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
    public async Task GetMatches_ReturnsMutualLike()
    {
        var db = CreateDb();

        var u1 = CreateUser(Guid.NewGuid());
        var u2 = CreateUser(Guid.NewGuid());

        var p1 = CreateProfile(Guid.NewGuid(), u1);
        var p2 = CreateProfile(Guid.NewGuid(), u2);

        db.Users.AddRange(u1, u2);
        db.Profiles.AddRange(p1, p2);

        db.Likes.Add(new Like { LikerUserId = u1.Id, TargetProfileId = p2.Id, IsLike = true });
        db.Likes.Add(new Like { LikerUserId = u2.Id, TargetProfileId = p1.Id, IsLike = true });

        await db.SaveChangesAsync();

        var handler = new GetMatchesHandler(db);

        var result = await handler.Handle(new GetMatchesQuery(u1.Id), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal(p2.Id, result.First().Id);
    }
}