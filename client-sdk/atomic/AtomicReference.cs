using System.Threading;

namespace io.unlaunch.atomic
{
    public class AtomicReference<T> where T : class
    {
        private T _t;

        public void Set(T t) 
        {
            Interlocked.Exchange(ref _t, t);
        }

        public T Get()
        {
            return _t;
        }
    }
}