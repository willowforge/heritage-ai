using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace HeritageAI;

public class TextToSpeech
{
    public static async Task<string> GenerateSpeechAsync(string text, string outputFilePath)
    {
        string subscriptionKey = Environment.GetEnvironmentVariable("AZURE_SPEECH_KEY");

        if (string.IsNullOrEmpty(subscriptionKey))
        {
            Console.Error.WriteLine("Azure Text-To-Speech subscription key is not set.");
            return null;
        }

        var config = SpeechConfig.FromSubscription(subscriptionKey, "eastus");
        config.SpeechSynthesisVoiceName = "en-US-AdamMultilingualNeural";

        // Create the output audio file stream
        var audioConfig = AudioConfig.FromWavFileOutput(outputFilePath);

        using var synthesizer = new SpeechSynthesizer(config, audioConfig);
        using var result = await synthesizer.SpeakTextAsync(text);

        if (result.Reason == ResultReason.SynthesizingAudioCompleted)
        {
            Console.WriteLine($"Speech synthesized and saved to: {outputFilePath}");
            return outputFilePath;
        }
        else if (result.Reason == ResultReason.Canceled)
        {
            var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
            Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");
            if (cancellation.Reason == CancellationReason.Error)
            {
                Console.WriteLine($"ErrorCode={cancellation.ErrorCode}");
                Console.WriteLine($"ErrorDetails=[{cancellation.ErrorDetails}]");
            }
        }

        return null;
    }
}