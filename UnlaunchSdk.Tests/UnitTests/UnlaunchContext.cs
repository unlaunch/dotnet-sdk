using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using io.unlaunch;
using io.unlaunch.engine;
using io.unlaunch.model;
using io.unlaunch.utils;

namespace UnlaunchSdk.Tests.UnitTests
{
    public class UnlaunchContext
    {
        protected const int OnVariationId = 368;
        protected const int OffVariationId = OnVariationId + 1;
        protected const string On = "on";
        protected const string Off = "off";
        protected const string FlagKey = "flagKey";
        protected readonly Evaluator Evaluator = new Evaluator();
        private IDictionary<string, FeatureFlag> _flagMap = new Dictionary<string, FeatureFlag>();

        protected readonly FlagResponse FlagResponse;

        public UnlaunchContext()
        {
            FlagResponse = CreateFlagResponse();
        }

        protected void LoadFeatureFlags()
        {
            _flagMap = FlagMapper.GetFeatureFlags(FlagResponse.data.flags).ToDictionary(x => x.Key);
        }

        protected UnlaunchFeature GetFeature(string flagKey, string identity, IEnumerable<UnlaunchAttribute> attributes)
        {
            if (!_flagMap.ContainsKey(flagKey))
            {
                throw new Exception("Flag doesn't exist");
            }
            var user = attributes == null ? UnlaunchUser.Create(identity) : UnlaunchUser.CreateWithAttributes(identity, attributes);
            
            return Evaluator.Evaluate(_flagMap[flagKey], user);
        }

        protected void OnVariationTargetingRulesMatch(IEnumerable<UnlaunchAttribute> attributes, string flagKey = FlagKey, string identity = "identity")
        {
            OnVariation(flagKey, identity, attributes, "Targeting Rule");
        }

        protected void OffVariationTargetingRulesNotMatch(IEnumerable<UnlaunchAttribute> attributes, string flagKey = FlagKey, string identity = "identity")
        {
            OffVariation(flagKey, identity, attributes);
        }

        protected void OnVariationUserInAllowList(string identity, string flagKey = FlagKey, IEnumerable<UnlaunchAttribute> attributes = null)
        {
            OnVariation(flagKey, identity, attributes, "Target User rules matched");
        }

        protected void OffVariationUserNotInAllowList(string identity, string flagKey = FlagKey, IEnumerable<UnlaunchAttribute> attributes = null)
        {
            OffVariation(flagKey, identity, attributes);
        }

        private void OnVariation(string flagKey, string identity, IEnumerable<UnlaunchAttribute> attributes, string evaluationReason) 
        {
            var feature = GetFeature(flagKey, identity, attributes);
            feature.GetVariation().Should().Be(On);
            feature.GetEvaluationReason().Should().StartWith(evaluationReason);
        }

        private void OffVariation(string flagKey, string identity, IEnumerable<UnlaunchAttribute> attributes)
        {
            var feature = GetFeature(flagKey, identity, attributes);
            feature.GetVariation().Should().Be(Off);
            feature.GetEvaluationReason().Should().StartWith("Default Rule served.");
        }

        private FlagResponse CreateFlagResponse()
        {
            return new FlagResponse
            {
                status = new Status
                {
                    code = "200"
                },
                data = new Data
                {
                    projectName = "TestProject",
                    envName = "Test",
                    flags = new[] { new FlagDto
                    {
                        key = FlagKey,
                        name = "flagName",
                        offVariation = OffVariationId,
                        type = FlagType.String,
                        state = FlagStatus.Active,
                        variations = new []
                        {
                            new VariationDto
                            {
                                id = OnVariationId,
                                key = "on",
                                name = "variation1",
                                allowList = "allowList1,allowList2"
                            },
                            new VariationDto
                            {
                                id = OffVariationId,
                                key = "off",
                                name = "variation2"
                            }
                        },
                        rules = new []
                        {
                            new TargetRuleDto
                            {
                                id = 651,
                                isDefault = false,
                                conditions = Enumerable.Empty<TargetRuleConditionDto>(),
                                splits = new []
                                {
                                    new SplitDto
                                    {
                                        id = 891,
                                        variationId = OnVariationId,
                                        rolloutPercentage = 100
                                    }
                                },
                                priority = 1
                            },
                            new TargetRuleDto
                            {
                                id = 366,
                                isDefault = true,
                                splits = new []
                                {
                                    new SplitDto
                                    {
                                        id = 892,
                                        variationId = OffVariationId,
                                        rolloutPercentage = 100
                                    }
                                },
                                conditions = Enumerable.Empty<TargetRuleConditionDto>(),
                                priority = 0
                            }
                        }
                    }}
                }
            };
        }
    }
}
