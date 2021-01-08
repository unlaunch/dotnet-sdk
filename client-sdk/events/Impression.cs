using System.Net;

namespace io.unlaunch.events
{
    public class Impression : UnlaunchEvent
    {
        public string flagKey { get; set; }
        public string userId { get; set; }
        public string variationKey { get; set; }
        public string flagStatus { get; set; }
        public string evaluationReason { get; set; }
        public string machineName { get; set; } = Dns.GetHostName();
    }
}