using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace CoreModels.Crawl.UrlCreator
{
    [DataContract]
    public class DatePageUrlCreator : PageUrlCreator
    {
        [DataMember]
        private string datePattern;
        public DatePageUrlCreator(string datePattern)
        {
            this.datePattern = datePattern;
        }
        public override string CreatePageUrl(int pageNumber)
        {
            var date = DateTime.Now.AddDays(-1*(pageNumber-1));
            string dateStr;
            if (datePattern == null)
                dateStr = ((DateTimeOffset)date).ToUnixTimeSeconds().ToString();
            else
                dateStr = date.ToString(datePattern);

            return $"{StartUrl}{dateStr}{EndUrl}";
        }
    }
}
