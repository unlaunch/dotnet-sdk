using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using io.unlaunch.atomic;
using io.unlaunch.utils;

namespace io.unlaunch.events
{
    public class AbstractEventHandler : IEventHandler
    {
        private static readonly IUnlaunchLogger Logger = LoggerProvider.For<AbstractEventHandler>();

        private readonly ConcurrentQueue<UnlaunchEvent> _queue = new ConcurrentQueue<UnlaunchEvent>();
        private readonly UnlaunchRestWrapper _restClient;
        private readonly AtomicBoolean _closed = new AtomicBoolean(false);
        private readonly string _name;
        private readonly int _maxBufferSize;
        private readonly AtomicLong _lastFlushInMillis = new AtomicLong(0);
        private readonly Timer _timer;

        public AbstractEventHandler(
            string name,
            UnlaunchRestWrapper restClientForEventsApi,
            TimeSpan eventFlushInterval,
            int maxBufferSize)
        {
            _name = name;
            _restClient = restClientForEventsApi;
            _maxBufferSize = maxBufferSize;

            _timer = new Timer((e) => { CreateFlushEventsTask(); }, null, TimeSpan.Zero, eventFlushInterval);
        }

        public bool Handle(UnlaunchEvent unlaunchEvent)
        {
            if (unlaunchEvent == null || _closed.Get())
            {
                return false;
            }

            try
            {
                _queue.Enqueue(unlaunchEvent);
                if (_queue.Count >= _maxBufferSize)
                {
                    Logger.Debug("maximum buffer sized reached. flushing.");
                    CreateFlushEventsTask();
                }

                return true;
            }
            catch (ThreadInterruptedException)
            {
                Logger.Warn($"Interrupted while adding event to the queue {unlaunchEvent}. ({_name})");
                return false;
            }
        }

        public void Flush()
        {
            FlushEvent().GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            _closed.Set(true);
            _timer?.Dispose();
            FlushEvent().GetAwaiter().GetResult();
        }

        private void CreateFlushEventsTask()
        {
            Task.Factory.StartNew(FlushEvent);
        }
        
        private async Task FlushEvent()
        {
            if (!_queue.Any())
            {
                return; 
            }

            try
            {
                var events = GetQueuedEvents();
                if (!events.Any())
                {
                    return;
                }
                
                await _restClient.PostAsync(events);
                var unixTime = UnixTime.Get();
                var lastRunTime = _lastFlushInMillis.Get() == 0 ? "never" : $"{(unixTime - _lastFlushInMillis.Get())/1000}";
                Logger.Info($"{events.Count} event(s) submitted. Elapsed time between last run {lastRunTime} seconds ago");

                _lastFlushInMillis.Set(unixTime);
            }
            catch (Exception e)
            {
                Logger.Error("There was an error submitting events to Unlaunch servers", e);
            }
        }

        private List<UnlaunchEvent> GetQueuedEvents()
        {
            var events = new List<UnlaunchEvent>();
            while (_queue.Any())
            {
                var hasItem = _queue.TryDequeue(out var item);
                if (hasItem)
                {
                    events.Add(item);
                }
            }

            return events;
        }
    }
}