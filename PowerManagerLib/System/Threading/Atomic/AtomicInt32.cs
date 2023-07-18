namespace System.Threading.Atomic
{
    public sealed class AtomicInt32 : IAtomic<int>
    {
        private readonly AtomicInt64 longValue;

        public AtomicInt32(int initialValue = 0)
        {
            longValue = new AtomicInt64(initialValue);
        }

        public int Value
        {
            get => (int)longValue.Value;
            set => longValue.Value = value;
        }

        public int PostAdd(int newValue)
        {
            return (int)longValue.PostAdd(newValue);
        }

        public int PostDecrement()
        {
            return (int)longValue.PostDecrement();
        }

        public int PostIncrement()
        {
            return (int)longValue.PostIncrement();
        }

        public int PreAdd(int newValue)
        {
            return (int)longValue.PreAdd(newValue);
        }

        public int PreDecrement()
        {
            return (int)longValue.PreDecrement();
        }

        public int PreIncrement()
        {
            return (int)longValue.PreIncrement();
        }

        public bool CompareAndSet(int expect, int update)
        {
            return longValue.CompareAndSet(expect, update);
        }
    }
}
