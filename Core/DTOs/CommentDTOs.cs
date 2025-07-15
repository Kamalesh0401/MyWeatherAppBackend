namespace WeatherApp.Api.Core.DTOs
{
    public class CommentResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string LocationName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateCommentRequest
    {
        public string LocationName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

    public class UpdateCommentRequest
    {
        public string Content { get; set; } = string.Empty;
    }

    public class GetCommentsRequest
    {
        public string LocationName { get; set; } = string.Empty;
        public int Limit { get; set; } = 50;
        public int Offset { get; set; } = 0;
    }
}