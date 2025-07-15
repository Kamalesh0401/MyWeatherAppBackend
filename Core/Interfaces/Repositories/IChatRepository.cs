using WeatherApp.Api.Core.Models;

namespace WeatherApp.Api.Core.Interfaces.Repositories
{
    public interface IChatRepository
    {
        Task<ChatMessage?> GetByIdAsync(int id);
        Task<List<ChatMessage>> GetRecentMessagesAsync(int limit = 50, int offset = 0);
        Task<List<ChatMessage>> GetMessagesByUserIdAsync(int userId, int limit = 50, int offset = 0);
        Task<ChatMessage> CreateAsync(ChatMessage message);
        Task<ChatMessage> UpdateAsync(ChatMessage message);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<int> GetTotalMessageCountAsync();
    }
}