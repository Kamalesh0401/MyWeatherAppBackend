using WeatherApp.Api.Core.DTOs;

namespace WeatherApp.Api.Core.Interfaces.Services
{
    public interface IChatService
    {
        Task<List<ChatMessageResponse>> GetRecentMessagesAsync(int limit = 50, int offset = 0);
        Task<ChatMessageResponse?> SendMessageAsync(int userId, string username, string message);
        Task<List<ChatMessageResponse>> GetMessagesByUserAsync(int userId, int limit = 50, int offset = 0);
        Task<bool> DeleteMessageAsync(int messageId, int userId);
        Task<int> GetTotalMessageCountAsync();
    }
}