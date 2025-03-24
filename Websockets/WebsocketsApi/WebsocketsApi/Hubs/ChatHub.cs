using Microsoft.AspNetCore.SignalR;

namespace WebsocketsApi.Hubs
{
    public class ChatHub:Hub
    {
        public async Task SendMessage(string connectionId, string message)
        {
            await Clients.Client(connectionId).SendAsync("ReceiveMessage", message, Context.ConnectionId);
        }
    }
}
