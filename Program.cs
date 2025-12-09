using System.Diagnostics;
using HuffmanCoding.Implementations;
using HuffmanCoding.Models;
using HuffmanCoding.UI;

namespace HuffmanCoding;

class Program
{
    static void Main(string[] args)
    {
        Output.ShowHeader();
        RunAllTests();
        Console.WriteLine();
        Output.ShowInfo("All tests completed!");
        Console.WriteLine();
    }

    private static void RunAllTests()
    {
        List<(string Name, CompressionResult Result)> results = new List<(string, CompressionResult)>();

        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var testFilesDir = Path.Combine(baseDir, "TestFiles");

        Output.ShowSection("TEST 1: Compressing sample1.txt (Algorithmic text)");
        var result1 = TestFile(Path.Combine(testFilesDir, "sample1.txt"), expectedMinRatio: 1.5);
        if (result1 != null) results.Add(("sample1.txt", result1));

        Output.ShowSection("TEST 2: Compressing sample2.txt (Repeated characters)");
        var result2 = TestFile(Path.Combine(testFilesDir, "sample2.txt"), expectedMinRatio: 1.6);
        if (result2 != null) results.Add(("sample2.txt", result2));

        Output.ShowSection("TEST 3: Compressing sample3.txt (English text)");
        var result3 = TestFile(Path.Combine(testFilesDir, "sample3.txt"), expectedMinRatio: 1.5);
        if (result3 != null) results.Add(("sample3.txt", result3));

        Output.ShowSection("TEST 4: Compressing sample4.txt (Uniform distribution)");
        var result4 = TestFile(Path.Combine(testFilesDir, "sample4.txt"), expectedMinRatio: 1.2);
        if (result4 != null) results.Add(("sample4.txt", result4));

        Output.ShowSection("SUMMARY: Compression Results for All Files");
        Output.ShowComparison(results);
    }

    private static CompressionResult? TestFile(string filePath, double expectedMinRatio)
    {
        try
        {
            if (!FileProcessor.FileExists(filePath))
            {
                Output.ShowError($"File not found: {filePath}");
                return null;
            }

            var text = FileProcessor.ReadTextFile(filePath);
            Output.ShowInfo($"Loaded file: {FileProcessor.GetFileName(filePath)} ({text.Length} characters)");

            ICompressor compressor = new HuffmanCompressor();

            var sw = Stopwatch.StartNew();
            var compressedText = compressor.Compress(text);
            sw.Stop();

            var result = new CompressionResult
            {
                OriginalSize = System.Text.Encoding.UTF8.GetByteCount(text),
                CompressedSize = compressedText.Length / 8,
                CompressionTimeMs = sw.ElapsedMilliseconds,
                UniqueCharacters = compressor.GetFrequencyTable().Count,
                OriginalFileName = FileProcessor.GetFileName(filePath)
            };

            Output.ShowInfo("Verifying decompression...");
            var decompressed = compressor.Decompress(compressedText);
            var decompressCorrect = decompressed == text;
            Output.ShowTestResult("Decompression correctness", decompressCorrect,
                decompressCorrect ? "Original text restored perfectly" : "Decompressed text does not match original");

            var ratioOk = result.CompressionRatio >= expectedMinRatio;
            Output.ShowTestResult("Compression ratio check", ratioOk,
                $"Expected >= {expectedMinRatio:F2}x, Got {result.CompressionRatio:F2}x");

            Console.WriteLine();
            Output.ShowInfo("Top 10 character frequencies:");
            var topFreqs = compressor.GetFrequencyTable()
                .OrderByDescending(x => x.Value)
                .Take(10)
                .ToDictionary(x => x.Key, x => x.Value);
            Output.ShowFrequencyTable(topFreqs);

            Console.WriteLine();
            Output.ShowInfo("Huffman codes (showing first 10):");
            var codes = compressor.GetHuffmanCodes()
                .OrderBy(x => x.Value.Length)
                .Take(10)
                .ToDictionary(x => x.Key, x => x.Value);
            Output.ShowHuffmanCodes(codes);

            return result;
        }
        catch (Exception ex)
        {
            Output.ShowError($"Test failed: {ex.Message}");
            return null;
        }
    }
}
