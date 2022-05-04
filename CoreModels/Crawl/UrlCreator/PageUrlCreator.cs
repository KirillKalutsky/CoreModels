
namespace CoreModels.Crawl.UrlCreator
{
    public abstract class PageUrlCreator
    {
        public string StartUrl { get; set; }
        public string EndUrl { get; set; }
        public abstract string CreatePageUrl(int pageNumber);
    }
}
