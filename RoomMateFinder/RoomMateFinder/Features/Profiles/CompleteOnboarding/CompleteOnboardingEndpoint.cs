using MediatR;

namespace RoomMateFinder.Features.Profiles.CompleteOnboarding;

public static class CompleteOnboardingEndpoint
{
    public static void MapCompleteOnboardingEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/profiles/{userId:guid}/onboarding", async (
            Guid userId,
            CompleteOnboardingRequest body,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var ok = await mediator.Send(new CompleteOnboardingCommand(userId, body), ct);
            return ok ? Results.NoContent() : Results.NotFound();
        });
    }
}