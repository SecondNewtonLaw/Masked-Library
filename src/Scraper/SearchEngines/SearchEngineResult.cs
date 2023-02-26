namespace Masked.Scraper.SearchEngines;

public struct ScrapedSearchResult {
    /// <summary>
    /// The position of the scraped item.
    /// </summary>
    public uint ItemPosition { get; set; }

    /// <summary>
    /// The URL of the scraped data.
    /// </summary>
    public string URL { get; set; }

    /// <summary>
    /// The title of the scraped item.
    /// </summary>
    public string? Title { get; set; }
}