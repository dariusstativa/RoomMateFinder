using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using RoomMateFinder.Features.Conversations.Messaging;
using RoomMateFinder.Features.Conversations.Messaging.Conversation;
using RoomMateFinder.Features.Conversations.Messaging.Message;
using System.Security.Claims;

namespace RoomMateFinder.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
            
            var senderId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (senderId is null)
                return Unauthorized("Invalid JWT: missing user identifier");

            var command = new SendMessageCommand
            {
                SenderId = Guid.Parse(senderId),
                ReceiverId = dto.ReceiverId,
                Content = dto.Content
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("conversation")]
        public async Task<IActionResult> GetConversation([FromQuery] Guid otherUserId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
                return Unauthorized("Invalid JWT: missing user identifier");

            var query = new GetConversationQuery
            {
                User1 = Guid.Parse(userId),
                User2 = otherUserId
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}