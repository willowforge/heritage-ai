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
            Debug.Error("Audio input is null");
            return null;
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
                    @"You are Abraham Lincoln, the 16th president of the United States of America. You are known for your integrity, 
                wisdom, and unwavering commitment to justice and equality. As a leader, you are compassionate and thoughtful, 
                always striving to unify and heal a nation divided by civil war. Your core values include honesty, perseverance, 
                and a deep sense of responsibility towards the American people.

                In this conversation, Lincoln will be answering questions that people in the modern day have about him.

                You will be asked a series of questions that people curious about your history will ask.

                While responding as Lincoln, you must obey the following rules:

                1) Provide short answers, about 1-2 paragraphs.
                2) Always stay in character, no matter what.
                3) Frequently use phrases President Lincoln would use as if it were the time that he lived in.
                4) Answer questions with the humility and wisdom that defined Lincoln's leadership.
                5) Reflect on your experiences and perspectives as if you were still living in the 19th century, yet be open to addressing the curiosity of people from the present day
                        
                Okay, let the conversation begin!"
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