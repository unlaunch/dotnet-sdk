﻿using System;

namespace io.unlaunch.events
{
    public class GenericEventHandler : AbstractEventHandler
    {
        public GenericEventHandler(
            string name,
            bool enabled,
            UnlaunchRestWrapper restClientForEventsApi,
            TimeSpan eventFlushInterval,
            int maxBufferSize) : base(name, enabled, restClientForEventsApi, eventFlushInterval, maxBufferSize)
        {

        }
    }
}
