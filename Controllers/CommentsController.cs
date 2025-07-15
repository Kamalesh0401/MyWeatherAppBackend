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
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet("location/{location}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<CommentResponse>>> GetCommentsByLocation(
            string location,
            [FromQuery] int limit = 50,
            [FromQuery] int offset = 0)
        {
            var comments = await _commentService.GetCommentsByLocationAsync(location, limit, offset);
            return Ok(comments);
        }

        [HttpGet("location/{location}/count")]
        [AllowAnonymous]
        public async Task<ActionResult<int>> GetCommentCountByLocation(string location)
        {
            var count = await _commentService.GetCommentCountByLocationAsync(location);
            return Ok(new { count });
        }

        [HttpGet("user")]
        public async Task<ActionResult<List<CommentResponse>>> GetCommentsByUser(
            [FromQuery] int limit = 50,
            [FromQuery] int offset = 0)
        {
            var userId = GetCurrentUserId();
            var comments = await _commentService.GetCommentsByUserAsync(userId, limit, offset);
            return Ok(comments);
        }

        [HttpPost]
        public async Task<ActionResult<CommentResponse>> CreateComment(CreateCommentRequest request)
        {
            var userId = GetCurrentUserId();
            var username = GetCurrentUsername();

            var comment = await _commentService.CreateCommentAsync(userId, username, request);
            if (comment == null)
                return BadRequest("Failed to create comment");

            return CreatedAtAction(nameof(GetCommentsByLocation),
                new { location = comment.LocationName }, comment);
        }

        [HttpPut("{commentId}")]
        public async Task<ActionResult<CommentResponse>> UpdateComment(int commentId, UpdateCommentRequest request)
        {
            var userId = GetCurrentUserId();
            var comment = await _commentService.UpdateCommentAsync(commentId, userId, request);

            if (comment == null)
                return NotFound();

            return Ok(comment);
        }

        [HttpDelete("{commentId}")]
        public async Task<ActionResult> DeleteComment(int commentId)
        {
            var userId = GetCurrentUserId();
            var success = await _commentService.DeleteCommentAsync(commentId, userId);

            if (!success)
                return NotFound();

            return Ok(new { message = "Comment deleted successfully" });
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