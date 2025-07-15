using WeatherApp.Api.Core.DTOs;

namespace WeatherApp.Api.Core.Interfaces.Services
{
    public interface ICommentService
    {
        Task<List<CommentResponse>> GetCommentsByLocationAsync(string location, int limit = 50, int offset = 0);
        Task<CommentResponse?> CreateCommentAsync(int userId, string username, CreateCommentRequest request);
        Task<CommentResponse?> UpdateCommentAsync(int commentId, int userId, UpdateCommentRequest request);
        Task<bool> DeleteCommentAsync(int commentId, int userId);
        Task<List<CommentResponse>> GetCommentsByUserAsync(int userId, int limit = 50, int offset = 0);
        Task<int> GetCommentCountByLocationAsync(string location);
    }
}