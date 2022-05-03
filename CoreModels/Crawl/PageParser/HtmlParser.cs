using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler;

namespace CoreModels.Crawl
{
    public class HtmlParser : IPageParser
    {
        public HtmlElement LinkElement { get; set; }
        public HtmlParser(HtmlElement linkElement)
        {
            LinkElement = linkElement;
        }

        public IEnumerable<string> ParsePageContent(string content)
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
