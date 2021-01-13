using System.Threading;
using io.unlaunch.atomic;

namespace io.unlaunch.store
{
    public class RefreshableDataStoreProvider
    {
        private readonly UnlaunchRestWrapper _restWrapper;
        private readonly int _dataStoreRefreshDelayInSeconds;
        private readonly CountdownEvent _initialDownloadDoneEvent;
        private readonly AtomicBoolean _downloadSuccessful;
        private readonly AtomicReference<UnlaunchHttpDataStore> _refreshableUnlaunchFetcherRef = new AtomicReference<UnlaunchHttpDataStore>();
        
        public RefreshableDataStoreProvider(
            UnlaunchRestWrapper restWrapper,
            CountdownEvent initialDownloadDoneEvent,
            AtomicBoolean downloadSuccessful,
            int dataStoreRefreshDelayInSeconds)
        {
            _restWrapper = restWrapper;
            _dataStoreRefreshDelayInSeconds = dataStoreRefreshDelayInSeconds;
            _initialDownloadDoneEvent = initialDownloadDoneEvent;
            _downloadSuccessful = downloadSuccessful;
        }
        
        public IUnlaunchDataStore GetDataStore()
        {
            if (_refreshableUnlaunchFetcherRef.Get() != null)
            {
                return _refreshableUnlaunchFetcherRef.Get();
            }

            var dataStore = new UnlaunchHttpDataStore(_restWrapper, _initialDownloadDoneEvent, _downloadSuccessful, _dataStoreRefreshDelayInSeconds);
            _refreshableUnlaunchFetcherRef.Set(dataStore);

            return dataStore;
        }

        public IUnlaunchDataStore GetNoOpDataStore()
        {
            return new UnlaunchNoOpDataStore();
        }
    }
}
