using System.Collections.Generic;
using System.Net.Http;
using CoreModels;

namespace WebCrawler
{
    public interface ICrawlableSource
    {
        IAsyncEnumerable<Event> CrawlAsync(HttpClient httpClient, int maxCountEvents = 100);
        bool IsCrawl();
    }
}