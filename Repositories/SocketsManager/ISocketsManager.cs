using System.Net.WebSockets;

namespace WebKafka.Repositories.SocketsManager
{
    public interface ISocketsManager
    {
        void TryAdd(Guid id, WebSocket socket);
        void TryRemove(Guid id);
        WebSocket GetById(Guid id);
        List<WebSocket> GetAll();

    }
}
