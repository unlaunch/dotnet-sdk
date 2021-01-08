namespace io.unlaunch
{
    public interface IUnlaunchClientBuilder
    {
        IUnlaunchClient Build();

        IUnlaunchClientBuilder SdkKey(string sdkKey);

        IUnlaunchClientBuilder OfflineMode();

        IUnlaunchClientBuilder OfflineModeWithLocalFeatures(string yamlFeaturesFilePath);

        IUnlaunchClientBuilder PollingIntervalInSeconds(int intervalInSeconds);

        IUnlaunchClientBuilder ConnectionTimeoutInMilliseconds(int millisecondsTimeout);

        IUnlaunchClientBuilder Host(string baseUrl);

        IUnlaunchClientBuilder MetricsFlushIntervalInSeconds(int intervalInSeconds);

        IUnlaunchClientBuilder EventsFlushIntervalInSeconds(int intervalInSeconds);

        IUnlaunchClientBuilder EventsQueueSize(int maxQueueSize);

        IUnlaunchClientBuilder MetricsQueueSize(int maxQueueSize);

        IUnlaunchClientBuilder HttpClientFactory(IHttpClientFactory httpClientFactory);
    }
}
