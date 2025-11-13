<<<<<<< Updated upstream
<<<<<<< Updated upstream
﻿using System.Security.Claims;
using MediatR;
=======
using MediatR;
=======
using MediatR;

namespace RoomMateFinder.Features.Profiles.GetMyProfile;
>>>>>>> Stashed changes

namespace RoomMateFinder.Features.Profiles.GetMyProfile;
>>>>>>> Stashed changes

namespace RoomMateFinder.Features.Profiles.GetMyProfile;

public static class GetMyProfileEndpoint
{
    public static void MapGetMyProfileEndpoint(this IEndpointRouteBuilder app)
    {
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        app.MapGet("/profiles/me", async (ClaimsPrincipal user, IMediator mediator, CancellationToken ct) =>
            {
                var idStr = user.FindFirstValue("sub") ?? user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!Guid.TryParse(idStr, out var userId))
                    return Results.Unauthorized();

                var profile = await mediator.Send(new GetMyProfileQuery(userId), ct);
                return profile is not null ? Results.Ok(profile) : Results.NotFound();
            })
            .RequireAuthorization();
=======
=======
>>>>>>> Stashed changes
        app.MapGet("/profiles/me", async (IMediator mediator, CancellationToken ct) =>
        {
            // TEMP user Id until JWT auth is added
            var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");

            var profile = await mediator.Send(new GetProfileQuery(userId), ct);

            return profile is not null
                ? Results.Ok(profile)
                : Results.NotFound();
        });
>>>>>>> Stashed changes
    }
}