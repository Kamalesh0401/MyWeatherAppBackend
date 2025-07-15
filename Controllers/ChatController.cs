using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherApp.Api.Core.DTOs;
using WeatherApp.Api.Core.Interfaces.Services;
using System.Security.Claims;

namespace WeatherApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet("messages")]
        public async Task<ActionResult<List<ChatMessageResponse>>> GetRecentMessages(
            [FromQuery] int limit = 50,
            [FromQuery] int offset = 0)
        {
            var messages = await _chatService.GetRecentMessagesAsync(limit, offset);
            return Ok(messages);
        }

        [HttpGet("messages/count")]
        public async Task<ActionResult<int>> GetTotalMessageCount()
        {
            var count = await _chatService.GetTotalMessageCountAsync();
            return Ok(new { count });
        }

        [HttpGet("messages/user")]
        public async Task<ActionResult<List<ChatMessageResponse>>> GetMessagesByUser(
            [FromQuery] int limit = 50,
            [FromQuery] int offset = 0)
        {
            var userId = GetCurrentUserId();
            var messages = await _chatService.GetMessagesByUserAsync(userId, limit, offset);
            return Ok(messages);
        }

        [HttpPost("messages")]
        public async Task<ActionResult<ChatMessageResponse>> SendMessage(SendMessageRequest request)
        {
            var userId = GetCurrentUserId();
            var username = GetCurrentUsername();

            var message = await _chatService.SendMessageAsync(userId, username, request.Message);
            if (message == null)
                return BadRequest("Failed to send message");

            return CreatedAtAction(nameof(GetRecentMessages), message);
        }

        [HttpDelete("messages/{messageId}")]
        public async Task<ActionResult> DeleteMessage(int messageId)
        {
            var userId = GetCurrentUserId();
            var success = await _chatService.DeleteMessageAsync(messageId, userId);

            if (!success)
                return NotFound();

            return Ok(new { message = "Message deleted successfully" });
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim.Value);
        }

        private string GetCurrentUsername()
        {
            var usernameClaim = User.FindFirst(ClaimTypes.Name);
            return usernameClaim.Value;
        }
    }
}