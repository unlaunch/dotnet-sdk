using System;
using System.Linq;
using io.unlaunch;
using io.unlaunch.engine;
using io.unlaunch.model;
using Xunit;

namespace UnlaunchSdk.Tests.UnitTests.engine.attribute
{
    public class InvalidAttributeTypeTest : UnlaunchContext
    {
        private const string AttributeKey = "attributeKey";

        [Fact]
        public void InvalidTypeCast()
        {
            CreateConditions(Operator.EQ);

            var attributes = new[]
            {
                UnlaunchAttribute.NewBoolean(AttributeKey, true)
            };

            Assert.Throws<InvalidCastException>(() => OffVariationTargetingRulesNotMatch(attributes));
        }

        [Fact]
        public void InvalidType()
        {
            CreateConditions(Operator.SW);

            var attributes = new[]
            {
                UnlaunchAttribute.NewBoolean(AttributeKey, true)
            };

            OffVariationTargetingRulesNotMatch(attributes);
        }

        private void CreateConditions(Operator op)
        {
            var flag = FlagResponse.data.flags.First();
            flag.rules.First().conditions = new[] { new TargetRuleConditionDto
            {
                id = 119,
                attribute = AttributeKey,
                type = AttributeType.Number,
                op = op, 
                value = "True"
            }};

            LoadFeatureFlags();
        }
    }
}
