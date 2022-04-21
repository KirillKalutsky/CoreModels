using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler
{
    public static class PageLoader
    {
        public static async Task<HttpResponseMessage> TrySendAsync(this HttpClient httpClient, string address, HttpMethod httpMethod, string content = null)
        {
            HttpResponseMessage responseRes;
            try
            {
                var request = new HttpRequestMessage(httpMethod, address);
                if (content != null)
                    request.Content = new StringContent(content, Encoding.UTF8, "application/json");
                responseRes = await httpClient.SendAsync(request);
            }
            catch (HttpRequestException exp)
            {
                throw exp;
            }

            return responseRes;
        }

        public static async Task<HttpResponseMessage> LoadPageAsync(this HttpClient httpClient, string url, HttpMethod method, int numberOfAttempts = 5, string content = null)
        {
            HttpResponseMessage result = null;
            for (var i = 0; i < numberOfAttempts; i++)
            {
                try
                {
                    result = await TrySendAsync(httpClient, url, method, content);
                }
                catch (Exception exc)
                {
                    throw exc;
                }
                if (result.IsSuccessStatusCode || numberOfAttempts == 0)
                    break;

                await Task.Delay(3);
            }
            return result;
        }



        public static async Task<IEnumerable<string>> GetPageElementAsync(this HttpClient httpClient, string url, HtmlElement link)
        {
            var result = new List<string>();

            var body = (await PageLoader.LoadPageAsync(httpClient, url, HttpMethod.Get));

            if (body.IsSuccessStatusCode)
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(await body.Content.ReadAsStringAsync());

                var searchElements = doc.DocumentNode.SelectNodes(link.XPath);

                if (searchElements != null)
                    foreach (var e in searchElements)
                    {
                        var l = e.GetAttributeValue(link.AttributeName, "");
                        result.Add(l);
                    }
                else
                {
                    Console.WriteLine($"{url} : элементы не найдены");
                    Debug.Print($"{url} : элементы не найдены");
                }
            }
            return result;
        }

        public static async Task<IEnumerable<string>> GetPageElementAsync(this HttpResponseMessage body, HtmlElement link)
        {
            var result = new List<string>();

            if (body.IsSuccessStatusCode)
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(await body.Content.ReadAsStringAsync());

                var searchElements = doc.DocumentNode.SelectNodes(link.XPath);

                if (searchElements != null)
                    foreach (var e in searchElements)
                    {
                        var l = e.GetAttributeValue(link.AttributeName, "");
                        result.Add(l);
                    }
                else
                {
                    Console.WriteLine($"{body} : элементы не найдены");
                    Debug.Print($"{body} : элементы не найдены");
                }
            }
            return result;
        }
    }
}
