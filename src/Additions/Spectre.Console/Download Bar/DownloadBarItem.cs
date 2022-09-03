namespace Masked.SpectreConsole;

public struct DownloadBarItem
{
    public DownloadBarItem(Uri url, string itemName, string savePath) { this.Url = url; this.ItemName = itemName; this.SavePath = savePath; }
    public Uri Url { get; init; }
    public string ItemName { get; init; }
    public string SavePath { get; init; }
}