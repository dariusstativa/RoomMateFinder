using MediatR;
using RoomMateFinder.Features.Profiles.GetMyProfile;

public static class GetProfileEndpoint
{
    public static void MapGetProfileEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/profiles/{userId:guid}", async (Guid userId, IMediator mediator) =>
        {
            var profile = await mediator.Send(new GetProfileQuery(userId));
            return profile is not null ? Results.Ok(profile) : Results.NotFound();
        });
    }
}