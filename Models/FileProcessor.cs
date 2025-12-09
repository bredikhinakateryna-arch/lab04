using System.Text;

namespace HuffmanCoding.Models;

public static class FileProcessor
{
    public static string ReadTextFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }

        return File.ReadAllText(filePath, Encoding.UTF8);
    }

    public static void WriteTextFile(string filePath, string content)
    {
        File.WriteAllText(filePath, content, Encoding.UTF8);
    }

    public static long GetFileSize(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }

        return new FileInfo(filePath).Length;
    }

    public static bool FileExists(string filePath)
    {
        return File.Exists(filePath);
    }

    public static string CreateTempFile(string content, string extension = ".txt")
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"huffman_{Guid.NewGuid()}{extension}");
        WriteTextFile(tempPath, content);
        return tempPath;
    }

    public static string GetFileName(string filePath)
    {
        return Path.GetFileName(filePath);
    }

    public static string GetFileNameWithoutExtension(string filePath)
    {
        return Path.GetFileNameWithoutExtension(filePath);
    }
}
