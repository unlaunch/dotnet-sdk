using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using io.unlaunch.exception;
using Newtonsoft.Json;

namespace io.unlaunch
{
    public sealed class UnlaunchRestWrapper : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiPath;
        private DateTime _lastModified;

        private static readonly IUnlaunchLogger Logger = LoggerProvider.For<UnlaunchRestWrapper>();

        UnlaunchRestWrapper(string sdkKey, HttpClient httpClient, string baseUrl, string apiPath, TimeSpan connectionTimeOut)
        {
            SetupHttpClient(sdkKey, httpClient, baseUrl, connectionTimeOut);
            _httpClient = httpClient;

            _apiPath = apiPath;
        }

        public static UnlaunchRestWrapper Create(string sdkKey, HttpClient httpClient, string baseUrl, string apiPath, TimeSpan connectionTimeOut)
        {
            return new UnlaunchRestWrapper(sdkKey, httpClient, baseUrl, apiPath, connectionTimeOut);
        }

        public async Task<HttpResponseMessage> GetAsync()
        {
            _httpClient.DefaultRequestHeaders.Remove("If-Modified-Since");
            _httpClient.DefaultRequestHeaders.Add("If-Modified-Since", _lastModified.ToUniversalTime().ToString("r"));
            try
            {
                var response = await _httpClient.GetAsync(_apiPath);
                if (response.Content.Headers.LastModified.HasValue)
                {
                    _lastModified = response.Content.Headers.LastModified.Value.UtcDateTime;
                }

                return response;
            }
            catch (TaskCanceledException tce)
            {
                Logger.Error("Task took too long to complete", tce);
                throw new UnlaunchHttpException(tce.Message);
            }
        }

        public async Task PostAsync<T>(T t) where T : class
        {
            var json = JsonConvert.SerializeObject(t);
            await _httpClient.PostAsync(_apiPath, new StringContent(json, Encoding.UTF8, "application/json"));
        }

        public DateTime GetLastModified()
        {
            return _lastModified;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        private static void SetupHttpClient(string sdkKey, HttpClient httpClient, string baseUrl, TimeSpan connectionTimeOut)
        {
            if (httpClient == null)
            {
                throw new ArgumentException("HttpClient can't be null");
            }

            httpClient.Timeout = connectionTimeOut;
            httpClient.BaseAddress = new Uri(baseUrl);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("X-Api-Key", sdkKey);
        }
    }
}
