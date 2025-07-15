using WeatherApp.Api.Core.Models;

namespace WeatherApp.Api.Core.Interfaces.Repositories
{
    public interface ICommentRepository
    {
        Task<Comment?> GetByIdAsync(int id);
        Task<List<Comment>> GetByLocationAsync(string location, int limit = 50, int offset = 0);
        Task<List<Comment>> GetByUserIdAsync(int userId, int limit = 50, int offset = 0);
        Task<Comment> CreateAsync(Comment comment);
        Task<Comment> UpdateAsync(Comment comment);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<int> GetCountByLocationAsync(string location);
    }
}