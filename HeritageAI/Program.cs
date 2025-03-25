using Google.Cloud.Speech.V2;

namespace HeritageAI;

using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;

class Program
{
    static async Task Main(string[] args)
    {
        //Console.WriteLine($"[DEBUG] GOOGLE_APPLICATION_CREDENTIALS = {Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS")}");
        
        
        var recorder = new WaveRecorder();
        Console.WriteLine("Press 'r' to start recording");
        
        while (true)
        {
            var key = Console.ReadKey().KeyChar;
            if (key == 'r')
            {
                recorder.StartRecording();
                Console.WriteLine("Press 's' to stop recording");
            }
            else if (key == 's')
            {
                recorder.StopRecording();
                Console.WriteLine("Recording stopped. Transcribing...");

                string transcription = await SpeechToText.Convert(recorder.OutputFilePath);
                Console.WriteLine($"Transcription: {transcription}");
            }
            else if (key == 'q')
            {
                break;
            }
        }
    }
}