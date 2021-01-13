using System;
using System.Linq;
using System.Text;
using io.unlaunch.atomic;
using Murmur;

namespace io.unlaunch.engine
{
    public class Evaluator
    {
        private static readonly IUnlaunchLogger Logger = LoggerProvider.For<Evaluator>();

        public UnlaunchFeature Evaluate(FeatureFlag flag, UnlaunchUser user)
        {
            var evaluationReasonRef = new AtomicReference<string>();
            var variation = EvaluateInternal(flag, user, evaluationReasonRef);

            return new UnlaunchFeature(flag.Key, variation.Key, variation.Properties, evaluationReasonRef.Get());
        }

        private Variation EvaluateInternal(FeatureFlag flag, UnlaunchUser user, AtomicReference<string> evaluationReasonRef)
        {
            if (flag == null)
            {
                throw new ArgumentException("unlaunchFlag must not be null");
            }

            if (user == null)
            {
                throw new ArgumentException("user must not be null");
            }

            Variation variationToServe;
            var evaluationReason = string.Empty;

            if (!flag.Enabled)
            {
                Logger.Debug($"FLAG_DISABLED, {flag.Key}, OFF_VARIATION is served to user {user.GetId()}");
                variationToServe = flag.OffVariation;
                evaluationReason = "Default Variation served. Because the flag is disabled.";
            }
            else if (!CheckDependencies(flag, user))
            {
                Logger.Info($"PREREQUISITE_FAILED for flag {flag.Key}, OFF_VARIATION is served to user {user.GetId()}");
                variationToServe = flag.OffVariation;
                evaluationReason = "Default Variation served. Because Pre-requisite failed.";
            }
            else if ((variationToServe = GetVariationIfUserInAllowList(flag, user)) != null)
            {
                Logger.Info($"USER_IN_TARGET_USER for flag {flag.Key}, VARIATION {variationToServe} is served to user {user.GetId()}");
                evaluationReason = $"Target User rules matched for userId: {user.GetId()}";
            }
            else
            {
                var bucketNumber = GetBucket(user.GetId(), flag.Key);
                foreach (var rule in flag.Rules)
                {
                    if (!rule.IsDefault() && rule.Matches(user))
                    {
                        variationToServe = GetVariationToServeByRule(rule, bucketNumber);
                        Logger.Debug($"RULE_MATCHED for flag {flag.Key}, {variationToServe.Key} Target Rule is served to user {user.GetId()}");
                        evaluationReason = $"Targeting Rule (priority #{rule.GetPriority()}) matched.";
                        break;
                    }
                }

                // No variation matched by rule. Use the default rule.
                if (variationToServe == null)
                {
                    var defaultRule = flag.DefaultRule;
                    variationToServe = GetVariationToServeByRule(defaultRule, bucketNumber);
                    Logger.Debug($"RULE_NOT_MATCHED for flag {flag.Key}, {variationToServe.Key} Default Rule is served to user {user.GetId()}");

                    evaluationReason = "Default Rule served. This is because the flag is Enabled and Target User and Targeting Rules didn't match.";
                }
            }

            evaluationReasonRef?.Set(evaluationReason); 

            return variationToServe;
        }

        public int GetBucket(string userId, string featureId)
        {
            if (userId == null || featureId == null)
            {
                throw new ArgumentException("userId and featureId must not be null");
            }

            var key = userId + featureId;
            var hash = GetHash(key);

            return (int)(Math.Abs(hash % 100) + 1);
        }

        private bool CheckDependencies(FeatureFlag featureFlag, UnlaunchUser user)
        {
            var prerequisiteFlags = featureFlag.PrerequisiteFlags;
            if (prerequisiteFlags == null || !prerequisiteFlags.Any())
            {
                return true;
            }

            foreach (var flag in prerequisiteFlags.Keys)
            {
                var variation = EvaluateInternal(flag, user, null);
                if (variation.Key != prerequisiteFlags[flag].Key)
                {
                    Logger.Info($"PREREQUISITE_FAILED,{flag.Key},{user.GetId()}");
                    return false;
                }
            }

            return true;
        }

        private long GetHash(string key)
        {
            var hashAlgorithm = MurmurHash.Create32(0); 
            var bytes = Encoding.UTF8.GetBytes(key);
            
            return BitConverter.ToUInt32(hashAlgorithm.ComputeHash(bytes, 0, bytes.Length), 0);
        }

        private static Variation GetVariationIfUserInAllowList(FeatureFlag flag, UnlaunchUser user)
        {
            foreach (var variation in flag.Variations)
            {
                if (variation.AllowList != null)
                {
                    var allowList = variation.AllowList.Replace(" ", "").Split(',');

                    if (allowList.Contains(user.GetId()))
                    {
                        return variation;
                    }
                }
            }
            
            return null;
        }

        private Variation GetVariationToServeByRule(Rule rule, int bucketNumber)
        {
            var sum = 0;
            foreach(var variation in rule.GetVariations())
            {
                sum += variation.RolloutPercentage;
                var variationToServe = IsVariationAvailable(sum, bucketNumber) ? variation : null;
                if (variationToServe != null)
                {
                    return variationToServe;
                }
            }
            Logger.Warn($"return null variationToServe. Something went wrong. Rule {rule}, bucketNumber {bucketNumber}");
            
            return null;
        }

        private bool IsVariationAvailable(int rolloutPercent, int bucket)
        {
            return bucket <= rolloutPercent;
        }
    }
}