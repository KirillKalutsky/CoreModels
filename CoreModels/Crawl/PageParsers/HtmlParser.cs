using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using WebCrawler;

namespace CoreModels.Crawl.PageParsers
{
    public class HtmlParser : PageParser
    {
        [JsonProperty]
        private HtmlElement LinkElement;
        public HtmlParser(HtmlElement linkElement)
        {
            LinkElement = linkElement;
        }

        public override IEnumerable<string> ParsePageContent(string content)
        {
            var result = new List<string>();
            
            var doc = new HtmlDocument();
            doc.LoadHtml(content);

            var searchElements = doc.DocumentNode.SelectNodes(LinkElement.XPath);

            if (searchElements != null)
                foreach (var e in searchElements)
                {
                    var l = e.GetAttributeValue(LinkElement.AttributeName, "");
                    result.Add(l);
                }
            else
                throw new HtmlElementNotFoundException($"{content} : элементы не найдены");
            
            return result;
        }

    }
    public class HtmlElementNotFoundException : Exception
    {
        public HtmlElementNotFoundException(string message) : base(message) { }
    }
}
