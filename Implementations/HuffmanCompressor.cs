using System.Text;
using HuffmanCoding.DataStructures;

namespace HuffmanCoding.Implementations;

public class HuffmanCompressor : ICompressor
{
    private Dictionary<char, int> _frequencyTable;
    private Dictionary<char, string> _huffmanCodes;
    private HuffmanNode? _root;

    public HuffmanCompressor()
    {
        _frequencyTable = new Dictionary<char, int>();
        _huffmanCodes = new Dictionary<char, string>();
        _root = null;
    }

    public string Compress(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        _frequencyTable = BuildFrequencyTable(text);
        _root = BuildHuffmanTree(_frequencyTable);
        _huffmanCodes = new Dictionary<char, string>();
        GenerateCodes(_root, "", _huffmanCodes);

        var compressed = new StringBuilder();
        foreach (var c in text)
        {
            compressed.Append(_huffmanCodes[c]);
        }

        return compressed.ToString();
    }

    public string Decompress(string compressedText)
    {
        if (string.IsNullOrEmpty(compressedText) || _root == null)
            return string.Empty;

        var result = new StringBuilder();
        var current = _root;

        foreach (var bit in compressedText)
        {
            current = bit == '0' ? current.Left : current.Right;

            if (current.IsLeaf)
            {
                result.Append(current.Character!.Value);
                current = _root;
            }
        }

        return result.ToString();
    }


    private Dictionary<char, int> BuildFrequencyTable(string text)
    {
        var freq = new Dictionary<char, int>();

        foreach (var c in text)
        {
            if (!freq.ContainsKey(c))
                freq[c] = 0;

            freq[c]++;
        }

        return freq;
    }


    private HuffmanNode BuildHuffmanTree(Dictionary<char, int> frequencies)
    {
        var pq = new PriorityQueue<HuffmanNode, int>();

        foreach (var kv in frequencies)
        {
            pq.Enqueue(new HuffmanNode(kv.Key, kv.Value), kv.Value);
        }

        if (pq.Count == 1)
        {
            var single = pq.Dequeue();

            return new HuffmanNode(null, single.Frequency)
            {
                Left = single,
                Right = null
            };
        }

        while (pq.Count > 1)
        {
            var left = pq.Dequeue();
            var right = pq.Dequeue();

            var parent = new HuffmanNode(null, left.Frequency + right.Frequency)
            {
                Left = left,
                Right = right
            };

            pq.Enqueue(parent, parent.Frequency);
        }

        return pq.Dequeue();
    }

    private void GenerateCodes(HuffmanNode? node, string code, Dictionary<char, string> codes)
    {
        if (node == null)
            return;

        if (node.IsLeaf)
        {
            char ch = node.Character!.Value;

            codes[ch] = code.Length > 0 ? code : "0";
            return;
        }

        GenerateCodes(node.Left, code + "0", codes);
        GenerateCodes(node.Right, code + "1", codes);
    }



    public Dictionary<char, string> GetHuffmanCodes()
    {
        return new Dictionary<char, string>(_huffmanCodes);
    }

    public Dictionary<char, int> GetFrequencyTable()
    {
        return new Dictionary<char, int>(_frequencyTable);
    }

    public HuffmanNode? GetHuffmanTree()
    {
        return _root;
    }

    public void Reset()
    {
        _frequencyTable.Clear();
        _huffmanCodes.Clear();
        _root = null;
    }
}
