using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text.Json;
using System;
using WebKafka.Messaging.Interfaces;
using WebKafka.Models;
using System.Text;

namespace WebKafka.Aplication
{
    public class ChatMessageProcessor: IMessageProcessor
    {
        public async Task<string> ProcessMessageAsync(Guid socketId, WebSocket socket, string rawMessage)
        {
            ChatMessage incomingMessage;
            try
            {
                incomingMessage = JsonSerializer.Deserialize<ChatMessage>(rawMessage);
                if (incomingMessage == null) throw new InvalidOperationException("Payload não pode ser nulo.");
            }
            catch (JsonException ex)
            {
                await SendValidationErrorAsync(socket, new List<string> { $"JSON malformado: {ex.Message}" });
                return null!;
            }

            var validationContext = new ValidationContext(incomingMessage);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(incomingMessage, validationContext, validationResults, validateAllProperties: true);

            if (!isValid)
            {
                var errorMessages = validationResults.Select(vr => vr.ErrorMessage).ToList();
                await SendValidationErrorAsync(socket, errorMessages);
                return null!;
            }
            var chatMessage = new ChatMessage
            {
                User = incomingMessage.User,
                Message = incomingMessage.Message,
            };
            var messagePayload = JsonSerializer.Serialize(chatMessage);
            return messagePayload;
        }

        private async Task SendValidationErrorAsync(WebSocket socket, List<string> errors)
        {
            var errorMessage = new { ErrorType = "ValidationError", Errors = errors };
            var messagePayload = JsonSerializer.Serialize(errorMessage);
            var buffer = Encoding.UTF8.GetBytes(messagePayload);

            if (socket.State == WebSocketState.Open)
            {
                await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}

