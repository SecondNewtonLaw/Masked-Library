using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Masked.Sys.Extensions;

namespace Masked.Scraper.SearchEngines;

public class GoogleScraper : ISearchScrape {
    private const string _xPathToLinks = "//div[@class=\"yuRUbf\"]/a";
    private const string _xPathToTitles = "//h3[@class=\"LC20lb MBeuO DKV0Md\"]";
    private static readonly HtmlWeb _htmlWeb = new();
    private readonly HtmlDocument _targetDoc;
    private readonly string _query;

    /// <summary>
    /// Try to scrape results of the HTML in the specified URL
    /// </summary>
    /// <param name="url">The URL of the HTML to try to scrape results from</param>
    public GoogleScraper(Uri url) {
        _targetDoc = _htmlWeb.Load(url);
        _query = url.ToString();
    }

    /// <summary>
    /// Try to scrape results of the HTML of a google page from the specified query
    /// </summary>
    /// <param name="query">The query that will be processed into a Google URL.</param>
    public GoogleScraper(string query) {
        const string GOOGLE_SEARCH_URL = "https://www.google.com/search";
        FormUrlEncodedContent encodedContent = new(
            new List<KeyValuePair<string, string>>() {
                new("q", query)
            }
        );

        _targetDoc = _htmlWeb.Load(
            new StringBuilder()
                .Append(GOOGLE_SEARCH_URL)
                .Append('?')
                .Append(encodedContent.ReadAsStringAsync().Result).ToString()
        );

        _query = query;
    }

    /// <summary>
    /// Scrape Google search.
    /// </summary>
    /// <exception cref="NoResultsException">Thrown if no results are found</exception>
    /// <returns>a List of scraped results.</returns>
    public List<ScrapedSearchResult> GetResults() {
        var endresult = new List<ScrapedSearchResult>(); // Create ScrapeResult List.

        var hnc_title = _targetDoc.DocumentNode.SelectNodes(_xPathToTitles);
        var hnc_links = _targetDoc.DocumentNode.SelectNodes(_xPathToLinks);

        if (hnc_links is null || hnc_title is null) throw new NoResultsException($"No results found for \'{_query}\'");
        ReadOnlySpan<HtmlNode> titlesSpan = CollectionsMarshal.AsSpan(hnc_title.ToList());
        ref var searchSpace = ref MemoryMarshal.GetReference(titlesSpan);
        unsafe {
            var ptr = Unsafe.AsPointer(ref searchSpace);
            hnc_links.FastIterator((node, index) => {
                var title = Unsafe.Add(ref Unsafe.AsRef<HtmlNode>(ptr), index);

                endresult.Add(new ScrapedSearchResult {
                    ItemPosition = (uint)index,
                    URL = node.Attributes["href"].Value,
                    Title = title.InnerText
                });
                return NextStep.Continue;
            });
        }

        return endresult;
    }

    /// <summary>
    /// Scrape Google search. Asynchronously
    /// </summary>
    /// <exception cref="NoResultsException">Thrown if no results are found</exception>
    /// <returns>a List of scraped results.</returns>
    public async Task<List<ScrapedSearchResult>> GetResultsAsync() {
        var endresult = new List<ScrapedSearchResult>(); // Create ScrapeResult List.
        Task<HtmlNodeCollection> hnc_title_task = Task.Run(() => _targetDoc.DocumentNode.SelectNodes(_xPathToTitles));
        Task<HtmlNodeCollection> hnc_links_task = Task.Run(() => _targetDoc.DocumentNode.SelectNodes(_xPathToLinks));
        Task<HtmlNodeCollection>[] tasks = new Task<HtmlNodeCollection>[2] {
            hnc_title_task, hnc_links_task
        };

        HtmlNodeCollection[] taskResult = await Task.WhenAll(tasks);

        if (taskResult[0] is null || taskResult[1] is null)
            throw new NoResultsException($"No results found for \'{_query}\'");

        var hnc_title = taskResult[0];
        var hnc_links = taskResult[1];

        hnc_links.FastIterator((node, index) => {
            endresult.Add(new ScrapedSearchResult {
                ItemPosition = (uint)index,
                URL = node.Attributes["href"].Value,
                Title = hnc_title[index].InnerText
            });
            return NextStep.Continue;
        });
        return endresult;
    }
}