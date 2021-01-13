using System;
using System.Collections.Generic;
using io.unlaunch.engine;

namespace io.unlaunch.store
{
    public interface IUnlaunchDataStore : IDisposable
    {
        FeatureFlag GetFlag(string flagKey);
        IEnumerable<FeatureFlag> GetAllFlags();
        bool IsFlagExist(string flagKey);
        string GetProjectName();
        string GetEnvironmentName();
        void RefreshNow();
    }
}