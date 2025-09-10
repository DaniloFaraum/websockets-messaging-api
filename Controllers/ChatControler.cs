using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using WebKafka.Services;
using WebKafka.Services.Interfaces;

namespace WebKafka.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController: ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }
        [HttpGet]
        public async Task HandleChat()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

                await _chatService.HandleChatConnection(webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
            }
        }
    }
}
