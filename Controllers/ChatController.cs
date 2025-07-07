using Microsoft.AspNetCore.Mvc;
using WeatherApp.Api.DTOs;
using WeatherApp.Api.Services;

namespace WeatherApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly ChatService _chatService;

        public ChatController(ChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet("messages")]
        public async Task<IActionResult> GetMessages()
        {
            var messages = await _chatService.GetRecentMessagesAsync();
            return Ok(messages);
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessageRequest request)
        {
            var userId = HttpContext.Items["UserId"] as int?;
            var username = HttpContext.Items["Username"] as string;

            if (userId == null || username == null)
            {
                return Unauthorized(new { message = "Authentication required" });
            }

            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest("Message is required");
            }

            var message = await _chatService.SendMessageAsync(userId.Value, username, request.Message);
            if (message == null)
            {
                return StatusCode(500, "Failed to send message");
            }

            return Ok(message);
        }
    }
}