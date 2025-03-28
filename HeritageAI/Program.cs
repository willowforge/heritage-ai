using Google.Cloud.Speech.V2;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using NAudio.Wave;

namespace HeritageAI;

class Program
{
    static async Task Main(string[] args)
    {
        //Debug.Info($"GOOGLE_APPLICATION_CREDENTIALS = {Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS")}");
        
        
        var recorder = new WaveRecorder();
        Console.WriteLine("Press 'F4' to start recording. Press 'Q' to quit\n");
        
        while (true)
        {
            var key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.F4)
            {
                recorder.StartRecording();
                Console.WriteLine("Recording started\nPress 'P' to stop recording\n");
            }
            else if (key == ConsoleKey.P)
            {
                recorder.StopRecording();
                Debug.Info("Recording stopped. Transcribing...\n");

                string transcription = await SpeechToText.Convert(recorder.OutputFilePath);
                Debug.Info($"Transcription: \n{transcription}\n");
                
                string response = await OpenAIChat.GetAIResponse(transcription);
                Debug.Info($"Response: \n{response}\n");

                string outputPath = Path.Combine("speech", "teddy_response.wav");
                Directory.CreateDirectory("speech");
                
                string audioPath = await TextToSpeech.GenerateSpeechAsync(response, outputPath);
                Debug.Info($"Audio file created at: \n{audioPath}\n");

                if (!string.IsNullOrEmpty(audioPath))
                {
                    Debug.Info("Playing audio...\n");
                    using var audioFile = new AudioFileReader(audioPath);
                    using var outputDevice = new WaveOutEvent();
                    outputDevice.Init(audioFile);
                    outputDevice.Play();

                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        await Task.Delay(100);
                    }
                    
                    Debug.Info("Playback finished.\n");
                }
                
                Console.WriteLine("Press 'F4' to start recording, or press 'Q' to quit\n");
            }
            else if (key == ConsoleKey.Q)
            {
                break;
            }
        }
    }
}