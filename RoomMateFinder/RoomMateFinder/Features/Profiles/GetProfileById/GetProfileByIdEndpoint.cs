using MediatR;

namespace RoomMateFinder.Features.Profiles.GetProfileById;

public static class GetProfileByIdEndpoint
{
    public static void MapGetProfileByIdEndpoint(this IEndpointRouteBuilder app)
    {
<<<<<<< HEAD
        app.MapGet("/profiles/{userId:guid}", async (Guid userId, IMediator mediator) =>
        {
            var profile = await mediator.Send(new GetProfileByIdQuery(userId));
            return Results.Ok(profile);
        });
=======
        app.MapGet("/profiles/{id:guid}", async (Guid id, IMediator mediator) =>
            {
                var profile = await mediator.Send(new GetProfileByIdQuery(id));

                return profile is not null
                    ? Results.Ok(profile)
                    : Results.NotFound();
            })
            .RequireAuthorization();
>>>>>>> DariusBranch
    }
}