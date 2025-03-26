using System.Text.Json.Serialization;

namespace HeritageAI;

public class ChatMessage
{
    [JsonPropertyName("role")]
    public string Role { get; set; } // "system", "user", "assistant"
    
    [JsonPropertyName("content")]
    public string Content { get; set; }
}