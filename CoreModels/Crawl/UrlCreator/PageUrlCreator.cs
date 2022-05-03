using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.Crawl.UrlCreator
{
    public abstract class PageUrlCreator
    {
        public string StartUrl { get; set; }
        public string EndUrl { get; set; }
        public string LinkURL { get; set; }
        public abstract string CreatePageUrl(int pageNumber);
    }
}
