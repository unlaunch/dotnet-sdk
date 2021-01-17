namespace io.unlaunch
{
    public class UnlaunchConstants
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

        public static string GetSdkKeyHelpMessage()
        {
            return "To obtain the API key, please sign in to the Unlaunch Console at " +
                   "https://app.unlaunch.io  Then on the right sidebar, click on 'Settings'. Then from the 'Projects' " +
                   "tab, Copy the 'SERVER KEY' for the environment you want to connect to, and provide it to this SDK. " +
                   "For more information, visit: https://docs.unlaunch.io/docs/sdks/sdk-keys";
        }
    }
}
