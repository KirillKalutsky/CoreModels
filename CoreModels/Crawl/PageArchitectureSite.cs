using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreModels;
using System.Net.Http;
using System;
using CoreModels.Extensions;
using System.Text;
using CoreModels.Crawl;
using CoreModels.Crawl.UrlCreator;

namespace WebCrawler
{
    public class PageArchitectureSite : CrawlableSource
    {
        public string LastEventLink { get; set; }
        
        public Dictionary<string, string> ParseEventProperties { get; set; }

        private int? maxCountEvents;
        private string currentEventLink;
        private int currentSeanceCrawledEventCount;

        
        public PageUrlCreator pageUrlCreator { get; set; }
        public IPageParser eventsUrlParser { get; set; }

        public override async IAsyncEnumerable<Event> CrawlAsync(int? maxCountEvents, HttpClient httpClient)
        {
            this.maxCountEvents = maxCountEvents;
            currentSeanceCrawledEventCount = 0;
            var pageCounter = 1;
            isCrawl = true;
            var errors = new List<Exception>();

            while (isCrawl)
            {
                var url = pageUrlCreator.CreatePageUrl(pageCounter);

                HttpResponseMessage page;
                try
                {
                    page = await httpClient.LoopSendingAsync(url, HttpMethod.Get);
                }
                catch (Exception exc)
                {
                    errors.Add(exc);
                    break;
                }

                if (!page.IsSuccessStatusCode)
                {
                    break;
                }

                var pageContent = await page.Content.ReadAsStringAsync();
                var pageLinks = eventsUrlParser.ParsePageContent(pageContent);//await PageLoader.GetPageElementAsync(page, LinkElement);
                pageCounter++;
                var events = new Dictionary<Task<HttpResponseMessage>, string>();
                foreach (var link in pageLinks)
                {
                    //var fullLink = $"{LinkURL}{link}";

                    currentEventLink = link;
                    currentSeanceCrawledEventCount += 1;

                    if (isCrawl)
                        isCrawl = !StopCrawl();

                    Task<HttpResponseMessage> content;
                    try
                    {
                        content = httpClient.LoopSendingAsync(link, HttpMethod.Get);
                    }
                    catch (Exception e)
                    {
                        errors.Add(e);
                        continue;
                    }

                    events[content] = link;
                }

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
                    catch(Exception ex)
                    {
                        errors.Add(ex);
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

            if (errors.Any())
                throw new AggregateException(errors);
        }

        protected override bool StopCrawl()
        {
            if (LastEventLink != null)
                return currentEventLink.Equals(LastEventLink);
            if (maxCountEvents != null)
                return currentSeanceCrawledEventCount >= maxCountEvents;
            return false;
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
    }
}
