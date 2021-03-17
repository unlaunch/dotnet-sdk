using System;
using System.Threading;
using io.unlaunch.atomic;

namespace io.unlaunch.store
{
    public class RefreshableDataStoreProvider
    {
        private readonly UnlaunchRestWrapper _restWrapper;
        private readonly UnlaunchGenericRestWrapper _s3BucketClient;
        private readonly TimeSpan _dataStoreRefreshDelay;
        private readonly CountdownEvent _initialDownloadDoneEvent;
        private readonly AtomicBoolean _downloadSuccessful;
        private readonly AtomicReference<UnlaunchHttpDataStore> _refreshableUnlaunchFetcherRef = new AtomicReference<UnlaunchHttpDataStore>(null);
        
        public RefreshableDataStoreProvider(
            UnlaunchRestWrapper restWrapper,
            UnlaunchGenericRestWrapper s3BucketClient,
            CountdownEvent initialDownloadDoneEvent,
            AtomicBoolean downloadSuccessful,
            TimeSpan dataStoreRefreshDelay)
        {
            _restWrapper = restWrapper;
            _s3BucketClient = s3BucketClient;
            _dataStoreRefreshDelay = dataStoreRefreshDelay;
            _initialDownloadDoneEvent = initialDownloadDoneEvent;
            _downloadSuccessful = downloadSuccessful;
        }
        
        public IUnlaunchDataStore GetDataStore()
        {
            if (_refreshableUnlaunchFetcherRef.Get() != null)
            {
                return _refreshableUnlaunchFetcherRef.Get();
            }

            var dataStore = new UnlaunchHttpDataStore(_restWrapper, _s3BucketClient, _initialDownloadDoneEvent, _downloadSuccessful, _dataStoreRefreshDelay);
            _refreshableUnlaunchFetcherRef.Set(dataStore);

            return dataStore;
        }

        public IUnlaunchDataStore GetNoOpDataStore()
        {
            return new UnlaunchNoOpDataStore();
        }
    }
}
