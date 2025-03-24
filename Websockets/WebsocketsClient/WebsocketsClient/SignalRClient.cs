using Microsoft.AspNetCore.SignalR.Client;

namespace WebsocketsClient
{
    internal class SignalRClient
    {
        private readonly HubConnection _connection;
        public string? Id { get; private set; }
        public event Action<string, string>? OnReceiveMessage;

        public SignalRClient()
        {
            _connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7008/chat")
            .Build();

            _connection.On<string, string>("ReceiveMessage", (message, sourceId) =>
            {
                string.Intern(sourceId);
                OnReceiveMessage?.Invoke(message, sourceId);
            });

            _connection.StartAsync().Wait();
            Id = _connection.ConnectionId;
        }

        public async Task SendMessageAsync(string id, string message)
        {
            await _connection.InvokeAsync("SendMessage", id, message);
        }
    }
}
