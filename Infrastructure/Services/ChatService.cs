using WeatherApp.Api.Core.DTOs;
using WeatherApp.Api.Core.Interfaces.Repositories;
using WeatherApp.Api.Core.Interfaces.Services;
using WeatherApp.Api.Core.Models;

namespace WeatherApp.Api.Infrastructure.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly ILogger<ChatService> _logger;

        public ChatService(IChatRepository chatRepository, ILogger<ChatService> logger)
        {
            _chatRepository = chatRepository;
            _logger = logger;
        }

        public async Task<List<ChatMessageResponse>> GetRecentMessagesAsync(int limit = 50, int offset = 0)
        {
            try
            {
                var messages = await _chatRepository.GetRecentMessagesAsync(limit, offset);
                return messages.Select(MapToChatMessageResponse).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent messages");
                return new List<ChatMessageResponse>();
            }
        }

        public async Task<ChatMessageResponse?> SendMessageAsync(int userId, string username, string message)
        {
            try
            {
                var chatMessage = new ChatMessage
                {
                    UserId = userId,
                    Username = username,
                    Message = message,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                var createdMessage = await _chatRepository.CreateAsync(chatMessage);
                return MapToChatMessageResponse(createdMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message for user: {UserId}", userId);
                return null;
            }
        }

        public async Task<List<ChatMessageResponse>> GetMessagesByUserAsync(int userId, int limit = 50, int offset = 0)
        {
            try
            {
                var messages = await _chatRepository.GetMessagesByUserIdAsync(userId, limit, offset);
                return messages.Select(MapToChatMessageResponse).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting messages for user: {UserId}", userId);
                return new List<ChatMessageResponse>();
            }
        }

        public async Task<bool> DeleteMessageAsync(int messageId, int userId)
        {
            try
            {
                var message = await _chatRepository.GetByIdAsync(messageId);
                if (message == null || message.UserId != userId)
                {
                    return false;
                }

                return await _chatRepository.DeleteAsync(messageId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting message: {MessageId} for user: {UserId}", messageId, userId);
                return false;
            }
        }

        public async Task<int> GetTotalMessageCountAsync()
        {
            try
            {
                return await _chatRepository.GetTotalMessageCountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total message count");
                return 0;
            }
        }

        private ChatMessageResponse MapToChatMessageResponse(ChatMessage message)
        {
            return new ChatMessageResponse
            {
                Id = message.Id,
                Username = message.Username,
                Message = message.Message,
                CreatedAt = message.CreatedAt
            };
        }
    }
}