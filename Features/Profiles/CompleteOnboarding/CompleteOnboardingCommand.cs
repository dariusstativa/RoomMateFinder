using MediatR;

namespace RoomMateFinder.Features.Profiles.CompleteOnboarding;

public record CompleteOnboardingCommand(Guid UserId, CompleteOnboardingRequest Request)
    : IRequest<bool>;