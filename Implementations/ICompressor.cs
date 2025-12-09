using HuffmanCoding.Models;

namespace HuffmanCoding.Implementations;

public interface ICompressor
{
    string Compress(string text);
    string Decompress(string compressedText);
    Dictionary<char, string> GetHuffmanCodes();
    Dictionary<char, int> GetFrequencyTable();
    DataStructures.HuffmanNode? GetHuffmanTree();
    void Reset();
}
