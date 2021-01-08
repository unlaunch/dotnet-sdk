using System.Collections.Generic;
using System.Linq;

namespace io.unlaunch.engine
{
    public class FeatureFlag
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public IEnumerable<Variation> Variations { get; set; } = Enumerable.Empty<Variation>();
        public IDictionary<FeatureFlag, Variation> PrerequisiteFlags { get; set; }
        public IEnumerable<Rule> Rules { get; set; } = Enumerable.Empty<Rule>();
        public bool Enabled { get; set; }
        public Variation OffVariation { get; set; }
        public Rule DefaultRule { get; set; }
        public string ExpectedVariationKey { get; set; }
        public string Type { get; set; }
    }
}
