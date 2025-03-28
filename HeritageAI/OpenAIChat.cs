using System.Runtime.InteropServices.ObjectiveC;
using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HeritageAI;

public class OpenAIResponse
{
    [JsonPropertyName("choices")]
    public List<OpenAIChoice> Choices { get; set; }
}

public class OpenAIChoice
{
    [JsonPropertyName("message")]
    public ChatMessage Message { get; set; }
}


public class OpenAIChat
{
    private static readonly HttpClient Client = new HttpClient();
    
    public static async Task<string> GetAIResponse(string transcription)
    {
        if (string.IsNullOrEmpty(transcription))
        {
            Debug.Error("OpenAIChat transcription is null or empty");
            return "[no input to respond to.";
        }

        string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        if (string.IsNullOrEmpty(apiKey))
        {
            Debug.Error("OpenAI API key environment variable is not set.");
            return "[Missing API key]";
        }

        var messages = new List<ChatMessage>
        {
            new ChatMessage
            {
                Role = "system",
                Content =
                    "You are Teddy Roosevelt, the 26th President of the United States of America. Respond to all input in character, using historically accurate tone and context.",
            },
            new ChatMessage
            {
                Role = "user",
                Content = transcription,
            }
        };

        var requestBody = new
        {
            model = "gpt-4o",
            messages = messages,
            temperature = 0.7
        };
        
        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        Client.DefaultRequestHeaders.Clear();
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        try
        {
            Debug.Info("Getting response from ChatGPT...");
            var response = await Client.PostAsync("https://api.openai.com/v1/chat/completions", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var chatResponse = JsonSerializer.Deserialize<OpenAIResponse>(responseJson);

            string reply = chatResponse?.Choices?.FirstOrDefault()?.Message?.Content?.Trim();

            return string.IsNullOrWhiteSpace(reply) ? "[empty AI response]" : reply;
        }
        catch (HttpRequestException ex)
        {
            Debug.Error($"HTTP request failed: {ex.Message}");
            return "[Failed to contact AI]";
        }
        catch (Exception ex)
        {
            Debug.Error($"Unexpected error: {ex.Message}");
            return "[AI Response failed]";
        }
        
        
        //string debugJson = JsonSerializer.Serialize(messages, new JsonSerializerOptions { WriteIndented = true });
        //Console.WriteLine("[DEBUG] Prepared ChatGPT Messages:\n" + debugJson);

        //return "[Placeholder]";
    }
}