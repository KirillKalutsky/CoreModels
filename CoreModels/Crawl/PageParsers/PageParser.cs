using System.Collections.Generic;

namespace CoreModels.Crawl.PageParsers
{
    public abstract class PageParser
    {
        public string LinkURL { get; set; }
        public abstract IEnumerable<string> ParsePageContent(string pageContent);
    }
}
