using MediatR;
using RoomMateFinder.Features.Profiles;

namespace RoomMateFinder.Features.Profiles.GetMyProfile;

public static class GetProfileEndpoint
{
    public static void MapGetMyProfileEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/profiles/me", async (HttpContext http, IMediator mediator, CancellationToken ct) =>
            {
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
            })
            .RequireAuthorization();
    }
}