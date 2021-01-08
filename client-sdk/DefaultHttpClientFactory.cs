using System;
using System.Net;
using System.Net.Http;

namespace io.unlaunch
{
    public class DefaultHttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateClient()
        {
            var httpClientHandler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            var httpClient = new HttpClient(httpClientHandler)
            {
                Timeout = TimeSpan.FromMilliseconds(10000)
            };

            httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
            httpClient.DefaultRequestHeaders.Add("Keep-Alive", "true");

            return httpClient;
        }
    }
}
