using MediatR;
using RoomMateFinder.Domain.Entities;

namespace RoomMateFinder.Features.Profiles.GetAllProfiles;

public static class GetAllProfilesEndpoint
{
    public static void MapGetAllProfilesEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/profiles", async (IMediator mediator, CancellationToken ct) =>
        {
            var profiles = await mediator.Send(new GetAllProfilesQuery(), ct);
            return Results.Ok(profiles);
        });
    }
}
