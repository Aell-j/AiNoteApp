namespace LocalAiApi.Models
{
    public class ChatRequest 
    {
        public string Message { get; set; } = string.Empty;
        public string Prompt { get; set; } = "analyse";
    }
}
