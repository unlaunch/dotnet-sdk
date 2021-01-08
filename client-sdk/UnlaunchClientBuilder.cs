using System;
using System.Diagnostics.Contracts;
using System.Threading;
using io.unlaunch.atomic;
using io.unlaunch.events;
using io.unlaunch.store;
using io.unlaunch.utils;
using NLog;

namespace io.unlaunch
{
    public class UnlaunchClientBuilder : IUnlaunchClientBuilder
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private string _sdkKey;
        private bool _isOffline;
        private int _pollingIntervalInSeconds = 60;
        private int _metricsFlushIntervalInSeconds = 30;
        private int _eventsFlushIntervalInSeconds = 60;
        private int _metricsQueueSize = 100;
        private int _eventsQueueSize = 500;
        private string _baseUrl = "https://api.unlaunch.io";
        private int _connectionTimeoutMs = 10_000;
        private string _yamlFeaturesFilePath;

        private bool _pollingIntervalUpdatedByUser;
        private bool _metricsFlushIntervalUpdatedByUser;
        private bool _eventsFlushIntervalUpdatedByUser;
        private IHttpClientFactory _httpClientFactory = new DefaultHttpClientFactory();

        private const string FlagApiPath = "/api/v1/flags";
        private const string EventApiPath = "/api/v1/events";
        private const string ImpressionApiPath = "/api/v1/impressions";
        
        public static int MinPollIntervalInSeconds = 15;
        public static int MinMetricsFlushIntervalInSeconds = 15;
        public static int MinEventsFlushIntervalInSeconds = 15;
        public static int MinEventsQueueSize = 500;
        public static int MinMetricsQueueSize = 100;
        public static int MinConnectionTimeoutMillis = 10_000;

        public IUnlaunchClient Build()
        {
            if (!string.IsNullOrEmpty(_sdkKey) && !_sdkKey.StartsWith("prod"))
            {
                Logger.Info("SDK key doesn't appear to be for production environment. Using aggressive settings to " +
                            "poll and sync events so data appears on Unlaunch Console faster.");
                LoadPreProductionDefaults();
            }

            ValidateConfigurationParameters();

            IUnlaunchClient client;
            if (_isOffline)
            {
                client = string.IsNullOrEmpty(_yamlFeaturesFilePath) ? new OfflineUnlaunchClient() : new OfflineUnlaunchClient(_yamlFeaturesFilePath);
            }
            else
            {
                client = CreateUnlaunchClient();
            }

            Logger.Info($"client built with following parameters {GetConfigurationAsPrintableString()}");
            
            return client;
        }

        public IUnlaunchClientBuilder SdkKey(string sdkKey)
        {
            _sdkKey = sdkKey;
            return this;
        }

        public IUnlaunchClientBuilder OfflineMode()
        {
            _isOffline = true;
            return this;
        }

        public IUnlaunchClientBuilder OfflineModeWithLocalFeatures(string yamlFeaturesFilePath)
        {
            _isOffline = true;
            _yamlFeaturesFilePath = yamlFeaturesFilePath;
            return this;
        }

        public IUnlaunchClientBuilder PollingIntervalInSeconds(int intervalInSeconds)
        {
            _pollingIntervalInSeconds = intervalInSeconds;
            _pollingIntervalUpdatedByUser = true;
            return this;
        }

        public IUnlaunchClientBuilder ConnectionTimeoutInMilliseconds(int millisecondsTimeout)
        {
            _connectionTimeoutMs = millisecondsTimeout;
            return this;
        }

        public IUnlaunchClientBuilder Host(string baseUrl)
        {
            _baseUrl = baseUrl;
            return this;
        }

        public IUnlaunchClientBuilder MetricsFlushIntervalInSeconds(int intervalInSeconds)
        {
            _metricsFlushIntervalInSeconds = intervalInSeconds;
            _metricsFlushIntervalUpdatedByUser = true;
            return this;
        }

        public IUnlaunchClientBuilder EventsFlushIntervalInSeconds(int intervalInSeconds)
        {
            _eventsFlushIntervalInSeconds = intervalInSeconds;
            _eventsFlushIntervalUpdatedByUser = true;
            return this;
        }

        public IUnlaunchClientBuilder EventsQueueSize(int maxQueueSize)
        {
            _eventsQueueSize = maxQueueSize;
            _eventsFlushIntervalUpdatedByUser = true;
            return this;
        }

        public IUnlaunchClientBuilder MetricsQueueSize(int maxQueueSize)
        {
            _metricsQueueSize = maxQueueSize;
            return this;
        }

        public IUnlaunchClientBuilder HttpClientFactory(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            return this;
        }

        private IUnlaunchClient CreateUnlaunchClient()
        {
            var restWrapper = UnlaunchRestWrapper.Create(_sdkKey, _httpClientFactory.CreateClient(), _baseUrl, FlagApiPath, _connectionTimeoutMs);
            var initialDownloadDoneEvent = new CountdownEvent(1);
            var downloadSuccessful = new AtomicBoolean(false);
            var refreshableDataStoreProvider = new RefreshableDataStoreProvider(restWrapper, initialDownloadDoneEvent, downloadSuccessful, _pollingIntervalInSeconds);
            
            var dataStore = refreshableDataStoreProvider.GetNoOpDataStore();
            try
            {
                dataStore = refreshableDataStoreProvider.GetDataStore();
            }
            catch 
            {
                Logger.Error("Unable to download features and init. Make sure you're using the " +
                             "correct SDK Key. We'll retry again but this error is usually not recoverable.");
            }

            var impressionApiRestClient = UnlaunchRestWrapper.Create(_sdkKey, _httpClientFactory.CreateClient(), _baseUrl, ImpressionApiPath, _connectionTimeoutMs);
            var impressionsEventHandler = new GenericEventHandler("metrics-impressions", impressionApiRestClient, _metricsFlushIntervalInSeconds, _metricsQueueSize);

            var eventsApiRestClient = UnlaunchRestWrapper.Create(_sdkKey, _httpClientFactory.CreateClient(), _baseUrl, EventApiPath, _connectionTimeoutMs);
            var eventHandler = new GenericEventHandler("generic", eventsApiRestClient, _eventsFlushIntervalInSeconds, _eventsQueueSize);
            var variationsCountEventHandler = new CountAggregatorEventHandler(eventHandler, _metricsFlushIntervalInSeconds);

            return UnlaunchClient.Create(dataStore, eventHandler, variationsCountEventHandler, impressionsEventHandler,
                initialDownloadDoneEvent, downloadSuccessful);
        }

        private void LoadPreProductionDefaults()
        {
            if (!_pollingIntervalUpdatedByUser)
            {
                _pollingIntervalInSeconds = MinPollIntervalInSeconds;
            }

            if (!_eventsFlushIntervalUpdatedByUser)
            {
                _eventsFlushIntervalInSeconds = MinEventsFlushIntervalInSeconds;
            }

            if (!_metricsFlushIntervalUpdatedByUser)
            {
                _metricsFlushIntervalInSeconds = MinMetricsFlushIntervalInSeconds;
            }
        }
        
        private void ValidateConfigurationParameters()
        {
            if (!_isOffline)
            {
                if (string.IsNullOrEmpty(_sdkKey))
                {
                    // User didn't supply SDK key, try reading from environment variable
                    var s = Environment.GetEnvironmentVariable(Constants.SdkKeyEnvVariableName);
                    if (string.IsNullOrEmpty(s))
                    {
                        throw new ArgumentException("sdkKey cannot be null or empty. Must be supplied to the builder or set as an environment variable.");
                    }

                    Logger.Info("Setting SDK Key read from environment variable");
                    _sdkKey = s;
                }

                if (string.IsNullOrEmpty(_baseUrl))
                {
                    throw new ArgumentException("hostname cannot be null or empty. Must point to a valid Unlaunch Service host");
                }
            }

            Contract.EnsuresOnThrow<ArgumentException>(_pollingIntervalInSeconds >= MinPollIntervalInSeconds,
                $"pollingInterval must be great than {MinPollIntervalInSeconds} seconds");
            Contract.EnsuresOnThrow<ArgumentException>(_connectionTimeoutMs >= MinConnectionTimeoutMillis,
                $"connectionTimeOut must be at least {_connectionTimeoutMs} milliseconds ");
            Contract.EnsuresOnThrow<ArgumentException>(_connectionTimeoutMs < int.MaxValue,
                "connectionTimeOut must be less than int.MaxValue={int.MaxValue}");
            Contract.EnsuresOnThrow<ArgumentException>(_metricsFlushIntervalInSeconds >= MinMetricsFlushIntervalInSeconds,
                $"metricsFlushInterval must be great than {MinMetricsFlushIntervalInSeconds} seconds");
            Contract.EnsuresOnThrow<ArgumentException>(_eventsFlushIntervalInSeconds >= MinEventsFlushIntervalInSeconds,
                $"eventsFlushInterval must be great than {MinEventsFlushIntervalInSeconds} seconds");
            Contract.EnsuresOnThrow<ArgumentException>(_eventsQueueSize >= MinEventsQueueSize,
                $"eventsQueue must be at least {MinEventsQueueSize}");
            Contract.EnsuresOnThrow<ArgumentException>(_metricsQueueSize >= MinMetricsQueueSize,
                $"metricsQueueSize must be at least {MinMetricsQueueSize}");
        }

        private string GetConfigurationAsPrintableString()
        {
            return $"isOffline={_isOffline}" +
                   $", pollingInterval (seconds) = {_pollingIntervalInSeconds}" +
                   $", metricsFlushInterval (seconds) = {_metricsFlushIntervalInSeconds}" +
                   $", metricsQueueSize = {_metricsQueueSize}" +
                   $", eventsFlushInterval (seconds) = {_eventsFlushIntervalInSeconds}" +
                   $", eventsQueueSize = {_eventsQueueSize}" +
                   $", host='{_baseUrl}'";
        }
    }
}
