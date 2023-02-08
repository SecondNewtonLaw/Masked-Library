using System.Security.Cryptography;
using System.Text;
using Masked.Sys.Extensions;

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
        if (hashCode.HasValue)
            return hashCode.Value;
        int dataVal = 0;
        var initial = ItemName.Length + SavePath.Length - Url.OriginalString.Length;

        byte[] data = Encoding.UTF8.GetBytes(ItemName[0..4]).Concat(Encoding.UTF8.GetBytes(Url.OriginalString[0..4])).Concat(Encoding.UTF8.GetBytes(SavePath[0..4])).ToArray();
        
        data.FastIterator((data, indx) =>
        {
            checked
            {
                dataVal += indx % 2 is 0 ? data.GetHashCode() : -data.GetHashCode();
            }
            return NextStep.Continue;
        });
        hashCode = dataVal;
        return hashCode.Value;
    }
}