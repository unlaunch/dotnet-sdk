using System.Collections.Generic;
using io.unlaunch.engine;

namespace io.unlaunch.model
{
    public class FlagResponse
    {
        public Status status { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public string projectName { get; set; }
        public string envName { get; set; }
        public IEnumerable<FlagDto> flags { get; set; }
    }

    public class FlagDto
    {
        public string key { get; set; }
        public string name { get; set; }
        public FlagType type { get; set; }
        public IEnumerable<VariationDto> variations { get; set; }
        public FlagStatus state { get; set; }
        public int offVariation { get; set; }
        public IEnumerable<TargetRuleDto> rules { get; set; }
        public IDictionary<string, string> prerequisiteFlags { get; set; }
    }

    public class Status
    {
        public string code { get; set; }
    }

    public enum FlagStatus
    {
        Active,
        Inactive
    }

    public enum FlagType
    {
        Boolean,
        String,
        Number
    }

    public class VariationDto
    {
        public int id { get; set; }
        public string key { get; set; }
        public string name { get; set; }
        public IDictionary<string, string> configs { get; set; }
        public string allowList { get; set; }
    }
    
    public class TargetRuleDto
    {
        public int id { get; set; }
        public bool isDefault { get; set; }
        public IEnumerable<TargetRuleConditionDto> conditions { get; set; }
        public IEnumerable<SplitDto> splits { get; set; }
        public int priority { get; set; }
    }

    public class TargetRuleConditionDto
    {
        public int id { get; set; }
        public string attribute { get; set; }
        public AttributeType type { get; set; }
        public string value { get; set; }
        public Operator op { get; set; }
    }

    public class SplitDto
    {
        public int id { get; set; }
        public int variationId { get; set; }
        public int rolloutPercentage { get; set; }
    }
}
