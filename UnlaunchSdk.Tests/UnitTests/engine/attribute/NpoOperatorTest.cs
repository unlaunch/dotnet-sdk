using System.Collections.Generic;
using System.Linq;
using io.unlaunch;
using io.unlaunch.engine;
using io.unlaunch.model;
using Xunit;

namespace UnlaunchSdk.Tests.UnitTests.engine.attribute
{
    public class NpoOperatorTest : UnlaunchContext
    {
        private const string AttributeKey = "attributeKey";

        [Fact]
        public void Set_userSet_is_disjoint()
        {
            CreateNpoCondition(AttributeType.Set, "0,2,3");

            var attributes = new[]
            {
                UnlaunchAttribute.NewSet(AttributeKey, new HashSet<string>(new []{"1", "6"}))
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        [Fact]
        public void Set_userSet_is_super_set()
        {
            CreateNpoCondition(AttributeType.Set, "0,3");

            var attributes = new[]
            {
                UnlaunchAttribute.NewSet(AttributeKey, new HashSet<string>(new []{"0", "3", "6"}))
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        [Fact]
        public void Enumerable()
        {
            CreateNpoCondition(AttributeType.Set, "1,2,3");

            var attributes = new[]
            {
                UnlaunchAttribute.NewSet(AttributeKey, new []{"1", "6"})
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        private void CreateNpoCondition(AttributeType type, string userValue)
        {
            var flag = FlagResponse.data.flags.First();
            flag.rules.First().conditions = new [] { new TargetRuleConditionDto
            {
                id = 119,
                attribute = AttributeKey,
                type = type,
                op = Operator.NPO,
                value = userValue
            }};

            LoadFeatureFlags();
        }
    }
}