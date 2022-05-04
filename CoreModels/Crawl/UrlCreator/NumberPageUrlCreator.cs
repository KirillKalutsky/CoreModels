
namespace CoreModels.Crawl.UrlCreator
{
    public class NumberPageUrlCreator : PageUrlCreator
    {
        public override string CreatePageUrl(int pageNumber)
        {
            return $"{StartUrl}{pageNumber}{EndUrl}";
        }
    }
}
