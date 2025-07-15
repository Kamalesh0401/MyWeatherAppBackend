namespace WeatherApp.Api.Core.DTOs
{
    public class ChatMessageResponse
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class SendMessageRequest
    {
        public string Message { get; set; } = string.Empty;
    }

    public class GetMessagesRequest
    {
        public int Limit { get; set; } = 50;
        public int Offset { get; set; } = 0;
    }
}