using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using io.unlaunch.atomic;

namespace io.unlaunch.events
{
    public class CountAggregatorEventHandler : IEventHandler
    {
        private static readonly IUnlaunchLogger Logger = LoggerProvider.For<CountAggregatorEventHandler>();

        private readonly IDictionary<string, AtomicLong> _variationsCountMap = new ConcurrentDictionary<string, AtomicLong>();
        private readonly IEventHandler _eventHandler;
        private readonly Timer _timer;

        public CountAggregatorEventHandler(IEventHandler eventHandler, long runFrequencyInSeconds)
        {
            _eventHandler = eventHandler;
            _timer = new Timer((e) => { CreateFlushEventsTask(); }, null, TimeSpan.Zero, TimeSpan.FromSeconds(runFrequencyInSeconds));
            Logger.Info($"Variation count metrics will be aggregated every {runFrequencyInSeconds} seconds");
        }
        
        public bool Handle(UnlaunchEvent unlaunchEvent)
        {
            IncrementFlagVariation(unlaunchEvent.key, unlaunchEvent.secondaryKey);
            return true;
        }

        public void Flush()
        {
           CreateFlushEventsTask();
        }

        public void Dispose()
        {
            Run();
            _timer.Dispose();
            _eventHandler.Dispose();
        }

        private void CreateFlushEventsTask()
        {
            Task.Factory.StartNew(Run);
        }

        private void IncrementFlagVariation(string flagId, string variationId)
        {
            Logger.Debug($"Incrementing variation {variationId} for flag {flagId}");

            var key = $"{flagId}:{variationId}";
            lock(_variationsCountMap) {
                if (!_variationsCountMap.ContainsKey(key))
                {
                    _variationsCountMap.Add(key, new AtomicLong(0));
                }
            }

            // ReSharper disable once InconsistentlySynchronizedField
            _variationsCountMap[key].IncrementAndGet();
        }

        private void Run()
        {
            if (_variationsCountMap.Any())
            {
                IDictionary<string, AtomicLong> copyOfVariationsCountMap;
                lock(_variationsCountMap)
                {
                    copyOfVariationsCountMap = new Dictionary<string, AtomicLong>(_variationsCountMap);
                    _variationsCountMap.Clear();
                }

                Logger.Debug($"{copyOfVariationsCountMap.Count} flag counts will be sent to the server.");

                foreach (var pair in copyOfVariationsCountMap)
                {
                    var temp = pair.Key.Split(':');
                    var flagKey = temp[0];
                    var mapKey = temp[1];

                    var e = new UnlaunchEvent
                    {
                        type = UnlaunchConstants.FlagInvocationsCountEventType,
                        key = flagKey
                    };
                    e.properties.Add(mapKey, pair.Value.Get());

                    try
                    {
                        _eventHandler.Handle(e);
                        _eventHandler.Flush();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("An error occurred sending event counts to the service", ex);
                    }
                }
            }
        }
    }
}
