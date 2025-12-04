using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using RoomMateFinder.Features.Conversations.Messaging;
using RoomMateFinder.Features.Conversations.Messaging.Conversation;
using RoomMateFinder.Features.Conversations.Messaging.Message;


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
            var senderId = User.FindFirst("sub")?.Value;

            if (senderId is null)
                return Unauthorized("Invalid JWT: missing sub claim");

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
        public async Task<IActionResult> GetConversation(Guid otherUserId)
        {
            var userId = User.FindFirst("sub")?.Value;

            if (userId is null)
                return Unauthorized("Invalid JWT");

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