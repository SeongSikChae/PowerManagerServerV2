namespace System.Threading.Atomic
{
    public sealed class AtomicInt64 : IAtomic<long>
    {
        private long atomicValue;

        public long Value
        {
            get => Interlocked.Read(ref atomicValue);
            set => Interlocked.Exchange(ref atomicValue, value);
        }

        public AtomicInt64(long initialValue = 0)
        {
            atomicValue = initialValue;
        }

        public long PreAdd(long newValue)
        {
            return Interlocked.Add(ref atomicValue, newValue);
        }

        public long PostAdd(long newValue)
        {
            return Interlocked.Exchange(ref atomicValue, atomicValue + newValue);
        }

        public long PreIncrement()
        {
            return Interlocked.Increment(ref atomicValue);
        }

        public long PostIncrement()
        {
            return Interlocked.Exchange(ref atomicValue, atomicValue + 1);
        }

        public long PreDecrement()
        {
            return Interlocked.Decrement(ref atomicValue);
        }

        public long PostDecrement()
        {
            return Interlocked.Exchange(ref atomicValue, atomicValue - 1);
        }

        public bool CompareAndSet(long expect, long update)
        {
            long original = Interlocked.CompareExchange(ref atomicValue, update, expect);
            return original == expect;
        }
    }
}
