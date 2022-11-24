using System.Collections.Generic;
using System.Threading.Tasks;

namespace Masked.Scraper.SearchEngines;
public interface ISearchScrape
{
    public List<ScrapedSearchResult> GetResults();
    public Task<List<ScrapedSearchResult>> GetResultsAsync();
}