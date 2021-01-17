using System;
using System.Collections.Generic;

namespace io.unlaunch
{
    public class OfflineUnlaunchClient : IUnlaunchClient
    {
        private readonly IDictionary<string, UnlaunchFeature> _dataStore = new Dictionary<string, UnlaunchFeature>();

        public OfflineUnlaunchClient()
        {

        }

        public OfflineUnlaunchClient(string yamlFeaturesFilePath)
        {

        }
        
        public void Dispose()
        {
            
        }

        public bool IsReady()
        {
            return true;
        }

        public void AwaitUntilReady(int millisecondsTimeout)
        {
            
        }

        public void AwaitUntilReady(TimeSpan timeout)
        {
            
        }

        public void Shutdown()
        {
            
        }

        public AccountDetails AccountDetails()
        {
            return new AccountDetails("client_is_in_offline_mode", "offine_mode", -1);
        }

        public UnlaunchFeature GetFeature(string flagKey, string identity)
        {
            return GetFeature(flagKey, identity, null);
        }

        public UnlaunchFeature GetFeature(string flagKey, string identity, IEnumerable<UnlaunchAttribute> attributes)
        {
            if (_dataStore.ContainsKey(flagKey))
            {
                return _dataStore[flagKey];
            }
            
            return new UnlaunchFeature(flagKey,
                UnlaunchConstants.FlagDefaultReturnType,
                new Dictionary<string, string>(1),
                "Client is initialized in Offline Mode. Returning 'control' variation for all flags.");
        }

        public string GetVariation(string flagKey, string identity)
        {
            return GetVariation(flagKey, identity, null);
        }

        public string GetVariation(string flagKey, string identity, IEnumerable<UnlaunchAttribute> attributes)
        {
            return GetFeature(flagKey, identity, attributes).GetVariation();
        }
    }
}
