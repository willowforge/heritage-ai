namespace HeritageAI;

using NAudio.Wave;

class WaveRecorder
{
    private WaveInEvent waveIn;
    private WaveFileWriter waveFileWriter;
    private string outputFilePath;
    
    public void StartRecording()
    {
        string projectDir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
        string targetDir = Path.Combine(projectDir, "waves");

        if (!Directory.Exists(targetDir))
        {
            Directory.CreateDirectory(targetDir);
        }
        
        outputFilePath = Path.Combine(targetDir, "audio.wav");
        
        waveIn = new WaveInEvent();
        waveIn.WaveFormat = new WaveFormat(44100, 1);
        
        waveFileWriter = new WaveFileWriter(outputFilePath, waveIn.WaveFormat);
        
        waveIn.DataAvailable += (s, e) =>
        {
            waveFileWriter.Write(e.Buffer, 0, e.BytesRecorded);
        };

        waveIn.RecordingStopped += (s, e) =>
        {
            waveFileWriter?.Dispose();
            waveFileWriter = null;
            waveIn?.Dispose();
        };
        
        waveIn.StartRecording();
        Console.WriteLine("Starting recording...");
    }

    public void StopRecording()
    {
        if (waveIn != null)
        {
            waveIn.StopRecording();
            Console.WriteLine("Stopping recording...");
            Console.WriteLine($"Audio saved to: {outputFilePath}");
        }
    }
}