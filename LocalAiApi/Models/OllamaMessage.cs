using System.Text.Json.Serialization;

namespace LocalAiApi.Models
{
    public class OllamaMessage
    {
        [JsonPropertyName("content")]
        public string? Content { get; set; }
    }
}