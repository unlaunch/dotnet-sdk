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

        UnlaunchRestWrapper(string sdkKey, HttpClient httpClient, string baseUrl, string apiPath, int connectionTimeOutMs)
        {
            SetupHttpClient(sdkKey, httpClient, baseUrl, connectionTimeOutMs);
            _httpClient = httpClient;

            _apiPath = apiPath;
        }

        public static UnlaunchRestWrapper Create(string sdkKey, HttpClient httpClient, string baseUrl, string apiPath, int connectionTimeOutMs)
        {
            return new UnlaunchRestWrapper(sdkKey, httpClient, baseUrl, apiPath, connectionTimeOutMs);
        }

        public async Task<HttpResponseMessage> GetAsync()
        {
            _httpClient.DefaultRequestHeaders.Remove("If-Modified-Since");
            _httpClient.DefaultRequestHeaders.Add("If-Modified-Since", _lastModified.ToString("ddd, dd MMM yyyy HH:mm:ss zzzz"));
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

        private void SetupHttpClient(string sdkKey, HttpClient httpClient, string baseUrl, int connectionTimeOutMs)
        {
            if (httpClient == null)
            {
                throw new ArgumentException("HttpClient can't be null");
            }
            
            httpClient.Timeout = TimeSpan.FromMilliseconds(connectionTimeOutMs);
            httpClient.BaseAddress = new Uri(baseUrl);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("X-Api-Key", sdkKey);
        }
    }
}
