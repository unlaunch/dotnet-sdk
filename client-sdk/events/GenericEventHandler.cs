namespace io.unlaunch.events
{
    public class GenericEventHandler : AbstractEventHandler
    {
        public GenericEventHandler(
            string name,
            UnlaunchRestWrapper restClientForEventsApi,
            int eventFlushIntervalInSeconds,
            int maxBufferSize) : base(name, restClientForEventsApi, eventFlushIntervalInSeconds, maxBufferSize)
        {

        }
    }
}
