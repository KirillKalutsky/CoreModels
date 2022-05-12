using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreModels.DBModels;
using System.Net.Http;
using System;
using System.Text;
using CoreModels.Crawl;
using CoreModels.Crawl.PageParsers;
using CoreModels.Crawl.UrlCreator;
using System.Runtime.Serialization;

namespace WebCrawler
{
    [KnownType(typeof(HtmlParser)), KnownType(typeof(JsonParser))]
    [DataContract]
    public class PageArchitectureSite : ICrawlableSource
    {
        public PageArchitectureSite(IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory.CreateClient();
        }
        [DataMember]
        public string LastEventLink { get; set; }
        [DataMember]
        public Dictionary<string, string> ParseEventProperties { get; set; }
        [DataMember]
        public PageParser parser{ get; set; }
        [DataMember]
        public PageUrlCreator urlCreator { get; set; }

        private int eventCount;
        private bool isCrawl;
        private HttpClient httpClient;
        private List<Exception> exceptions;

        public async IAsyncEnumerable<IncidentDto> CrawlAsync(int maxCountEvents)
        {
            eventCount = 0;
            var pageCounter = 1;
            isCrawl = true;
            exceptions = new List<Exception>();

            while (isCrawl)
            {
                var eventsUrl = await GetArticleLinksFromPage(pageCounter);
                pageCounter++;

                var httpArticles = DownloadArticlesContent(eventsUrl, maxCountEvents);

                await foreach (var ev in GetIncidentsFromResponse(httpArticles))
                    yield return ev;
            }

            if (exceptions.Any())
                throw new AggregateException(exceptions);
        }

        private bool StopCrawl(string currentEventLink, int maxCountEvents)
        {
            if (eventCount >= maxCountEvents)
                return true;

            if (LastEventLink != null)
                return currentEventLink.Equals(LastEventLink);
            
            return false;
        }

        private async Task<IEnumerable<string>> GetArticleLinksFromPage(int pageNumber)
        {
            var pageUrl = urlCreator.CreatePageUrl(pageNumber);
            IEnumerable<string> pageLinks = new string[0];

            HttpResponseMessage page;
            try
            {
                page = await httpClient.LoopSendingAsync(pageUrl, HttpMethod.Get);
            }
            catch (Exception exc)
            {
                page = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
                exceptions.Add(exc);
                isCrawl = false;
            }

            if (!page.IsSuccessStatusCode)
                isCrawl = false;
            else
            {
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
                pageLinks = parser.ParsePageContent(pageContent);
            }

            return pageLinks;
        }

        private Dictionary<Task<HttpResponseMessage>, string> DownloadArticlesContent(
            IEnumerable<string> pageLinks, int maxCountEvents)
        {
            var events = new Dictionary<Task<HttpResponseMessage>, string>();
            foreach (var link in pageLinks)
            {
                eventCount += 1;

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

        private async IAsyncEnumerable<IncidentDto> GetIncidentsFromResponse(
            Dictionary<Task<HttpResponseMessage>, string> events)
        {
            while (events.Any())
            {
                var tMessage = await Task.WhenAny(events.Keys);
                var link = events[tMessage];
                events.Remove(tMessage);

                HttpResponseMessage p;
                try
                {
                    p = await tMessage;
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    continue;
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

        private IncidentDto ParseHtmlPage(HtmlDocument page, Dictionary<string, string> pageElements)
        {
            var result = new IncidentDto();
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

    }
}
