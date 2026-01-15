using MediatR;

public record IsMatchQuery(Guid OtherUserId) : IRequest<bool>;