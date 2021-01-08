using System.Collections.Generic;

namespace io.unlaunch.engine
{
    public class Variation
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public int RolloutPercentage { get; set; }
        public IDictionary<string, string> Properties { get; set; }
        public string AllowList { get; set; }
    }
}
