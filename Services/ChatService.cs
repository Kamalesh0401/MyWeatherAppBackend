using WeatherApp.Api.DTOs;
using WeatherApp.Api.Models;

namespace WeatherApp.Api.Services
{
    public class ChatService
    {
        private readonly DatabaseService _databaseService;

        public ChatService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<List<ChatMessageResponse>> GetRecentMessagesAsync()
        {
            var messages = await _databaseService.GetRecentChatMessagesAsync();
            return messages.Select(m => new ChatMessageResponse
            {
                Id = m.Id,
                Username = m.Username,
                Message = m.Message,
                CreatedAt = m.CreatedAt
            }).ToList();
        }

        public async Task<ChatMessageResponse?> SendMessageAsync(int userId, string username, string message)
        {
            var chatMessage = new ChatMessage
            {
                UserId = userId,
                Username = username,
                Message = message,
                CreatedAt = DateTime.UtcNow
            };

            var createdMessage = await _databaseService.CreateChatMessageAsync(chatMessage);
            if (createdMessage == null) return null;

            return new ChatMessageResponse
            {
                Id = createdMessage.Id,
                Username = createdMessage.Username,
                Message = createdMessage.Message,
                CreatedAt = createdMessage.CreatedAt
            };
        }
    }
}
