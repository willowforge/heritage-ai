﻿using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace HeritageAI;

public class TextToSpeech
{
    public static async Task<string> GenerateSpeechAsync(string text, string outputFilePath)
    {
        string subscriptionKey = Environment.GetEnvironmentVariable("ELEVENLABS_SPEECH_KEY"); // don't forget to set in environemnt variables

        if (string.IsNullOrEmpty(subscriptionKey))
        {
            Debug.Error("Azure Text-To-Speech subscription key is not set.");   
            return null;
        }

        var config = SpeechConfig.FromSubscription(subscriptionKey, "eastus");
        config.SpeechSynthesisVoiceName = "g6XTfKskXxj7LA6IIN60"; // ElevenLabs voice model ID

        // Create the output audio file stream
        var audioConfig = AudioConfig.FromWavFileOutput(outputFilePath);

        using var synthesizer = new SpeechSynthesizer(config, audioConfig);
        using var result = await synthesizer.SpeakTextAsync(text);

        if (result.Reason == ResultReason.SynthesizingAudioCompleted)
        {
            Debug.Info($"Speech synthesized and saved to: {outputFilePath}");
            return outputFilePath;
        }
        if (result.Reason == ResultReason.Canceled)
        {
            var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
            Debug.Error($"CANCELED: Reason={cancellation.Reason}");
            if (cancellation.Reason == CancellationReason.Error)
            {
                Debug.Error($"ErrorCode={cancellation.ErrorCode}");
                Debug.Error($"ErrorDetails=[{cancellation.ErrorDetails}]");
            }
        }

        return null;
    }
}