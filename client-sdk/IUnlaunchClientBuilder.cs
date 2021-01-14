using System;

namespace io.unlaunch
{
    public interface IUnlaunchClientBuilder
    {
        IUnlaunchClient Build();

        IUnlaunchClientBuilder SdkKey(string sdkKey);

        IUnlaunchClientBuilder OfflineMode();

        IUnlaunchClientBuilder OfflineModeWithLocalFeatures(string yamlFeaturesFilePath);

        IUnlaunchClientBuilder PollingInterval(TimeSpan ts);

        IUnlaunchClientBuilder ConnectionTimeout(TimeSpan ts);

        IUnlaunchClientBuilder Host(string baseUrl);

        IUnlaunchClientBuilder MetricsFlushInterval(TimeSpan ts);

        IUnlaunchClientBuilder EventsFlushInterval(TimeSpan ts);

        IUnlaunchClientBuilder EventsQueueSize(int maxQueueSize);

        IUnlaunchClientBuilder MetricsQueueSize(int maxQueueSize);

        IUnlaunchClientBuilder HttpClientFactory(IHttpClientFactory httpClientFactory);
    }
}
