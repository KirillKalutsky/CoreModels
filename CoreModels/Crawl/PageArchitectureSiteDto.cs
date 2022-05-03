using CoreModels.Crawl.PageParser;
using CoreModels.Crawl.UrlCreator;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.Crawl
{
    public class PageArchitectureSiteDto
    {
        public Dictionary<string, string> ParseEventProperties { get; set; }
        public string LastEventLink { get; set; }
        public UrlCreatorType urlCreatorType { get; set; }
        public TypePageParser pageParserType { get; set; }
        public string serializedUrlCreator { get; set; }
        public string serializedpageParser { get; set; }

    }

    public static class PageParserExtensions
    {
        public static Func<string, IEnumerable<string>> GetFunc(
            this TypePageParser typePageParser, string serializedPageParser)
        {
            IPageParser result;
            switch (typePageParser)
            {
                case TypePageParser.HtmlParser:
                    result = JsonConvert.DeserializeObject<HtmlParser>(serializedPageParser);
                    break;
                case TypePageParser.JsonParser:
                    result = JsonConvert.DeserializeObject<JsonParser>(serializedPageParser);
                    break;
                default:
                    throw new ArgumentException($"Нет реализации для: {typePageParser}");
            }
            return result.ParsePageContent;
        }
    }

    public static class UrlCreatorExtensions
    {
        public static Func<int, string> GetFunc(
            this UrlCreatorType typePageParser, string serializedUrlCreator)
        {
            PageUrlCreator result;
            switch (typePageParser)
            {
                case UrlCreatorType.NumberUrl:
                    result = JsonConvert.DeserializeObject<NumberPageUrlCreator>(serializedUrlCreator);
                    break;
                case UrlCreatorType.DateUrl:
                    result = JsonConvert.DeserializeObject<DatePageUrlCreator>(serializedUrlCreator);
                    break;
                default:
                    throw new ArgumentException($"Нет реализации для: {typePageParser}");
            }
            return result.CreatePageUrl;
        }
    }

}
