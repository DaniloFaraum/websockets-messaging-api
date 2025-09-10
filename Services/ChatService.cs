using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using WebKafka.Messaging.Interfaces;
using WebKafka.Services.Interfaces;

namespace WebKafka.Services
{
    public class ChatService : IChatService
    {
        private readonly ConcurrentDictionary<Guid, WebSocket> _Sockets = new();
        private readonly IEventBus _EventBus;

        public ChatService(IEventBus eventBus)
        {
            _EventBus = eventBus;
            _EventBus.Subscribe("chat-topic", async (message) =>
            {
                await BroadcastMessageAsync(message);
            });
        }

        public async Task HandleChatConnection(WebSocket webSocket)
        {
            var socketId = Guid.NewGuid();
            _Sockets.TryAdd(socketId, webSocket);

            Console.WriteLine($"WebSocket conectado: {socketId}");

            try
            {
                var buffer = new byte[1024 * 4];
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                while (!result.CloseStatus.HasValue)
                {
                    var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine($"Mensagem recebida de {socketId}: {receivedMessage}");

                    await _EventBus.PublishAsync("chat-topic", receivedMessage);

                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                }

                await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro na conexão {socketId}: {ex.Message}");
            }
            finally
            {
                _Sockets.TryRemove(socketId, out _);
            }
        }

        public async Task BroadcastMessageAsync(string message)
        {
            var messageBuffer = Encoding.UTF8.GetBytes(message);

            var tasks = _Sockets.Values.Select(socket =>
            {
                if (socket.State == WebSocketState.Open)
                {
                    return socket.SendAsync(
                        new ArraySegment<byte>(messageBuffer, 0, messageBuffer.Length),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None);
                }
                return Task.CompletedTask;
            });

            await Task.WhenAll(tasks);
        }
    }
}

