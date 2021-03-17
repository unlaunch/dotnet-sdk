using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace io.unlaunch
{
    public sealed class UnlaunchGenericRestWrapper : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _path;

        private static readonly IUnlaunchLogger Logger = LoggerProvider.For<UnlaunchGenericRestWrapper>();

        UnlaunchGenericRestWrapper(HttpClient httpClient, string baseUrl, string path, TimeSpan connectionTimeOut)
        {
            SetupHttpClient(httpClient, baseUrl, connectionTimeOut);
            _httpClient = httpClient;

            _path = path;
        }

        public static UnlaunchGenericRestWrapper Create(HttpClient httpClient, string baseUrl, string path, TimeSpan connectionTimeOut)
        {
            return new UnlaunchGenericRestWrapper(httpClient, baseUrl, path, connectionTimeOut);
        }

        public async Task<HttpResponseMessage> GetAsync()
        {
            return await _httpClient.GetAsync(_path);
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        private static void SetupHttpClient(HttpClient httpClient, string baseUrl, TimeSpan connectionTimeOut)
        {
            if (httpClient == null)
            {
                throw new ArgumentException("HttpClient can't be null");
            }

            httpClient.Timeout = connectionTimeOut;
            httpClient.BaseAddress = new Uri(baseUrl);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
