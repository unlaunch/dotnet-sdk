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
        private const string On = "on";
        private const string Off = "off";
        private const string FlagKey = "flagKey";
        private const int OnVariationId = 368;
        private const int OffVariationId = OnVariationId + 1;
        private IDictionary<string, FeatureFlag> _flagMap = new Dictionary<string, FeatureFlag>();
        private readonly Evaluator _evaluator = new Evaluator();

        protected readonly FlagResponse FlagResponse;

        public UnlaunchContext()
        {
            FlagResponse = CreateFlagResponse();
        }

        protected void LoadFeatureFlags()
        {
            _flagMap = FlagMapper.GetFeatureFlags(FlagResponse.data.flags).ToDictionary(x => x.Key);
        }

        protected UnlaunchFeature GetFeature(string identity, IEnumerable<UnlaunchAttribute> attributes, string flagKey = FlagKey)
        {
            if (!_flagMap.ContainsKey(flagKey))
            {
                throw new Exception("Flag doesn't exist");
            }
            var user = attributes == null ? UnlaunchUser.Create(identity) : UnlaunchUser.CreateWithAttributes(identity, attributes);
            
            return _evaluator.Evaluate(_flagMap[flagKey], user);
        }

        protected void OnVariation(IEnumerable<UnlaunchAttribute> attributes)
        {
            var feature = GetFeature("identity", attributes);
            feature.GetVariation().Should().Be(On);
            feature.GetEvaluationReason().Should().StartWith("Targeting Rule");
        }

        protected void OffVariation(IEnumerable<UnlaunchAttribute> attributes)
        {
            var feature = GetFeature("identity", attributes);
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
                                conditions = new []
                                {
                                    new TargetRuleConditionDto
                                    {
                                        id = 119,
                                        attribute = "attribute1",
                                        type = AttributeType.Set,
                                        op = Operator.EQ,
                                        value = "value1,value2"
                                    }
                                },
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
