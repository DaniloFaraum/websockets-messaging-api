using System.Net.WebSockets;
using WebKafka.Models;

namespace WebKafka.Aplication
{
    public interface IMessageProcessor
    {
        Task<string> ProcessMessageAsync(Guid socketId, WebSocket socket, string rawMessage);
    }
}
