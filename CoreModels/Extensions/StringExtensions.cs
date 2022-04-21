using System.Text.RegularExpressions;

namespace WebCrawler
{
    public static class StringExtensions
    {
        public static string RemoveExtraSpace(this string text)
        {
            var filter = @"[^\S\r\n]+";

            return Regex.Replace(text.Trim(), filter, " ");
        }
        public static string ReplaceHtmlTags(this string text, string alternativeText)
        {
            var filter = @"(&\s?\S+?\s?;)?(\\\w+)?";

            var cleanString = Regex.Replace(text, filter, alternativeText);
            return cleanString.RemoveExtraSpace();
        }
    }
}
