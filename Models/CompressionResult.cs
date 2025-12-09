namespace HuffmanCoding.Models;

public class CompressionResult
{
    public long OriginalSize { get; set; }
    public long CompressedSize { get; set; }
    public double CompressionRatio => OriginalSize > 0 ? (double)OriginalSize / CompressedSize : 0;
    public double SpaceSavings => OriginalSize > 0 ? (1 - (double)CompressedSize / OriginalSize) * 100 : 0;
    public long CompressionTimeMs { get; set; }
    public int UniqueCharacters { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
}
