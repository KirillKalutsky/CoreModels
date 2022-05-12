using System.Runtime.Serialization;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using WebCrawler;

namespace CoreModels.Crawl.PageParsers
{
    [DataContract]
    public class HtmlParser : PageParser
    {
        [DataMember]
        private HtmlElement LinkElement;
        public HtmlParser(HtmlElement linkElement)
        {
            LinkElement = linkElement;
        }

        public override IEnumerable<string> ParsePageContent(string content)
        {
            content = DecodeEncodedNonAsciiCharacters(content);
            var doc = new HtmlDocument();
            doc.LoadHtml(content);

            var searchElements = doc.DocumentNode.SelectNodes(LinkElement.XPath);

            if (searchElements != null)
                foreach (var e in searchElements)
                {
                    var l = e.GetAttributeValue(LinkElement.AttributeName, "");
                    yield return $"{LinkURL}{l}";
                }
            else
                throw new HtmlElementNotFoundException($"{content} : элементы не найдены");
        }

        private string DecodeEncodedNonAsciiCharacters(string value)
        {
            return Regex.Replace(
                value,
                @"\\u(?<Value>[a-zA-Z0-9]{4})",
                m => {
                    return ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString();
                });
        }

    }
    public class HtmlElementNotFoundException : Exception
    {
        public HtmlElementNotFoundException(string message) : base(message) { }
    }
}
