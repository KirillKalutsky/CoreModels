using System.Runtime.Serialization;
using System.Collections.Generic;

namespace CoreModels.Crawl.PageParsers
{
    [DataContract]
    public abstract class PageParser
    {
        [DataMember]
        public string LinkURL { get; set; }
        public abstract IEnumerable<string> ParsePageContent(string pageContent);
    }
}
