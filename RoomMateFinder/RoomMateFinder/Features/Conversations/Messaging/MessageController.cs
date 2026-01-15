using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomMateFinder.Features.Conversations.Messaging.Message;
namespace RoomMateFinder.Features.Conversations.Messaging;
[ApiController]
[Route("messages")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly IMediator _mediator;

    public MessagesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send([FromBody] SendMessageDto dto)
    {
        var senderIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                            ?? User.FindFirst("sub")?.Value;

        if (senderIdClaim is null)
            return Unauthorized("Invalid JWT");

        var command = new SendMessageCommand
        {
            SenderId = Guid.Parse(senderIdClaim),
            ConversationId = dto.ConversationId,
            Content = dto.Content
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }
}