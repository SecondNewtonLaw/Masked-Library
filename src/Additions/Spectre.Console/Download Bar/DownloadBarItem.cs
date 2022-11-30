using System.Security.Cryptography;

namespace Masked.SpectreConsole;

public struct DownloadBarItem : IEquatable<DownloadBarItem>
{
    public DownloadBarItem(Uri url, string itemName, string savePath)
    {
        Url = url;
        ItemName = itemName;
        SavePath = savePath;
        hashCode = null;
    }

    public Uri Url { get; init; }
    public string ItemName { get; init; }
    public string SavePath { get; init; }

    private int? hashCode;

    public override bool Equals(object? obj) => obj is DownloadBarItem item && Equals(item);

    public bool Equals(DownloadBarItem other)
        => EqualityComparer<Uri>.Default.Equals(Url, other.Url) && (ItemName == other.ItemName) && (SavePath == other.SavePath);

    public static bool operator ==(DownloadBarItem left, DownloadBarItem right) => left.Equals(right);

    public static bool operator !=(DownloadBarItem left, DownloadBarItem right) => !left.Equals(right);

    /// <summary>
    /// Receive the object's hashcode.
    /// </summary>
    /// <returns>A Randomly generated hash that is consistent through calls of the method.</returns>
    public override int GetHashCode()
    {
        // The HashCode must remain consistent.
        if (hashCode.HasValue)
            return hashCode.Value;

        static int BitShift(int value, int positions)
        {
            // Save the existing bit pattern, but interpret it as an unsigned integer.
            uint number = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
            // Preserve the bits to be discarded.
            uint wrapped = number >> (32 - positions);
            // Shift and wrap the discarded bits.
            return BitConverter.ToInt32(BitConverter.GetBytes((number << positions) | wrapped), 0);
        }

        int rndVal = RandomNumberGenerator.GetInt32(RandomNumberGenerator.GetInt32(913672193));
        int shift = RandomNumberGenerator.GetInt32(RandomNumberGenerator.GetInt32(24));

        hashCode = BitShift(rndVal, shift);
        return (int)hashCode;
    }
}