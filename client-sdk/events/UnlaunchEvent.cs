using System;
using System.Collections.Generic;
using io.unlaunch.utils;

namespace io.unlaunch.events
{
    public class UnlaunchEvent
    {
        public long createdTime { get; set; } = UnixTime.Get();
        public string type { get; set; }
        public string key { get; set; }
        public object value { get; set; } = new object();
        public IDictionary<string, object> properties { get; set; } = new Dictionary<string, object>();
        public string secondaryKey { get; set; } = string.Empty;
        public string sdk  { get; set; } = "C#";
        public string sdkVersion { get; set; } = "0.0.2";
    }
}