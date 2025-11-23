using MediatR;
<<<<<<< HEAD
using System.Security.Claims;
=======
using RoomMateFinder.Features.Profiles;
>>>>>>> CleanFixBranch

namespace RoomMateFinder.Features.Profiles.GetMyProfile;

public static class GetProfileEndpoint
{
    public static void MapGetMyProfileEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/profiles/me", async (HttpContext http, IMediator mediator, CancellationToken ct) =>
            {
<<<<<<< HEAD
                var userId = Guid.Parse(http.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                var profile = await mediator.Send(new GetProfileQuery(userId), ct);

                return profile is not null
                    ? Results.Ok(profile)
                    : Results.NotFound();
=======
                var userId = Guid.Parse(http.User.FindFirst("sub")!.Value);

                var profile = await mediator.Send(new GetProfileQuery(userId), ct);

                if (profile is null)
                    return Results.NotFound();

                var dto = new ProfileDto(
                    profile.Id,
                    profile.UserId,
                    profile.FullName,
                    profile.Age,
                    profile.Gender,
                    profile.University,
                    profile.Bio,
                    profile.SleepSchedule,
                    profile.Cleanliness,
                    profile.NoiseTolerance,
                    profile.SmokingPreference,
                    profile.PetPreference,
                    profile.StudyHabits
                );

                return Results.Ok(dto);
>>>>>>> CleanFixBranch
            })
            .RequireAuthorization();
    }
}