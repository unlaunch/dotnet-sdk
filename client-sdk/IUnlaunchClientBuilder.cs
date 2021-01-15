using System;

namespace io.unlaunch
{
    public interface IUnlaunchClientBuilder
    {
        IUnlaunchClient Build();

        IUnlaunchClientBuilder SdkKey(string sdkKey);

        IUnlaunchClientBuilder OfflineMode();

        IUnlaunchClientBuilder OfflineModeWithLocalFeatures(string yamlFeaturesFilePath);

        IUnlaunchClientBuilder PollingInterval(TimeSpan timeSpan);

        IUnlaunchClientBuilder ConnectionTimeout(TimeSpan timeSpan);

        IUnlaunchClientBuilder Host(string baseUrl);

        IUnlaunchClientBuilder MetricsFlushInterval(TimeSpan timeSpan);

        IUnlaunchClientBuilder EventsFlushInterval(TimeSpan timeSpan);

        IUnlaunchClientBuilder EventsQueueSize(int maxQueueSize);

        IUnlaunchClientBuilder MetricsQueueSize(int maxQueueSize);

        IUnlaunchClientBuilder HttpClientFactory(IHttpClientFactory httpClientFactory);
    }
}
