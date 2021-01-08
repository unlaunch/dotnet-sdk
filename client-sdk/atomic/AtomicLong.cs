using System.Threading;

namespace io.unlaunch.atomic
{
    public class AtomicLong
    {
        private long _value;

        public AtomicLong(int value)
        {
            SetValue(value);
        }

        public long Get()
        {
            return Interlocked.Read(ref _value);
        }

        public long IncrementAndGet()
        {
            return Interlocked.Increment(ref _value);
        }

        public long DecrementAndGet()
        {
            return Interlocked.Decrement(ref _value);
        }

        public void Set(long value)
        {
            SetValue(value);
        }

        private void SetValue(long value)
        {
            Interlocked.Exchange(ref _value, value);
        }
    }
}
