using System.Collections.Generic;
using System.Linq;

namespace io.unlaunch.engine
{
    public class Rule
    {
        private readonly bool _isDefault;
        private readonly long _priority;
        private readonly IEnumerable<Condition> _conditions;
        private readonly IEnumerable<Variation> _variations;
        
        public Rule(bool isDefault, long priority, IEnumerable<Condition> conditions, IEnumerable<Variation> variations)
        {
            _isDefault = isDefault;
            _priority = priority;
            _conditions = conditions;
            _variations = variations;
        }

        public bool IsDefault()
        {
            return _isDefault;
        }

        public long GetPriority()
        {
            return _priority;
        }

        public IEnumerable<Variation> GetVariations()
        {
            return _variations;
        }

        public bool Matches(UnlaunchUser user)
        {
            return _conditions.All(condition => condition.Match(user));
        }
    }
}