using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace WebKafka.Repositories.SocketsManager
{
    public class InMemorySocketManager: ISocketsManager
    {
        private readonly ConcurrentDictionary<Guid, WebSocket> _Sockets = new();
        public void TryAdd(Guid id, WebSocket socket)
        {
            _Sockets.TryAdd(id, socket);
        }
        public void TryRemove(Guid id)
        {
            _Sockets.TryRemove(id, out _);
        }
        public WebSocket? GetById(Guid id)
        {
            return _Sockets.TryGetValue(id, out WebSocket? value) ? value : null;
        }
        public List<WebSocket> GetAll()
        {
            return _Sockets.Values.ToList();
        }
    }
}
