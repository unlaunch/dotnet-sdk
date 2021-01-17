using System.Collections.Generic;
using System.Linq;
using io.unlaunch;
using io.unlaunch.engine;
using io.unlaunch.model;
using Xunit;

namespace UnlaunchSdk.Tests.UnitTests.engine.attribute
{
    public class HasAllOfOperatorTest : UnlaunchContext
    {
        private const string AttributeKey = "attributeKey";

        [Fact]
        public void Set_userSet_is_super_set()
        {
            CreateAllOfCondition(AttributeType.Set, "2,3,6");

            var attributes = new[]
            {
                UnlaunchAttribute.NewSet(AttributeKey, new HashSet<string>(new []{"1","3","2","6"}))
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        [Fact]
        public void Set_userSet_is_the_same()
        {
            CreateAllOfCondition(AttributeType.Set, "2,3,6");

            var attributes = new[]
            {
                UnlaunchAttribute.NewSet(AttributeKey, new HashSet<string>(new []{"3","2","6"}))
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        [Fact]
        public void Enumerable()
        {
            CreateAllOfCondition(AttributeType.Set, "2,1,3");

            var attributes = new[]
            {
                UnlaunchAttribute.NewSet(AttributeKey, new []{"1","3","2","6"})
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        private void CreateAllOfCondition(AttributeType type, string userValue)
        {
            var flag = FlagResponse.data.flags.First();
            flag.rules.First().conditions = new [] { new TargetRuleConditionDto
            {
                id = 119,
                attribute = AttributeKey,
                type = type,
                op = Operator.AO,
                value = userValue
            }};

            LoadFeatureFlags();
        }
    }
}