using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace RoomMateFinder.Features.Profiles.SearchProfiles;

public static class SearchProfilesEndpoint
{
    public static void MapSearchProfilesEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/profiles/search", async (
            [AsParameters] SearchProfilesRequest req,
            IMediator mediator) =>
        {
            var result = await mediator.Send(req);
            return Results.Ok(result);
        });
    }
}