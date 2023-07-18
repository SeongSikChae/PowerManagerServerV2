namespace System.Threading.Atomic
{
    public interface IAtomic<T> where T : struct
    {
        public T PreAdd(T newValue);

        public T PostAdd(T newValue);

        public T PreIncrement();

        public T PostIncrement();

        public T PreDecrement();

        public T PostDecrement();

        public bool CompareAndSet(T expect, T update);

        public T Value { get; set; }
    }
}
