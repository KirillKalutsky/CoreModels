using System.Runtime.Serialization;
using System;
using System.Collections.Generic;

namespace CoreModels.Crawl.PageParsers
{
    [DataContract]
    public class JsonParser : PageParser
    {
        public override IEnumerable<string> ParsePageContent(string pageContent)
        {
            throw new NotImplementedException();
        }
    }
}
