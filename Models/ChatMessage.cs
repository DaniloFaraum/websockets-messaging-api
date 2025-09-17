using System.ComponentModel.DataAnnotations;
using System.Net.WebSockets;
using System.Text.Json.Serialization;

namespace WebKafka.Models
{
    public class ChatMessage
    {
        [JsonPropertyName("user")]
        [Required(ErrorMessage = "No user specified.")]
        public string User { get; set; }
        [JsonPropertyName("message")]
        [Required(ErrorMessage = "Message can't be empty.")]
        public string Message { get; set; }
    }
}
