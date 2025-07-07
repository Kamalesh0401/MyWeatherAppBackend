using Microsoft.AspNetCore.Mvc;
using WeatherApp.Api.DTOs;
using WeatherApp.Api.Models;
using WeatherApp.Api.Services;

namespace WeatherApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly DatabaseService _databaseService;

        public CommentsController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        [HttpGet("{location}")]
        public async Task<IActionResult> GetComments(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                return BadRequest("Location is required");
            }

            var comments = await _databaseService.GetCommentsByLocationAsync(location);
            var commentResponses = comments.Select(c => new CommentResponse
            {
                Id = c.Id,
                Username = c.Username,
                LocationName = c.LocationName,
                Content = c.Content,
                CreatedAt = c.CreatedAt
            }).ToList();

            return Ok(commentResponses);
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CommentRequest request)
        {
            var userId = HttpContext.Items["UserId"] as int?;
            var username = HttpContext.Items["Username"] as string;

            if (userId == null || username == null)
            {
                return Unauthorized(new { message = "Authentication required" });
            }

            if (string.IsNullOrWhiteSpace(request.LocationName) || string.IsNullOrWhiteSpace(request.Content))
            {
                return BadRequest("Location and content are required");
            }

            var comment = new Comment
            {
                UserId = userId.Value,
                Username = username,
                LocationName = request.LocationName,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow
            };

            var createdComment = await _databaseService.CreateCommentAsync(comment);
            if (createdComment == null)
            {
                return StatusCode(500, "Failed to create comment");
            }

            var response = new CommentResponse
            {
                Id = createdComment.Id,
                Username = createdComment.Username,
                LocationName = createdComment.LocationName,
                Content = createdComment.Content,
                CreatedAt = createdComment.CreatedAt
            };

            return CreatedAtAction(nameof(GetComments), new { location = request.LocationName }, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var userId = HttpContext.Items["UserId"] as int?;
            if (userId == null)
            {
                return Unauthorized(new { message = "Authentication required" });
            }

            // In a real app, you'd verify the user owns this comment before deleting
            // For demo purposes, we'll allow any authenticated user to delete
            return Ok(new { message = "Comment deletion not implemented in this demo" });
        }
    }
}