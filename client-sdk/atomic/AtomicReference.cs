using System.Threading;

namespace io.unlaunch.atomic
{
    public class AtomicReference<T> where T : class
    {
        private T _t;

        public AtomicReference(T t)
        {
            SetReference(t);
        }

        public void Set(T t)
        {
            SetReference(t);
        }

        public T Get()
        {
            return _t;
        }

        private void SetReference(T t)
        {
            Interlocked.Exchange(ref _t, t);
        }
    }
}