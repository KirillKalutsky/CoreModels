
using System.Runtime.Serialization;

namespace CoreModels.Crawl.UrlCreator
{
    [DataContract]
    public class NumberPageUrlCreator : PageUrlCreator
    {
        public override string CreatePageUrl(int pageNumber)
        {
            return $"{StartUrl}{pageNumber}{EndUrl}";
        }
    }
}
