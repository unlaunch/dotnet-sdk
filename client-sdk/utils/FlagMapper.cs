using System.Collections.Generic;
using System.Linq;
using io.unlaunch.engine;
using io.unlaunch.model;
using Newtonsoft.Json;

namespace io.unlaunch.utils
{
    public class FlagMapper
    {
        public static IList<FeatureFlag> GetFeatureFlags(IEnumerable<FlagDto> flags)
        {
            return flags?.Select(GetFeatureFlag).ToList();
        }

        private static FeatureFlag GetFeatureFlag(FlagDto flag)
        {
            var variations = flag.variations?.Select(GetVariation).ToList();
            var rules = GetRules(flag);
            var offVariation = GetVariation(flag.variations?.First(x => x.id == flag.offVariation));

            return new FeatureFlag
            {
                Name = flag.name,
                Key = flag.key,
                Variations = variations,
                PrerequisiteFlags = GetPrerequisiteFlags(flag.prerequisiteFlags),
                Enabled = flag.state == FlagStatus.Active,
                Rules = rules,
                OffVariation = offVariation,
                DefaultRule = rules?.First(x => x.IsDefault()),
                ExpectedVariationKey = null,
                Type = flag.type.ToString()
            };
        }

        private static Variation GetVariation(VariationDto var)
        {
            return new Variation
            {
                Key = var.key,
                Name = var.name,
                AllowList = var.allowList,
                Properties = var.configs
            };
        }

        private static IDictionary<FeatureFlag, Variation> GetPrerequisiteFlags(IDictionary<string, string> preqs)
        {
            if (preqs == null)
            {
                return null;
            }

            var flags = new Dictionary<FeatureFlag, Variation>();

            foreach (var pair in preqs)
            {
                var key = GetFeatureFlag(JsonConvert.DeserializeObject<FlagDto>(pair.Key));
                var value = GetVariation(JsonConvert.DeserializeObject<VariationDto>(pair.Value));
                flags.Add(key, value);
            }

            return flags;
        }

        private static List<Rule> GetRules(FlagDto flag)
        {
            var rules = flag.rules.Select(x => new Rule(x.isDefault, x.priority, 
                x.conditions.Select(GetCondition), GetVariations(x.splits, flag.variations.ToList())));

            return rules.OrderBy(x => x.GetPriority()).ToList();
        }

        private static Condition GetCondition(TargetRuleConditionDto con)
        {
            return new Condition(con.attribute, con.op, con.type, con.value);
        }

        private static IEnumerable<Variation> GetVariations(IEnumerable<SplitDto> splits, List<VariationDto> vars)
        {
            var variations = new List<Variation>();
            foreach (var split in splits)
            {
                var variation = GetVariation(vars.First(x => x.id == split.variationId));
                variation.RolloutPercentage = split.rolloutPercentage;
                variations.Add(variation);
            }

            return variations.OrderBy(x => x.Key);
        }
    }
}
