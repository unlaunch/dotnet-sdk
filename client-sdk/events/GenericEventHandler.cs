using System;

namespace io.unlaunch.events
{
    public class GenericEventHandler : AbstractEventHandler
    {
        public GenericEventHandler(
            string name,
            UnlaunchRestWrapper restClientForEventsApi,
            TimeSpan eventFlushInterval,
            int maxBufferSize) : base(name, restClientForEventsApi, eventFlushInterval, maxBufferSize)
        {

        }
    }
}
