using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace io.unlaunch
{
    public class UnlaunchFeature
    {
        private readonly string _flagKey;
        private readonly string _variationKey;
        private readonly IDictionary<string, string> _properties;
        private readonly string _evaluationReason;

        public UnlaunchFeature(string flagKey, 
            string variationKey, 
            IDictionary<string, string> properties,
            string evaluationReason)
        {
            _flagKey = flagKey;
            _variationKey = variationKey;
            _properties = properties;
            _evaluationReason = evaluationReason;
        }

        public string GetFlag()
        {
            return _flagKey;
        }

        public string GetVariation()
        {
            return _variationKey;
        }

        public IDictionary<string, string> GetVariationConfigAsDictionary()
        {
            if (_properties == null || !_properties.Any())
            {
                return new Dictionary<string, string>();
            }
            
            return new Dictionary<string, string>(_properties);
        }

        public IUnlaunchDynamicConfig GetVariationConfig()
        {
            if (_properties == null || !_properties.Any())
            {
                return new DefaultUnlaunchDynamicConfig(null);
            }
            
            return new DefaultUnlaunchDynamicConfig(new ReadOnlyDictionary<string, string>(_properties));
        }

        public string GetEvaluationReason()
        {
            return _evaluationReason;
        }
    }
}