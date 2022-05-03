using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.Crawl
{
    public interface IPageParser
    {
        IEnumerable<string> ParsePageContent(string pageContent);
    }
}
