using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HeritageAI;

public class TextToSpeech
{
    private const string VoiceId = "g6XTfKskXxj7LA6IIN60"; // set ElevenLabs voice ID here
    private const string ElevenLabsUrl = $"https://api.elevenlabs.io/v1/text-to-speech/{VoiceId}";

    public static async Task<string> GenerateSpeechAsync(string text, string outputFilePath)
    {
        string apiKey = Environment.GetEnvironmentVariable("ELEVENLABS_SPEECH_KEY"); // Don't forget to set the environment variable
        if (string.IsNullOrEmpty(apiKey))
        {
            Debug.Error("ELEVENLABS_SPEECH_KEY not set in environment variables.");
            return null;
        }

        var requestBody = new
        {
            text = text,
            model_id = "eleven_multilingual_v2",
            voice_settings = new
            {
                stability = 0.5,
                similarity_boost = 0.7
            }
        };

        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("xi-api-key", apiKey);
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("audio/mpeg"));

        var json = System.Text.Json.JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await httpClient.PostAsync(ElevenLabsUrl, content);
            response.EnsureSuccessStatusCode();

            await using var fs = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write);
            await response.Content.CopyToAsync(fs);

            Debug.Info($"Speech synthesized and saved to: {outputFilePath}");
            return outputFilePath;
        }
        catch (HttpRequestException e)
        {
            Debug.Error($"HTTP request failed: {e.Message}");
        }
        catch (Exception e)
        {
            Debug.Error($"Unexpected error during ElevenLabs TTS: {e.Message}");
        }

        return null;
    }
}
