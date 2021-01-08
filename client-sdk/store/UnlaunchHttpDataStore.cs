using System;
using System.Collections.Concurrent;
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
using NLog;

namespace io.unlaunch.store
{
    sealed class UnlaunchHttpDataStore : UnlaunchDataStore
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly UnlaunchRestWrapper _restWrapper;
        private readonly CountdownEvent _initialDownloadDoneEvent;
        private readonly AtomicBoolean _downloadSuccessful;
        private readonly IDictionary<string, FeatureFlag> _flagMap;
        private AtomicReference<string> _projectNameRef = new AtomicReference<string>();
        private AtomicReference<string> _environmentNameRef = new AtomicReference<string>();
        private AtomicBoolean _isTaskRunning = new AtomicBoolean(false);
        private readonly AtomicLong _numHttpCalls = new AtomicLong(0);
        private readonly Timer _timer;

        public UnlaunchHttpDataStore(UnlaunchRestWrapper restWrapper, CountdownEvent intInitialDownloadDoneEvent, AtomicBoolean downloadSuccessful, int dataStoreRefreshDelayInSeconds)
        {
            _restWrapper = restWrapper;
            _initialDownloadDoneEvent = intInitialDownloadDoneEvent;
            _downloadSuccessful = downloadSuccessful;
            _flagMap = new ConcurrentDictionary<string, FeatureFlag>();

            _timer = new Timer((e) => { CreateTask(); }, null, TimeSpan.Zero, TimeSpan.FromSeconds(dataStoreRefreshDelayInSeconds));
        }

        public FeatureFlag GetFlag(string flagKey)
        {
            return _flagMap.ContainsKey(flagKey) ? _flagMap[flagKey] : null;
        }

        public IEnumerable<FeatureFlag> GetAllFlags()
        {
            return _flagMap.Values.ToList();
        }

        public bool IsFlagExist(string flagKey)
        {
            return _flagMap.ContainsKey(flagKey);
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
            _flagMap?.Clear();
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
            
            _numHttpCalls.IncrementAndGet();
            var fetchedSuccessfully = false;

            try
            {
                var response = _restWrapper.GetAsync().GetAwaiter().GetResult();
                Console.WriteLine("The http status code is ...: " + (int)response.StatusCode);

                if (response.StatusCode == HttpStatusCode.NotModified)
                {
                    Logger.Debug("synced flags with the server. No update");
                }
                else if (response.StatusCode == HttpStatusCode.OK)
                {
                    var stringResp = response.Content.ReadAsStringAsync().Result;
                    var flagResponse = JsonConvert.DeserializeObject<FlagResponse>(stringResp);
                    _projectNameRef.Set(flagResponse.data.projectName);
                    _environmentNameRef.Set(flagResponse.data.envName);

                    var featureFlags = FlagMapper.GetFeatureFlags(flagResponse.data.flags);
                    foreach (var featureFlag in featureFlags)
                    {
                        _flagMap.Add(featureFlag.Key, featureFlag);
                    }

                    fetchedSuccessfully = true;
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
                Logger.Warn($"unable to fetch flags using REST API, {e.Message}");
            }
            catch (Exception e)
            {
                Logger.Warn($"an error occurred when fetching flags using the REST API, {e.Message}");
            }

            if (fetchedSuccessfully && !_downloadSuccessful.Get())
            {
                _downloadSuccessful.Set(true);
            }

            if (fetchedSuccessfully && !_initialDownloadDoneEvent.IsSet)
            {
                _initialDownloadDoneEvent.Signal();
            }

            _isTaskRunning.Set(false);
        }
    }
}
