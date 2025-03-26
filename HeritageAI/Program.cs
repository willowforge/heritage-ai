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
        //Console.WriteLine($"[DEBUG] GOOGLE_APPLICATION_CREDENTIALS = {Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS")}");
        
        
        var recorder = new WaveRecorder();
        Console.WriteLine("Press 'r' to start recording\n");
        
        while (true)
        {
            var key = Console.ReadKey().KeyChar;
            if (key == 'r')
            {
                recorder.StartRecording();
                Console.WriteLine("Recording started\n");
                Console.WriteLine("Press 's' to stop recording\n");
            }
            else if (key == 's')
            {
                recorder.StopRecording();
                Console.WriteLine("Recording stopped. Transcribing...\n");

                string transcription = await SpeechToText.Convert(recorder.OutputFilePath);
                Console.WriteLine($"Transcription: \n{transcription}\n");
                
                string response = await OpenAIChat.GetAIResponse(transcription);
                Console.WriteLine($"Response: \n{response}\n");

                string outputPath = Path.Combine("speech", "teddy_response.wav");
                Directory.CreateDirectory("speech");
                
                string audioPath = await TextToSpeech.GenerateSpeechAsync(response, outputPath);
                Console.WriteLine($"Audio file created at: \n{audioPath}\n");

                if (!string.IsNullOrEmpty(audioPath))
                {
                    Console.WriteLine("Playing audio...\n");
                    using var audioFile = new AudioFileReader(audioPath);
                    using var outputDevice = new WaveOutEvent();
                    outputDevice.Init(audioFile);
                    outputDevice.Play();

                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        await Task.Delay(100);
                    }
                    
                    Console.WriteLine("Playback finished.\n");
                }
                
                Console.WriteLine("Press 'r' to start recording\n");
            }
            else if (key == 'q')
            {
                break;
            }
        }
    }
}