using System.Threading;

namespace io.unlaunch.atomic
{
    public class AtomicBoolean
    {
        private int _value;

        public AtomicBoolean(bool value)
        {
            SetValue(value);
        }

        public void Set(bool value)
        {
            SetValue(value);
        }

        public bool Get()
        {
            return Interlocked.CompareExchange(ref _value, 1, 1) == 1;
        }

        private void SetValue(bool value)
        {
            Interlocked.Exchange(ref _value, value ? 1 : 0);
        }
    }
}
