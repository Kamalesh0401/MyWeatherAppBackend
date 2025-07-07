using Microsoft.AspNetCore.SignalR;
using WeatherApp.Api.Services;

namespace WeatherApp.Api.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ChatService _chatService;

        public ChatHub(ChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task SendMessage(string username, string message)
        {
            // In a real app, you'd validate the user and get userId from context
            // For demo purposes, we'll use a simple approach
            var userId = Context.Items["UserId"] as int? ?? 0;

            var chatMessage = await _chatService.SendMessageAsync(userId, username, message);
            if (chatMessage != null)
            {
                await Clients.All.SendAsync("ReceiveMessage", chatMessage);
            }
        }

        public async Task JoinChat(string username)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "ChatRoom");
            await Clients.Group("ChatRoom").SendAsync("UserJoined", username);
        }

        public async Task LeaveChat(string username)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "ChatRoom");
            await Clients.Group("ChatRoom").SendAsync("UserLeft", username);
        }
    }
}