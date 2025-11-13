using MediatR;

namespace RoomMateFinder.Features.Profiles.GetMyProfile;

public static class GetProfileEndpoint
{
    public static void MapGetMyProfileEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/profiles/me", async (IMediator mediator, CancellationToken ct) =>
        {
            // TEMP user Id until JWT auth is added
            var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");

            var profile = await mediator.Send(new GetProfileQuery(userId), ct);

            return profile is not null
                ? Results.Ok(profile)
                : Results.NotFound();
        });
    }
}