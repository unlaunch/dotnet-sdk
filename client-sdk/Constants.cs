namespace io.unlaunch.utils
{
    public class Constants
    {

        public static readonly string SdkKeyEnvVariableName = "UNLAUNCH_SDK_KEY";

        public static readonly string FlagInvocationsCountEventType = "VARIATIONS_COUNT_EVENT";

        public static readonly string EventTypeForImpressionEvents = "IMPRESSION";

        public static readonly string FlagDefaultReturnType = "control";

        public static UnlaunchFeature GetControlFeatureByName(string flagKey)
        {
            return new UnlaunchFeature(flagKey, FlagDefaultReturnType, null,
                "Unable to retrieve feature. Returning control variation");
        }
    }
}
