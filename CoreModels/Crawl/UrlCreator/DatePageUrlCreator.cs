using Newtonsoft.Json;
using System;

namespace CoreModels.Crawl.UrlCreator
{
    public class DatePageUrlCreator : PageUrlCreator
    {
        [JsonProperty]
        private string datePattern;
        public DatePageUrlCreator(string datePattern)
        {
            this.datePattern = datePattern;
        }
        public override string CreatePageUrl(int pageNumber)
        {
            var date = DateTime.Now.AddDays(-1*pageNumber);
            string dateStr;
            if (datePattern == null)
                dateStr = ((DateTimeOffset)date).ToUnixTimeSeconds().ToString();
            else
                dateStr = date.ToString(datePattern);

            return $"{StartUrl}{dateStr}{EndUrl}";
        }
    }
}
