using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using io.unlaunch.atomic;
using io.unlaunch.events;
using io.unlaunch.store;

namespace io.unlaunch
{
    public class UnlaunchClientBuilder : IUnlaunchClientBuilder
    {
        private static readonly IUnlaunchLogger Logger = LoggerProvider.For<UnlaunchClientBuilder>();

        private string _sdkKey;
        private bool _isOffline;
        private TimeSpan _pollingInterval = TimeSpan.FromSeconds(60);
        private TimeSpan _metricsFlushInterval = TimeSpan.FromSeconds(30);
        private TimeSpan _eventsFlushInterval = TimeSpan.FromSeconds(60);
        private TimeSpan _connectionTimeout = TimeSpan.FromSeconds(10);
        private int _metricsQueueSize = 100;
        private int _eventsQueueSize = 500;
        private string _baseUrl = "https://api.unlaunch.io";
        private string _yamlFeaturesFilePath;

        private bool _pollingIntervalUpdatedByUser;
        private bool _metricsFlushIntervalUpdatedByUser;
        private bool _eventsFlushIntervalUpdatedByUser;
        private IHttpClientFactory _httpClientFactory = new DefaultHttpClientFactory();

        private const string FlagApiPath = "/api/v1/flags";
        private const string EventApiPath = "/api/v1/events";
        private const string ImpressionApiPath = "/api/v1/impressions";
        
        public static readonly TimeSpan MinPollInterval = TimeSpan.FromSeconds(15);
        public static readonly TimeSpan MinMetricsFlushInterval = TimeSpan.FromSeconds(15);
        public static readonly TimeSpan MinEventsFlushInterval = TimeSpan.FromSeconds(15);
        public static readonly TimeSpan MinConnectionTimeout = TimeSpan.FromSeconds(10);
        public const int MinEventsQueueSize = 500;
        public const int MinMetricsQueueSize = 100;

        private static readonly IDictionary<string, IUnlaunchClient> Clients = new ConcurrentDictionary<string, IUnlaunchClient>();

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
                Check4DuplicatedClient(client);
            }

            Logger.Info($"client built with following parameters {GetConfigurationAsPrintableString()}");
            
            return client;
        }

        private void Check4DuplicatedClient(IUnlaunchClient client)
        {
            if (Clients.ContainsKey(_sdkKey))
            {
                Logger.Warn("Duplicated Unlaunch client is created. Consider creating only one client per sdkKey");
            }
            else
            {
                Clients.Add(_sdkKey, client);
            }
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

        public IUnlaunchClientBuilder PollingInterval(TimeSpan timeSpan)
        {
            _pollingInterval = timeSpan;
            _pollingIntervalUpdatedByUser = true;
            return this;
        }

        public IUnlaunchClientBuilder ConnectionTimeout(TimeSpan timeSpan)
        {
            _connectionTimeout = timeSpan;
            return this;
        }

        public IUnlaunchClientBuilder Host(string baseUrl)
        {
            _baseUrl = baseUrl;
            return this;
        }

        public IUnlaunchClientBuilder MetricsFlushInterval(TimeSpan timeSpan)
        {
            _metricsFlushInterval = timeSpan;
            _metricsFlushIntervalUpdatedByUser = true;
            return this;
        }

        public IUnlaunchClientBuilder EventsFlushInterval(TimeSpan timeSpan)
        {
            _eventsFlushInterval = timeSpan;
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
            var restWrapper = UnlaunchRestWrapper.Create(_sdkKey, _httpClientFactory.CreateClient(), _baseUrl, FlagApiPath, _connectionTimeout);
            var initialDownloadDoneEvent = new CountdownEvent(1);
            var downloadSuccessful = new AtomicBoolean(false);
            var refreshableDataStoreProvider = new RefreshableDataStoreProvider(restWrapper, initialDownloadDoneEvent, downloadSuccessful, _pollingInterval);
            
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

            var impressionApiRestClient = UnlaunchRestWrapper.Create(_sdkKey, _httpClientFactory.CreateClient(), _baseUrl, ImpressionApiPath, _connectionTimeout);
            var impressionsEventHandler = new GenericEventHandler("metrics-impressions", false, impressionApiRestClient, _metricsFlushInterval, _metricsQueueSize);

            var eventsApiRestClient = UnlaunchRestWrapper.Create(_sdkKey, _httpClientFactory.CreateClient(), _baseUrl, EventApiPath, _connectionTimeout);
            var eventHandler = new GenericEventHandler("generic", true, eventsApiRestClient, _eventsFlushInterval, _eventsQueueSize);
            var variationsCountEventHandler = new CountAggregatorEventHandler(eventHandler, _metricsFlushInterval);

            return UnlaunchClient.Create(dataStore, eventHandler, variationsCountEventHandler, impressionsEventHandler,
                initialDownloadDoneEvent, downloadSuccessful);
        }

        private void LoadPreProductionDefaults()
        {
            if (!_pollingIntervalUpdatedByUser)
            {
                _pollingInterval = MinPollInterval;
            }

            if (!_eventsFlushIntervalUpdatedByUser)
            {
                _eventsFlushInterval = MinEventsFlushInterval;
            }

            if (!_metricsFlushIntervalUpdatedByUser)
            {
                _metricsFlushInterval = MinMetricsFlushInterval;
            }
        }
        
        private void ValidateConfigurationParameters()
        {
            if (!_isOffline)
            {
                if (string.IsNullOrEmpty(_sdkKey))
                {
                    // User didn't supply SDK key, try reading from environment variable
                    var s = Environment.GetEnvironmentVariable(UnlaunchConstants.SdkKeyEnvVariableName);
                    if (string.IsNullOrEmpty(s))
                    {
                        throw new ArgumentException($"sdkKey cannot be null or empty. Must be supplied to the builder or set as an environment variable. {UnlaunchConstants.GetSdkKeyHelpMessage()}");
                    }

                    Logger.Info("Setting SDK Key read from environment variable");
                    _sdkKey = s;
                }

                if (string.IsNullOrEmpty(_baseUrl))
                {
                    throw new ArgumentException("hostname cannot be null or empty. Must point to a valid Unlaunch Service host");
                }
            }

            Contract.EnsuresOnThrow<ArgumentException>(_pollingInterval >= MinPollInterval,
                $"pollingInterval must be great than {MinPollInterval.TotalSeconds} seconds");
            Contract.EnsuresOnThrow<ArgumentException>(_connectionTimeout >= MinConnectionTimeout,
                $"connectionTimeOut must be at least {_connectionTimeout.TotalMilliseconds} milliseconds ");
            Contract.EnsuresOnThrow<ArgumentException>(_connectionTimeout.TotalMilliseconds < int.MaxValue,
                "connectionTimeOut must be less than int.MaxValue={int.MaxValue} milliseconds");
            Contract.EnsuresOnThrow<ArgumentException>(_metricsFlushInterval >= MinMetricsFlushInterval,
                $"metricsFlushInterval must be great than {MinMetricsFlushInterval.TotalSeconds} seconds");
            Contract.EnsuresOnThrow<ArgumentException>(_eventsFlushInterval >= MinEventsFlushInterval,
                $"eventsFlushInterval must be great than {MinEventsFlushInterval.TotalSeconds} seconds");
            Contract.EnsuresOnThrow<ArgumentException>(_eventsQueueSize >= MinEventsQueueSize,
                $"eventsQueue must be at least {MinEventsQueueSize}");
            Contract.EnsuresOnThrow<ArgumentException>(_metricsQueueSize >= MinMetricsQueueSize,
                $"metricsQueueSize must be at least {MinMetricsQueueSize}");
        }

        private string GetConfigurationAsPrintableString()
        {
            return $"isOffline={_isOffline}" +
                   $", pollingInterval (seconds) = {_pollingInterval.TotalSeconds}" +
                   $", metricsFlushInterval (seconds) = {_metricsFlushInterval.TotalSeconds}" +
                   $", metricsQueueSize = {_metricsQueueSize}" +
                   $", eventsFlushInterval (seconds) = {_eventsFlushInterval.TotalSeconds}" +
                   $", eventsQueueSize = {_eventsQueueSize}" +
                   $", host='{_baseUrl}'";
        }
    }
}
