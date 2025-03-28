using Google.Cloud.Speech.V2;
using Google.Protobuf;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HeritageAI;

public class SpeechToText
{
    public static async Task<string> Convert(string audioFilePath)
    {
        try
        {
            if (!File.Exists(audioFilePath))
            {
                Console.Error.WriteLine($"File {audioFilePath} does not exist");
                return string.Empty;
            }
            
            var client = await SpeechClient.CreateAsync();
            var audioBytes = await File.ReadAllBytesAsync(audioFilePath);

            if (audioBytes.Length == 0)
            {
                Console.Error.WriteLine($"File {audioFilePath} is empty");
                return string.Empty;
            }
            
            string projectID = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

            if (string.IsNullOrEmpty(projectID))
            {
                Console.Error.WriteLine($"GOOGLE_PROJECT_ID environment variable is not set");
                return string.Empty;
            }
            
            var request = new RecognizeRequest
            {
                Recognizer = $"projects/{projectID}/locations/global/recognizers/heritage-ai-recognizer",
                Config = new RecognitionConfig
                {
                    AutoDecodingConfig = new AutoDetectDecodingConfig(),
                    LanguageCodes = { "en-US" },
                    Model = "latest_long",
                },
                Content = ByteString.CopyFrom(audioBytes),
            };
            
            Debug.Info($"Sending request with: {audioBytes.Length} bytes | Model: {request.Config.Model} | Language code: {request.Config.LanguageCodes}");

            var response = await client.RecognizeAsync(request);

            string transcript = string.Join(" ", response.Results
                .Select(r => r.Alternatives.FirstOrDefault()?.Transcript)
                .Where(t => !string.IsNullOrEmpty(t)));
            
            return transcript;
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("default credentials"))
        {
            Debug.Error("" +
                        "Google Cloud default credentials not found.Make sure you have set the" +
                        " GOOGLE_APPLICATION_CREDENTIALS environment variable to the path of y" +
                        "our service account key (.json file).See: https://cloud.google.com/do" +
                        "cs/authentication/external/set-up-adc");
            Debug.Error($"Details: {ex.Message}");
            return string.Empty;
        }
        catch (Exception ex)
        {
            Debug.Error("An unexpected error occurred during speech-to-text conversion.");
            Debug.Error($"Details: {ex.Message}");
            return string.Empty;
        }
    }
}