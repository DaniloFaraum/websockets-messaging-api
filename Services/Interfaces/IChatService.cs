using System.Net.WebSockets;

namespace WebKafka.Services.Interfaces
{
    public interface IChatService
    {
        Task HandleChatConnection(WebSocket webSocket);
        Task BroadcastMessageAsync(string message);
    }
}
