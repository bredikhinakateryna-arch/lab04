using System.Text;
using HuffmanCoding.DataStructures;
using HuffmanCoding.Models;

namespace HuffmanCoding.UI;

public static class Output
{
    private static readonly ConsoleColor HeaderColor = ConsoleColor.Cyan;
    private static readonly ConsoleColor SuccessColor = ConsoleColor.Green;
    private static readonly ConsoleColor ErrorColor = ConsoleColor.Red;
    private static readonly ConsoleColor InfoColor = ConsoleColor.Yellow;
    private static readonly ConsoleColor DataColor = ConsoleColor.White;

    public static void ShowHeader()
    {
        Console.Clear();
        WriteColoredLine("╔════════════════════════════════════════════════════════╗", HeaderColor);
        WriteColoredLine("║           HUFFMAN CODING - TEXT COMPRESSION            ║", HeaderColor);
        WriteColoredLine("╚════════════════════════════════════════════════════════╝", HeaderColor);
        Console.WriteLine();
    }
    
    public static void ShowHuffmanCodes(Dictionary<char, string> codes)
    {
        Console.WriteLine();
        WriteColoredLine("╔══════════════════ HUFFMAN CODES TABLE ════════════════════╗", HeaderColor);
        WriteColoredLine("║ Character │      Code       │  Length  │    Binary        ║", HeaderColor);
        WriteColoredLine("╠═══════════╪═════════════════╪══════════╪══════════════════╣", HeaderColor);

        var sortedCodes = codes.OrderBy(x => x.Value.Length).ThenBy(x => x.Key);

        foreach (var kvp in sortedCodes)
        {
            var c = kvp.Key;
            var code = kvp.Value;
            var displayChar = GetDisplayChar(c);

            Console.Write("║ ");
            WriteColored($"{displayChar,-9}", DataColor);
            Console.Write(" │ ");
            WriteColored($"{code,-15}", SuccessColor);
            Console.Write(" │ ");
            WriteColored($"{code.Length,-8}", InfoColor);
            Console.Write(" │ ");
            WriteColored($"{Convert.ToString(c, 2).PadLeft(8, '0'),-16}", DataColor);
            Console.WriteLine(" ║");
        }

        WriteColoredLine("╚═══════════════════════════════════════════════════════════╝", HeaderColor);
        WriteColoredLine($"\nTotal unique characters: {codes.Count}", InfoColor);
    }

    public static void ShowFrequencyTable(Dictionary<char, int> frequencies)
    {
        Console.WriteLine();
        WriteColoredLine("╔══════════════════ FREQUENCY TABLE ═══════════════════╗", HeaderColor);
        WriteColoredLine("║ Character │  Frequency  │   Percentage               ║", HeaderColor);
        WriteColoredLine("╠═══════════╪═════════════╪════════════════════════════╣", HeaderColor);

        var total = frequencies.Values.Sum();
        var sortedFreqs = frequencies.OrderByDescending(x => x.Value).Take(15);

        foreach (var kvp in sortedFreqs)
        {
            var c = kvp.Key;
            var freq = kvp.Value;
            var percent = (double)freq / total * 100;
            var displayChar = GetDisplayChar(c);

            Console.Write("║ ");
            WriteColored($"{displayChar,-9}", DataColor);
            Console.Write(" │ ");
            WriteColored($"{freq,11}", InfoColor);
            Console.Write(" │ ");
            WriteColored($"{percent,6:F2}%", SuccessColor);
            Console.Write(" ");
            DrawBar(percent, 15);
            Console.WriteLine("  ║");
        }

        WriteColoredLine("╚══════════════════════════════════════════════════════╝", HeaderColor);
        if (frequencies.Count > 15)
        {
            WriteColoredLine($"(Showing top 15 of {frequencies.Count} characters)", InfoColor);
        }
    }

    public static void ShowHuffmanTree(HuffmanNode? root)
    {
        if (root == null)
        {
            WriteColoredLine("Huffman tree not built.", ErrorColor);
            return;
        }

        Console.WriteLine();
        WriteColoredLine("╔══════════════════ HUFFMAN TREE ══════════════════╗", HeaderColor);
        Console.WriteLine();

        PrintTree(root, "", true);

        WriteColoredLine("\n╚══════════════════════════════════════════════════╝", HeaderColor);
    }

    private static void PrintTree(HuffmanNode node, string indent, bool last)
    {
        Console.Write(indent);
        if (last)
        {
            Console.Write("└─");
            indent += "  ";
        }
        else
        {
            Console.Write("├─");
            indent += "│ ";
        }

        if (node.IsLeaf)
        {
            WriteColored($"'{GetDisplayChar(node.Character!.Value)}' ", SuccessColor);
            WriteColored($"({node.Frequency})\n", InfoColor);
        }
        else
        {
            WriteColored($"[{node.Frequency}]\n", DataColor);

            if (node.Left != null)
                PrintTree(node.Left, indent, node.Right == null);
            if (node.Right != null)
                PrintTree(node.Right, indent, true);
        }
    }

    public static void ShowComparison(List<(string Name, CompressionResult Result)> results)
    {
        Console.WriteLine();
        WriteColoredLine("╔═══════════════════════════════════════ COMPARISON ═══════════════════════════════════╗", HeaderColor);
        WriteColoredLine("║  File                   │  Original  │  Compressed │  Ratio  │  Savings │  Status    ║", HeaderColor);
        WriteColoredLine("╠═════════════════════════╪════════════╪═════════════╪═════════╪══════════╪════════════╣", HeaderColor);

        foreach (var (name, result) in results)
        {
            var passed = result.CompressionRatio >= result.OriginalSize / (double)result.CompressedSize * 0.9;
            var status = passed ? "✓ PASS" : "✗ FAIL";
            var statusColor = passed ? SuccessColor : ErrorColor;

            Console.Write("║ ");
            WriteColored($"{TruncateText(name, 23),-23}", DataColor);
            Console.Write(" │ ");
            WriteColored($"{FormatBytes(result.OriginalSize),10}", InfoColor);
            Console.Write(" │ ");
            WriteColored($"{FormatBytes(result.CompressedSize),11}", InfoColor);
            Console.Write(" │ ");
            WriteColored($"{result.CompressionRatio,7:F2}x", SuccessColor);
            Console.Write(" │ ");
            WriteColored($"{result.SpaceSavings,7:F2}%", SuccessColor);
            Console.Write(" │ ");
            WriteColored($"{status,-9}", statusColor);
            Console.WriteLine(" ║");
        }

        WriteColoredLine("╚══════════════════════════════════════════════════════════════════════════════════════╝", HeaderColor);
    }

    public static void ShowSuccess(string message)
    {
        Console.WriteLine();
        WriteColoredLine($"✓ {message}", SuccessColor);
    }

    public static void ShowError(string message)
    {
        Console.WriteLine();
        WriteColoredLine($"✗ Error: {message}", ErrorColor);
    }

    public static void ShowInfo(string message)
    {
        WriteColoredLine($"ℹ {message}", InfoColor);
    }

    public static void ShowTestResult(string testName, bool passed, string message = "")
    {
        Console.Write("  ");
        if (passed)
        {
            WriteColored("✓ PASS", SuccessColor);
        }
        else
        {
            WriteColored("✗ FAIL", ErrorColor);
        }
        Console.Write($" - {testName}");
        if (!string.IsNullOrEmpty(message))
        {
            Console.Write($": {message}");
        }
        Console.WriteLine();
    }

    public static void ShowSection(string title)
    {
        Console.WriteLine();
        WriteColoredLine($"═══ {title} ═══", HeaderColor);
        Console.WriteLine();
    }

    private static void DrawBar(double percent, int width)
    {
        var filled = (int)(percent / 100 * width);
        Console.Write("[");
        WriteColored(new string('█', filled), SuccessColor);
        Console.Write(new string('░', width - filled));
        Console.Write("]");
    }

    private static string GetDisplayChar(char c)
    {
        return c switch
        {
            '\n' => "\\n",
            '\r' => "\\r",
            '\t' => "\\t",
            ' ' => "SPACE",
            _ => c.ToString()
        };
    }

    private static string TruncateText(string text, int maxLength)
    {
        if (text.Length <= maxLength)
            return text;

        return text.Substring(0, maxLength - 3) + "...";
    }

    private static string FormatBytes(long bytes)
    {
        if (bytes < 1024)
            return $"{bytes} B";
        if (bytes < 1024 * 1024)
            return $"{bytes / 1024.0:F1} KB";
        return $"{bytes / (1024.0 * 1024.0):F1} MB";
    }

    private static void WriteColored(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ResetColor();
    }

    private static void WriteColoredLine(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }
}
