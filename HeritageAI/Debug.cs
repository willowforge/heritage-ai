using System;
using System.IO;
using Google.Protobuf;

namespace HeritageAI;

public static class Debug
{
    private static readonly string logFilePath = Path.Combine("logs", $"log_{DateTime.Now:yyyy-MM-dd}.txt");

    static Debug()
    {
        Directory.CreateDirectory("logs");
    }

    public static void Info(string message)
    {
        Write("[INFO] ", message);
    }

    public static void Error(string message)
    {
        Write("[ERROR] ", message);
    }

    public static void Warning(string message)
    {
        Write("[WARNING] ", message);
    }

    public static void Write(string level, string message)
    {
        string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {level}: {message}";
        Console.WriteLine(logMessage);
        File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
    }
}