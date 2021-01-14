using System.Collections.Generic;
using System.Linq;
using io.unlaunch;
using io.unlaunch.engine;
using io.unlaunch.model;
using Xunit;

namespace UnlaunchSdk.Tests.UnitTests.engine.attribute
{
    public class HasAnyOfOperatorTest : UnlaunchContext
    {
        private const string AttributeKey = "attributeKey";

        [Fact]
        public void Set_userSet_is_super_set()
        {
            CreateAnyOfCondition(AttributeType.Set, "3,5,9");

            var attributes = new[]
            {
                UnlaunchAttribute.NewSet(AttributeKey, new HashSet<string>(new []{"1","3","2","6"}))
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        [Fact]
        public void Set_userSet_is_sub_set()
        {
            CreateAnyOfCondition(AttributeType.Set, "3,5,9");

            var attributes = new[]
            {
                UnlaunchAttribute.NewSet(AttributeKey, new HashSet<string>(new []{"3","5"}))
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        [Fact]
        public void Set_userSet_has_some_item()
        {
            CreateAnyOfCondition(AttributeType.Set, "3,5,9");

            var attributes = new[]
            {
                UnlaunchAttribute.NewSet(AttributeKey, new HashSet<string>(new []{"5"}))
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        [Fact]
        public void Enumerable()
        {
            CreateAnyOfCondition(AttributeType.Set, "2,8,0");

            var attributes = new[]
            {
                UnlaunchAttribute.NewSet(AttributeKey, new []{"1","3","2","6"})
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        private void CreateAnyOfCondition(AttributeType type, string userValue)
        {
            var flag = FlagResponse.data.flags.First();
            flag.rules.First().conditions = new [] { new TargetRuleConditionDto
            {
                id = 119,
                attribute = AttributeKey,
                type = type,
                op = Operator.HA,
                value = userValue
            }};

            LoadFeatureFlags();
        }
    }
}