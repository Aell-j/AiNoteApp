using System.Text.Json.Serialization;

namespace LocalAiApi.Models
{
    public class OllamaRespose
    {
        [JsonPropertyName("message")]
        public OllamaMessage Message { get; set; } = new();
    }
}
