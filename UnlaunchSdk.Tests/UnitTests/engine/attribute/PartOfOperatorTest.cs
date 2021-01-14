using System.Collections.Generic;
using System.Linq;
using io.unlaunch;
using io.unlaunch.engine;
using io.unlaunch.model;
using Xunit;

namespace UnlaunchSdk.Tests.UnitTests.engine.attribute
{
    public class PartOfOperatorTest : UnlaunchContext
    {
        private const string AttributeKey = "attributeKey";

        [Fact]
        public void Set_userSet_is_sub_set()
        {
            CreatePartOfCondition(AttributeType.Set, "1,2,3");

            var attributes = new[]
            {
                UnlaunchAttribute.NewSet(AttributeKey, new HashSet<string>(new []{"1", "3"}))
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        [Fact]
        public void Set_userSet_is_same()
        {
            CreatePartOfCondition(AttributeType.Set, "1,2,3");

            var attributes = new[]
            {
                UnlaunchAttribute.NewSet(AttributeKey, new HashSet<string>(new []{"1", "3", "2"}))
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        [Fact]
        public void Enumerable()
        {
            CreatePartOfCondition(AttributeType.Set, "1,2,3");

            var attributes = new[]
            {
                UnlaunchAttribute.NewSet(AttributeKey, new []{"1", "3"})
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        private void CreatePartOfCondition(AttributeType type, string userValue)
        {
            var flag = FlagResponse.data.flags.First();
            flag.rules.First().conditions = new [] { new TargetRuleConditionDto
            {
                id = 119,
                attribute = AttributeKey,
                type = type,
                op = Operator.PO,
                value = userValue
            }};

            LoadFeatureFlags();
        }
    }
}