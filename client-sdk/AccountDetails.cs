using System;

namespace io.unlaunch
{
    public class AccountDetails
    {
        private readonly string _projectName;
        private readonly string _environmentName;
        private readonly int _totalFlags;

        public AccountDetails(string projectName, string environmentName, int totalFlags)
        {
            _projectName = projectName;
            _environmentName = environmentName;
            _totalFlags = totalFlags;
        }

        public string GetProjectName()
        {
            return _projectName;
        }

        public string GetEnvironmentName()
        {
            return _environmentName;
        }

        public int GetTotalFlags()
        {
            return _totalFlags;
        }
        
        public override string ToString()
        {
            return "AccountDetails{" +
                   $"projectName='{_projectName}\'" +
                   $", environmentName='{_environmentName}\'" +
                   $", totalFlags={_totalFlags}" +
                   '}';
        }
        
        public override bool Equals(object o)
        {
            if (this == o) return true;
            if (o == null || GetType() != o.GetType()) return false;
            var that = (AccountDetails)o;
            return _totalFlags == that.GetTotalFlags() &&
                   _projectName == that.GetProjectName() &&
                   _environmentName == that.GetEnvironmentName();
        }
        
        public override int GetHashCode()
        {
            var hash = 17;
            hash = hash * 31 + _projectName.GetHashCode();
            hash = hash * 31 + _environmentName.GetHashCode();
            hash = hash * 31 + _totalFlags.GetHashCode();

            return hash;
        }
    }
}
