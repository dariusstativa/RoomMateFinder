using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

public static class IsMatchEndpoint
{
    public static void MapIsMatchEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/matching/is-match/{otherUserId:guid}",
                async (Guid otherUserId, HttpContext http, IMediator mediator) =>
                {
                    var userIdClaim =
                        http.User.FindFirst(ClaimTypes.NameIdentifier) ??
                        http.User.FindFirst("sub");

                    if (userIdClaim is null)
                        return Results.Unauthorized();

                    var isMatch = await mediator.Send(
                        new IsMatchQuery(otherUserId));

                    return Results.Ok(isMatch);
                })
            .RequireAuthorization();
    }
}