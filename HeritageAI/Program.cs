namespace HeritageAI;

using System;

class Program
{
    static void Main(string[] args)
    {
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
            }
            else if (key == 'q')
            {
                break;
            }
        }
    }
}