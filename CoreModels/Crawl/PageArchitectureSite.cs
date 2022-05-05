using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreModels;
using System.Net.Http;
using System;
using System.Text;

namespace WebCrawler
{
    public class PageArchitectureSite : ICrawlableSource
    {
        public string LastEventLink { get; set; }
        public Dictionary<string, string> ParseEventProperties { get; set; }
        public Func<int, string> getPageUrl{ get; set; }
        public Func<string, IEnumerable<string>> getEventsUrl { get; set; }

        private int currentSeanceCrawledEventCount;
        private bool isCrawl;
        private HttpClient httpClient;
        private List<Exception> exceptions;

        public async IAsyncEnumerable<Event> CrawlAsync(HttpClient httpClient, int maxCountEvents)
        {
            this.httpClient = httpClient;
            currentSeanceCrawledEventCount = 0;
            var pageCounter = 1;
            isCrawl = true;
            exceptions = new List<Exception>();

            while (isCrawl)
            {
                var pageLinks = await GetEventsUri(pageCounter);
                pageCounter++;

                var events = DownloadEventsContext(pageLinks, maxCountEvents);

                await foreach (var ev in GetEventFromResponse(events))
                    yield return ev;
            }

            if (exceptions.Any())
                throw new AggregateException(exceptions);
        }

        private bool StopCrawl(string currentEventLink, int maxCountEvents)
        {
            if (LastEventLink != null)
                return currentEventLink.Equals(LastEventLink);
            
            return currentSeanceCrawledEventCount >= maxCountEvents;
        }

        private async Task<IEnumerable<string>> GetEventsUri(int pageNumber)
        {
            var url = getPageUrl(pageNumber);

            HttpResponseMessage page;
            try
            {
                page = await httpClient.LoopSendingAsync(url, HttpMethod.Get);
            }
            catch (Exception exc)
            {
                page = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
                exceptions.Add(exc);
                isCrawl = false;
            }

            if (!page.IsSuccessStatusCode)
                isCrawl = false;

            string pageContent;
            try
            {
                pageContent = await page.Content.ReadAsStringAsync();
            }
            catch (Exception exc)
            {
                pageContent = String.Empty;
                exceptions.Add(exc);
                isCrawl = false;
            }

            var pageLinks = getEventsUrl(pageContent);

            return pageLinks;
        }

        private Dictionary<Task<HttpResponseMessage>, string> DownloadEventsContext(
            IEnumerable<string> pageLinks, int maxCountEvents)
        {
            var events = new Dictionary<Task<HttpResponseMessage>, string>();
            foreach (var link in pageLinks)
            {
                currentSeanceCrawledEventCount += 1;

                if (isCrawl)
                    isCrawl = !StopCrawl(link, maxCountEvents);

                Task<HttpResponseMessage> content;
                try
                {
                    content = httpClient.LoopSendingAsync(link, HttpMethod.Get);
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                    continue;
                }

                events[content] = link;
            }

            return events;
        }

        private async IAsyncEnumerable<Event> GetEventFromResponse(Dictionary<Task<HttpResponseMessage>, string> events)
        {
            while (events.Any())
            {
                var tP = await Task.WhenAny(events.Keys);
                var link = events[tP];
                events.Remove(tP);

                HttpResponseMessage p;
                try
                {
                    p = await tP;
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    p = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
                }
                if (!p.IsSuccessStatusCode)
                {
                    continue;
                }

                HtmlDocument document = new HtmlDocument();
                var stringContent = await p.Content.ReadAsStringAsync();
                document.LoadHtml(stringContent);

                var news = ParseHtmlPage(document, ParseEventProperties);

                news.Link = link;

                yield return news;
            }
        }

        private Event ParseHtmlPage(HtmlDocument page, Dictionary<string, string> pageElements)
        {
            var result = new Event();
            foreach (var e in pageElements)
            {
                var p = result.GetType().GetProperty(e.Key);
                var value = new StringBuilder();

                var nodes = page.DocumentNode;
                var myNodes = nodes.SelectNodes(e.Value);

                if (myNodes != null)
                {
                    foreach (var node in myNodes)
                        value.Append(node.InnerText);
                }

                p.SetValue(result, value.ToString().ReplaceHtmlTags(string.Empty));
            }
            return result;
        }

        bool ICrawlableSource.IsCrawl() => isCrawl;
    }
}
