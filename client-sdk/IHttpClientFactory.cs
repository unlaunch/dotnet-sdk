using System.Net.Http;

namespace io.unlaunch
{
    public interface IHttpClientFactory
    {
        HttpClient CreateClient();
    }
}
