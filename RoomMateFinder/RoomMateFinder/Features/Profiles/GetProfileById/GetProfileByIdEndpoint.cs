using MediatR;

namespace RoomMateFinder.Features.Profiles.GetProfileById;

public static class GetProfileByIdEndpoint
{
    public static void MapGetProfileByIdEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/profiles/{userId:guid}", async (Guid userId, IMediator mediator, CancellationToken ct) =>
        {
            var profile = await mediator.Send(new GetProfileByIdQuery(userId), ct);
            return profile is not null ? Results.Ok(profile) : Results.NotFound();
        });
    }
}