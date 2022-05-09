using System.Collections.Generic;
using CoreModels.Crawl;
using CoreModels.DBModels;

namespace WebCrawler
{
    public interface ICrawlableSource
    {
        IAsyncEnumerable<IncidentDto> CrawlAsync(int maxCountEvents);
    }
}