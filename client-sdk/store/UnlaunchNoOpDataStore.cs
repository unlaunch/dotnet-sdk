using System.Collections.Generic;
using io.unlaunch.engine;

namespace io.unlaunch.store
{
    public class UnlaunchNoOpDataStore : IUnlaunchDataStore
    {
        public FeatureFlag GetFlag(string flagKey)
        {
            return null;
        }

        public IEnumerable<FeatureFlag> GetAllFlags()
        {
            return null;
        }

        public bool IsFlagExist(string flagKey)
        {
            return false;
        }

        public string GetProjectName()
        {
            return null;
        }

        public string GetEnvironmentName()
        {
            return null;
        }

        public void RefreshNow()
        {
            
        }

        public void Dispose()
        {
            
        }
    }
}
