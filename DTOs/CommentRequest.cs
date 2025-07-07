namespace WeatherApp.Api.DTOs
{
    public class CommentRequest
    {
        public string LocationName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

    public class CommentResponse
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string LocationName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class ChatMessageRequest
    {
        public string Message { get; set; } = string.Empty;
    }

    public class ChatMessageResponse
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}