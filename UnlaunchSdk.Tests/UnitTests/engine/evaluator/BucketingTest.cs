using System.Linq;
using FluentAssertions;
using io.unlaunch.model;
using Xunit;

namespace UnlaunchSdk.Tests.UnitTests.engine.evaluator
{
    public class BucketingTest : UnlaunchContext
    {
        [Fact]
        public void HashingValueUnChangeForSameIdentity()
        {
            var featureId = System.Guid.NewGuid().ToString();
            var identity = System.Guid.NewGuid().ToString();
            
            var bucketNumber = -1;
            for (var i = 0; i < 30; i++)
            {
                var bucket = Evaluator.GetBucket(identity, featureId);
                if (bucket < 1)
                {
                    break;
                }

                if (bucketNumber == -1)
                {
                    bucketNumber = bucket;
                }
                else if (bucketNumber != bucket)
                {
                    bucketNumber = -1;
                    break;
                }
            }

            bucketNumber.Should().BeGreaterThan(0);
            bucketNumber.Should().BeLessOrEqualTo(100);
        }

        [Fact]
        public void BucketingWith_50_50_Percent()
        {
            CreateRulesWithRolloutPercentage(50, 50);
            
            var onVariations = 0;
            var offVariations = 0;
            var total = 1000;
            for (var i = 0; i < total; i++)
            {
                var variation = GetFeature(FlagKey, i.ToString(), null).GetVariation();
                if (variation == On)
                {
                    onVariations++;
                }

                if (variation == Off)
                {
                    offVariations++;
                }
            }

            onVariations.Should().BeInRange(450, 550);
            offVariations.Should().BeInRange(450, 550);
            (onVariations + offVariations).Should().Be(total);
        }

        [Fact]
        public void KnownBucketLocations()
        {
            var b = Evaluator.GetBucket("user1", "flag1");
            b.Should().Be(57);

            b = Evaluator.GetBucket("user2", "flag2");
            b.Should().Be(40);
        }

        [Fact]
        public void VariationResultChangeWhenRolloutPercentageChange()
        {
            var identity = "userIdentity";
            CreateRulesWithRolloutPercentage(50, 50);
            var beforeChange = GetFeature(FlagKey, identity, null).GetVariation();
            CreateRulesWithRolloutPercentage(80, 20);
            var afterChange = GetFeature(FlagKey, identity, null).GetVariation();

            beforeChange.Should().Be(Off);
            afterChange.Should().Be(On);
        }

        private void CreateRulesWithRolloutPercentage(int onPercentage, int offPercentage)
        {
            FlagResponse.data.flags.First().rules = new[]
            {
                new TargetRuleDto
                {
                    id = 651,
                    isDefault = true,
                    splits = new []
                    {
                        new SplitDto
                        {
                            id = 891,
                            variationId = OnVariationId,
                            rolloutPercentage = onPercentage
                        },
                        new SplitDto
                        {
                            id = 892,
                            variationId = OffVariationId,
                            rolloutPercentage = offPercentage
                        }
                    },
                    conditions = Enumerable.Empty<TargetRuleConditionDto>(),
                    priority = 0
                }
            };

            LoadFeatureFlags();
        }
    }
}
