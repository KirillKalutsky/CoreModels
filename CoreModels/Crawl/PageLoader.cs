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
        public static async Task<HttpResponseMessage> TrySendAsync(
            this HttpClient httpClient, string address, HttpMethod httpMethod, string content = null)
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
                throw;
            }

            return responseRes;
        }

        public static async Task<HttpResponseMessage> LoopSendingAsync(this HttpClient httpClient,
            string url, HttpMethod method, int numberOfAttempts = 5, string content = null)
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
                    throw;
                }
                if (result.IsSuccessStatusCode || numberOfAttempts == 0)
                    break;

                await Task.Delay(3);
            }
            return result;
        }

    }
}
