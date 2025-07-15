using WeatherApp.Api.Core.DTOs;
using WeatherApp.Api.Core.Interfaces.Repositories;
using WeatherApp.Api.Core.Interfaces.Services;
using WeatherApp.Api.Core.Models;

namespace WeatherApp.Api.Infrastructure.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly ILogger<CommentService> _logger;

        public CommentService(ICommentRepository commentRepository, ILogger<CommentService> logger)
        {
            _commentRepository = commentRepository;
            _logger = logger;
        }

        public async Task<List<CommentResponse>> GetCommentsByLocationAsync(string location, int limit = 50, int offset = 0)
        {
            try
            {
                var comments = await _commentRepository.GetByLocationAsync(location, limit, offset);
                return comments.Select(MapToCommentResponse).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comments for location: {Location}", location);
                return new List<CommentResponse>();
            }
        }

        public async Task<CommentResponse?> CreateCommentAsync(int userId, string username, CreateCommentRequest request)
        {
            try
            {
                var comment = new Comment
                {
                    UserId = userId,
                    Username = username,
                    LocationName = request.LocationName,
                    Content = request.Content,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                var createdComment = await _commentRepository.CreateAsync(comment);
                return MapToCommentResponse(createdComment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating comment for user: {UserId}", userId);
                return null;
            }
        }

        public async Task<CommentResponse?> UpdateCommentAsync(int commentId, int userId, UpdateCommentRequest request)
        {
            try
            {
                var comment = await _commentRepository.GetByIdAsync(commentId);
                if (comment == null || comment.UserId != userId)
                {
                    return null;
                }

                comment.Content = request.Content;
                comment.UpdatedAt = DateTime.UtcNow;

                var updatedComment = await _commentRepository.UpdateAsync(comment);
                return MapToCommentResponse(updatedComment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating comment: {CommentId} for user: {UserId}", commentId, userId);
                return null;
            }
        }

        public async Task<bool> DeleteCommentAsync(int commentId, int userId)
        {
            try
            {
                var comment = await _commentRepository.GetByIdAsync(commentId);
                if (comment == null || comment.UserId != userId)
                {
                    return false;
                }

                return await _commentRepository.DeleteAsync(commentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting comment: {CommentId} for user: {UserId}", commentId, userId);
                return false;
            }
        }

        public async Task<List<CommentResponse>> GetCommentsByUserAsync(int userId, int limit = 50, int offset = 0)
        {
            try
            {
                var comments = await _commentRepository.GetByUserIdAsync(userId, limit, offset);
                return comments.Select(MapToCommentResponse).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comments for user: {UserId}", userId);
                return new List<CommentResponse>();
            }
        }

        public async Task<int> GetCommentCountByLocationAsync(string location)
        {
            try
            {
                return await _commentRepository.GetCountByLocationAsync(location);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comment count for location: {Location}", location);
                return 0;
            }
        }

        private CommentResponse MapToCommentResponse(Comment comment)
        {
            return new CommentResponse
            {
                Id = comment.Id,
                UserId = comment.UserId,
                Username = comment.Username,
                LocationName = comment.LocationName,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt
            };
        }
    }
}