using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using io.unlaunch.atomic;
using io.unlaunch.engine;
using io.unlaunch.exception;
using io.unlaunch.model;
using io.unlaunch.utils;
using Newtonsoft.Json;

namespace io.unlaunch.store
{
    sealed class UnlaunchHttpDataStore : IUnlaunchDataStore
    {
        private static readonly IUnlaunchLogger Logger = LoggerProvider.For<UnlaunchHttpDataStore>();

        private readonly UnlaunchRestWrapper _restWrapper;
        private readonly UnlaunchGenericRestWrapper _s3BucketClient;
        private readonly CountdownEvent _initialDownloadDoneEvent;
        private readonly AtomicBoolean _downloadSuccessful;
        private readonly AtomicReference<IDictionary<string, FeatureFlag>> _flagMapReference;
        private readonly AtomicReference<string> _projectNameRef = new AtomicReference<string>(string.Empty);
        private readonly AtomicReference<string> _environmentNameRef = new AtomicReference<string>(string.Empty);
        private readonly AtomicBoolean _isTaskRunning = new AtomicBoolean(false);
        private readonly AtomicBoolean _sync0Complete = new AtomicBoolean(false);
        private readonly AtomicLong _numHttpCalls = new AtomicLong(0);
        private readonly Timer _timer;

        public UnlaunchHttpDataStore(
            UnlaunchRestWrapper restWrapper,
            UnlaunchGenericRestWrapper s3BucketClient,
            CountdownEvent intInitialDownloadDoneEvent,
            AtomicBoolean downloadSuccessful,
            TimeSpan dataStoreRefreshDelay)
        {
            _restWrapper = restWrapper;
            _s3BucketClient = s3BucketClient;
            _initialDownloadDoneEvent = intInitialDownloadDoneEvent;
            _downloadSuccessful = downloadSuccessful;
            _flagMapReference = new AtomicReference<IDictionary<string, FeatureFlag>>(new Dictionary<string, FeatureFlag>());

            _timer = new Timer((e) => { CreateTask(); }, null, TimeSpan.Zero, dataStoreRefreshDelay);
        }

        public FeatureFlag GetFlag(string flagKey)
        {
            return _flagMapReference.Get().ContainsKey(flagKey) ? _flagMapReference.Get()[flagKey] : null;
        }

        public IEnumerable<FeatureFlag> GetAllFlags()
        {
            return _flagMapReference.Get().Values.ToList();
        }

        public bool IsFlagExist(string flagKey)
        {
            return _flagMapReference.Get().ContainsKey(flagKey);
        }

        public string GetProjectName()
        {
            return _projectNameRef.Get();
        }

        public string GetEnvironmentName()
        {
            return _environmentNameRef.Get();
        }

        public void RefreshNow()
        {
            CreateTask();
        }

        public void Dispose()
        {
            _timer?.Dispose();
            _flagMapReference.Set(new Dictionary<string, FeatureFlag>());
            _isTaskRunning.Set(true);
        }

        private void CreateTask()
        {
            if (!_isTaskRunning.Get())
            {
                Task.Factory.StartNew(GetFlagData);
            }
        }

        private void GetFlagData()
        {
            if (_isTaskRunning.Get())
            {
                return;
            }
            _isTaskRunning.Set(true);

            if (!_sync0Complete.Get())
            {
                _sync0Complete.Set(true);
                if (!Sync0())
                {
                    RegularServerSync();
                }
            }
            else
            {
                RegularServerSync();
            }

            _isTaskRunning.Set(false);
        }

        private bool Sync0()
        {
            try
            {
                var response = _s3BucketClient.GetAsync().GetAwaiter().GetResult();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return false;
                }

                var stringResp = response.Content.ReadAsStringAsync().Result;
                var flagData = JsonConvert.DeserializeObject<Data>(stringResp);
                InitFeatureStore(flagData);

                if (_flagMapReference.Get().Any())
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.Warn("an error occurred when fetching flags from S3", e);
            }
            finally
            {
                _s3BucketClient.Dispose();
            }

            return false;
        }

        private void InitFeatureStore(Data data)
        {
            _projectNameRef.Set(data.projectName);
            _environmentNameRef.Set(data.envName);

            var featureFlags = FlagMapper.GetFeatureFlags(data.flags);
            var flagMap = featureFlags.ToDictionary(x => x.Key);
            _flagMapReference.Set(flagMap);

            if (!_downloadSuccessful.Get())
            {
                _downloadSuccessful.Set(true);
            }

            if (!_initialDownloadDoneEvent.IsSet)
            {
                _initialDownloadDoneEvent.Signal();
            }
        }

        private void RegularServerSync()
        {
            _numHttpCalls.IncrementAndGet();

            try
            {
                var response = _restWrapper.GetAsync().GetAwaiter().GetResult();
                if (response.StatusCode == HttpStatusCode.NotModified)
                {
                    Logger.Debug("synced flags with the server. No update");
                }
                else if (response.StatusCode == HttpStatusCode.OK)
                {
                    var stringResp = response.Content.ReadAsStringAsync().Result;
                    var flagResponse = JsonConvert.DeserializeObject<FlagResponse>(stringResp);
                    InitFeatureStore(flagResponse.data);
                }
                else if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    Logger.Info("The SDK key you provided was rejected by the server and no data was " +
                                "returned. All variation evaluations will return 'control'. You must use the correct " +
                                "SDK key for the project and environment you're connecting to. For more " +
                                "information on how to obtain right SDK keys, see: " +
                                "https://docs.unlaunch.io/docs/sdks/sdk-keys");
                }
                else
                {
                    Logger.Error($"HTTP error downloading features: {response.Content.ReadAsStringAsync().Result} - {response.StatusCode}");
                }
            }
            catch (UnlaunchHttpException e)
            {
                Logger.Warn("unable to fetch flags using REST API", e);
            }
            catch (Exception e)
            {
                Logger.Warn("an error occurred when fetching flags using the REST API", e);
            }
        }
    }
}
