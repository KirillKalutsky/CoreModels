using System.Runtime.Serialization;

namespace CoreModels.Crawl.UrlCreator
{
    [DataContract]
    public abstract class PageUrlCreator
    {
        [DataMember]
        public string StartUrl { get; set; }
        [DataMember]
        public string EndUrl { get; set; }
        public abstract string CreatePageUrl(int pageNumber);
    }
}
