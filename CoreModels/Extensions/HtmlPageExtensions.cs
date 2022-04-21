using HtmlAgilityPack;
using System.Collections.Generic;
using System.Text;
using WebCrawler;

namespace CoreModels.Extensions
{
    public static class HtmlPageExtensions
    {
        public static Event ParseHtmlPage(this HtmlDocument page, Dictionary<string, string> pageElements)
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
