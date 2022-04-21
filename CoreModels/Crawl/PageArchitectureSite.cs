using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreModels;
using System.Net.Http;
using System;

namespace WebCrawler
{
    public class PageArchitectureSite : CrawlableSource
    {
        public string StartUrl { get; set; }
        public string EndUrl { get; set; }
        public string LinkURL { get; set; }
        public HtmlElement LinkElement { get; set; }
        public Dictionary<string, string> ParseEventProperties { get; set; }

        private int? maxCountEvents;
        private string currentEventLink;
        int currentSeanceCrawledEventCount;

        public override async IAsyncEnumerable<Event> CrawlAsync(int? maxCountEvents, HttpClient httpClient)
        {
            this.maxCountEvents = maxCountEvents;
            currentSeanceCrawledEventCount = 0;
            var pageCounter = 1;
            isCrawl = true;
            var errors = new List<Exception>();
            while (isCrawl)
            {
                var url = $"{StartUrl}{pageCounter}{EndUrl}";

                HttpResponseMessage page;
                try
                {
                    page = await PageLoader.LoadPageAsync(httpClient, url, HttpMethod.Get);
                }
                catch (Exception e)
                {
                    yield break;
                }

                if (!page.IsSuccessStatusCode)
                {
                    yield break;
                }

                var pageLinks = await PageLoader.GetPageElementAsync(page, LinkElement);
                pageCounter++;
                var events = new Dictionary<Task<HttpResponseMessage>, string>();
                foreach (var link in pageLinks)
                {
                    var fullLink = $"{LinkURL}{link}";

                    currentEventLink = fullLink;
                    currentSeanceCrawledEventCount += 1;

                    if (isCrawl)
                        isCrawl = !StopCrawl();

                    Task<HttpResponseMessage> content;
                    try
                    {
                        content = httpClient.LoadPageAsync(fullLink, HttpMethod.Get);
                    }
                    catch (Exception e)
                    {
                        errors.Add(e);
                        continue;
                    }

                    events[content] = fullLink;
                }

                while (events.Any())
                {
                    var tP = await Task.WhenAny(events.Keys);
                    var link = events[tP];
                    events.Remove(tP);

                    var p = await tP;
                    if (!p.IsSuccessStatusCode)
                    {
                        continue;
                    }

                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(await p.Content.ReadAsStringAsync());

                    var news = document.ParseHtmlPage(ParseEventProperties);

                    news.Link = link;
                    news.IdSource = SourseId;

                    yield return news;
                }

            }

            if (errors.Any())
                throw new AggregateException(errors);
        }

        public override bool StopCrawl()
        {
            if (lastEventLink != null)
                return currentEventLink.Equals(lastEventLink);
            if (maxCountEvents != null)
                return currentSeanceCrawledEventCount >= maxCountEvents;
            return false;
        }
    }
}
