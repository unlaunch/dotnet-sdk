using System.Collections.Generic;
using System.Linq;
using io.unlaunch;
using io.unlaunch.engine;
using io.unlaunch.model;
using Xunit;

namespace UnlaunchSdk.Tests.UnitTests.engine.attribute
{
    public class NaoOperatorTest : UnlaunchContext
    {
        private const string AttributeKey = "attributeKey";

        [Fact]
        public void Set()
        {
            CreateEqualsCondition(AttributeType.Set, "2,3,6,8");

            var attributes = new[]
            {
                UnlaunchAttribute.NewSet(AttributeKey, new HashSet<string>(new []{"1","3","2","6"}))
            };

            OnVariation(attributes);
        }

        [Fact]
        public void Enumerable()
        {
            CreateEqualsCondition(AttributeType.Set, "2,1,3,9");

            var attributes = new[]
            {
                UnlaunchAttribute.NewSet(AttributeKey, new []{"1","3","2","6"})
            };

            OnVariation(attributes);
        }

        private void CreateEqualsCondition(AttributeType type, string userValue)
        {
            var flag = FlagResponse.data.flags.First();
            flag.rules.First().conditions = new [] { new TargetRuleConditionDto
            {
                id = 119,
                attribute = AttributeKey,
                type = type,
                op = Operator.NAO,
                value = userValue
            }};

            LoadFeatureFlags();
        }
    }
}