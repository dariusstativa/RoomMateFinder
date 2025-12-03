using MediatR;
using RoomMateFinder.Features.Profiles;
using System.Security.Claims;

namespace RoomMateFinder.Features.Profiles.GetMyProfile;

public static class GetProfileEndpoint
{
    public static void MapGetMyProfileEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/profiles/me", async (HttpContext http, IMediator mediator, CancellationToken ct) =>
            {
                // 1. Claim prioritization: NameIdentifier → sub
                var userIdClaim =
                    http.User.FindFirst(ClaimTypes.NameIdentifier) ??
                    http.User.FindFirst("sub");

                if (userIdClaim is null)
                {
                    var claims = string.Join(", ", http.User.Claims.Select(c => $"{c.Type}={c.Value}"));
                    Console.WriteLine($"❌ No userId in token. Claims: {claims}");

                    return Results.BadRequest(new
                    {
                        error = "User ID not found in token. Please log in again."
                    });
                }

                var userId = Guid.Parse(userIdClaim.Value);

                // 2. Retrieve profile
                var profile = await mediator.Send(new GetProfileQuery(userId), ct);

                if (profile is null)
                    return Results.NotFound();

                // 3. Convert to DTO (important for frontend consistency)
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