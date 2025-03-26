using NAudio.Wave;

namespace HeritageAI;

class WaveRecorder
{
    private WaveInEvent waveIn;
    private WaveFileWriter waveFileWriter;
    private string outputFilePath;
    public string OutputFilePath => outputFilePath;
    
    public void StartRecording()
    {
        // Manually construct the target directory for a more predictable output path across dev environments
        string projectDir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
        string targetDir = Path.Combine(projectDir, "waves");

        if (!Directory.Exists(targetDir))
        {
            Directory.CreateDirectory(targetDir);
        }
        
        waveIn = new WaveInEvent();
        waveIn.WaveFormat = new WaveFormat(44100, 1); // Standard mono audio format and file size
        
        outputFilePath = Path.Combine(targetDir, "audio.wav");
        
        waveFileWriter = new WaveFileWriter(outputFilePath, waveIn.WaveFormat);
        
        // Buffer the audio data as it's captured to avoid memory overload
        waveIn.DataAvailable += (s, e) =>
        {
            waveFileWriter.Write(e.Buffer, 0, e.BytesRecorded);
        };

        // Dispose of resources immediately when recording stops to avoid locking the file
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