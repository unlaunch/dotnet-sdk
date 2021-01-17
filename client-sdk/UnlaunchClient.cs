using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using io.unlaunch.atomic;
using io.unlaunch.engine;
using io.unlaunch.events;
using io.unlaunch.store;

namespace io.unlaunch
{
    public class UnlaunchClient : IUnlaunchClient
    {
        private static readonly IUnlaunchLogger Logger = LoggerProvider.For<UnlaunchClient>();

        protected readonly IEventHandler _defaultEventHandler;
        private readonly IEventHandler _flagInvocationMetricHandler;
        private readonly IEventHandler _impressionsEventHandler;
        private readonly IUnlaunchDataStore _dataStore;
        private readonly Evaluator _evaluator = new Evaluator();
        private readonly AtomicBoolean _shutdownInitiated = new AtomicBoolean(false);
        private readonly CountdownEvent _initialDownloadDoneEvent;
        private readonly AtomicBoolean _downloadSuccessful;

        private UnlaunchClient(IUnlaunchDataStore dataStore,
            IEventHandler eventHandler,
            IEventHandler flagInvocationMetricHandler,
            IEventHandler impressionsEventHandler,
            CountdownEvent initialDownloadDoneEvent,
            AtomicBoolean downloadSuccessful)
        {
            _defaultEventHandler = eventHandler;
            _flagInvocationMetricHandler = flagInvocationMetricHandler;
            _impressionsEventHandler = impressionsEventHandler;
            _dataStore = dataStore;
            _initialDownloadDoneEvent = initialDownloadDoneEvent;
            _downloadSuccessful = downloadSuccessful;
        }

        public static IUnlaunchClient Create(IUnlaunchDataStore dataStore,
            IEventHandler eventHandler,
            IEventHandler flagInvocationMetricHandler,
            IEventHandler impressionsEventHandler,
            CountdownEvent initialDownloadDoneEvent,
            AtomicBoolean downloadSuccessful)
        {
            return new UnlaunchClient(dataStore, eventHandler, flagInvocationMetricHandler, impressionsEventHandler, initialDownloadDoneEvent, downloadSuccessful);
        }

        public static IUnlaunchClient Create()
        {
            return Builder().Build();
        }

        public static IUnlaunchClient Create(string sdkKey)
        {
            return Builder().SdkKey(sdkKey).Build();
        }

        public bool IsReady()
        {
            return _downloadSuccessful.Get();
        }

        public void AwaitUntilReady(TimeSpan timeSpan)
        {
            var isReady = _initialDownloadDoneEvent.Wait((int)timeSpan.TotalMilliseconds);
            if (!isReady)
            {
                Logger.Error($"Unlaunch client didn't finish initialization in {timeSpan.TotalMilliseconds} milliseconds. The download could still be in progress. Check logs for any errors.");
                throw new TimeoutException($"Unlaunch client was not ready in {timeSpan.TotalMilliseconds} milliseconds");
            }
        }

        public void Shutdown()
        {
            if (_shutdownInitiated.Get())
            {
                Logger.Debug("shutdown already initiated on the client. It is safe to ignore this message");
            }
            else
            {
                Logger.Debug("client shutdown called");
                _shutdownInitiated.Set(true);
                Dispose();
            }
        }

        public AccountDetails AccountDetails()
        {
            if (!IsReady())
            {
                Logger.Error("The client isn't ready yet. You can call accountDetails() method when the client is ready.");
                return new AccountDetails("client_not_ready", "client_not_ready", -1);
            }

            return new AccountDetails(_dataStore.GetProjectName(), _dataStore.GetEnvironmentName(), _dataStore.GetAllFlags().Count());
        }

        public UnlaunchFeature GetFeature(string flagKey, string identity)
        {
            return Evaluate(flagKey, identity, null);
        }

        public UnlaunchFeature GetFeature(string flagKey, string identity, IEnumerable<UnlaunchAttribute> attributes)
        {
            return Evaluate(flagKey, identity, attributes);
        }

        public string GetVariation(string flagKey, string identity)
        {
            return GetVariation(flagKey, identity, null);
        }

        public string GetVariation(string flagKey, string identity, IEnumerable<UnlaunchAttribute> attributes)
        {
            var feature = Evaluate(flagKey, identity, attributes);
            return feature.GetVariation();
        }

        public static UnlaunchClientBuilder Builder()
        {
            return new UnlaunchClientBuilder();
        }

        public void Dispose()
        {
            _dataStore.Dispose();
            _impressionsEventHandler.Dispose();
            _flagInvocationMetricHandler.Dispose();
            _defaultEventHandler.Dispose();
        }

        private UnlaunchFeature Evaluate(string flagKey, string identity, IEnumerable<UnlaunchAttribute> attributes)
        {
            if (string.IsNullOrEmpty(flagKey))
            {
                throw new ArgumentException("flagKey must not be null or empty");
            }

            if (string.IsNullOrWhiteSpace(identity))
            {
                throw new ArgumentException($"userId must be a string and cannot contain any whitespace characters: {identity}");
            }

            if (_shutdownInitiated.Get())
            {
                Logger.Debug($"Asked to evaluate flag {flagKey} but shutdown already initiated on the client");
                return UnlaunchConstants.GetControlFeatureByName(flagKey);
            }

            if (!IsReady())
            {
                Logger.Warn("the SDK is not ready. Returning the SDK default 'control' as variation which may not give the right result");
                return UnlaunchConstants.GetControlFeatureByName(flagKey);
            }

            var user = attributes == null ? UnlaunchUser.Create(identity) : UnlaunchUser.CreateWithAttributes(identity, attributes);

            FeatureFlag flag;
            try
            {
                flag = _dataStore.GetFlag(flagKey);
            }
            catch (Exception e)
            {
                return new UnlaunchFeature(flagKey, UnlaunchConstants.FlagDefaultReturnType,
                    null, $"there was an error fetching flag: {e.Message}");
            }        

            if (flag == null)
            {
                Logger.Warn($"UnlaunchFeature '{flagKey}' not found in the data store. Returning 'control' variation");
                return new UnlaunchFeature(flagKey, UnlaunchConstants.FlagDefaultReturnType, 
                    null, "flag was not found in the in-memory cache");
            }

            var result = _evaluator.Evaluate(flag, user);
            var impression = new Impression
            {
                type = UnlaunchConstants.EventTypeForImpressionEvents,
                key = flag.Key,
                secondaryKey = result.GetVariation(),
                flagKey = flag.Key,
                flagStatus = flag.Enabled.ToString(),
                evaluationReason = result.GetEvaluationReason(),
                userId = user.GetId(),
                variationKey = result.GetVariation()
            };
            Track(impression);

            return result;
        }

        private void Track(Impression impression)
        {
            if (_shutdownInitiated.Get())
            {
                Logger.Error("Cannot track impression because client shutdown is already initiated.");
            }
            else
            {
                _flagInvocationMetricHandler.Handle(impression);
                _impressionsEventHandler.Handle(impression);
            }
        }
    }
}
