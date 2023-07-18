namespace System.Threading
{
    using Atomic;
    using Tasks;

    public interface ICountdown : IDisposable
    {
        void AddCount(int signalCount = 1);

        void Rest();

        void Rest(int count);

        bool Signal(int signalCount);

        bool Signal();

        bool TryAddCount();

        bool TryAddCount(int signalCount);

        void Wait();

        bool Wait(int millisecondsTimeout);

        void Wait(CancellationToken cancellationToken);

        bool Wait(TimeSpan timeout);

        bool Wait(int millisecondsTimeout, CancellationToken cancellationToken);

        bool Wait(TimeSpan timeout, CancellationToken cancellationToken);

        int CurrentCount { get; }

        int InitialCount { get; }

        public bool IsSet { get; }

        public WaitHandle WaitHandle { get; }

        public static readonly ICountdown Null = new NullCountdown(0);

        public sealed class NullCountdown : ICountdown
        {
            public NullCountdown(int initialCount = 0)
            {
                this.count = new AtomicInt32(initialCount);
                this.InitialCount = initialCount;
            }

            private readonly AtomicInt32 count;

            public int CurrentCount => count.Value;

            public int InitialCount { get; }

            public bool IsSet => CurrentCount == 0;

            public WaitHandle WaitHandle => throw new NotImplementedException();

            public void AddCount(int signalCount = 1)
            {
                TryAddCount(signalCount);
            }

            public void Rest()
            {
                count.Value = InitialCount;
            }

            public void Rest(int count)
            {
                this.count.Value = count;
            }

            public bool Signal(int signalCount)
            {
                if (IsSet)
                    return false;
                count.PreAdd(-signalCount);
                return true;
            }

            public bool Signal()
            {
                return Signal(1);
            }

            public bool TryAddCount()
            {
                return TryAddCount(1);
            }

            public bool TryAddCount(int signalCount)
            {
                if (!IsSet)
                {
                    count.PreAdd(signalCount);
                    return true;
                }
                return false;
            }

            public void Wait()
            {
                bool flag = Wait(Timeout.InfiniteTimeSpan, CancellationToken.None);
                if (!flag)
                    throw new TimeoutException();
            }

            public bool Wait(int millisecondsTimeout)
            {
                return Wait(TimeSpan.FromMilliseconds(millisecondsTimeout), CancellationToken.None);
            }

            public void Wait(CancellationToken cancellationToken)
            {
                bool flag = Wait(Timeout.InfiniteTimeSpan, cancellationToken);
                if (!flag)
                    throw new TimeoutException();
            }

            public bool Wait(TimeSpan timeout)
            {
                return Wait(timeout, CancellationToken.None);
            }

            public bool Wait(int millisecondsTimeout, CancellationToken cancellationToken)
            {
                return Wait(TimeSpan.FromMilliseconds(millisecondsTimeout), cancellationToken);
            }

            public bool Wait(TimeSpan timeout, CancellationToken cancellationToken)
            {
                try
                {
                    Task.Run(() =>
                    {
                        while (true)
                        {
                            if (IsSet)
                                break;
                            Thread.Sleep(100);
                        }
                    }, cancellationToken).Wait(cancellationToken);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            public void Dispose()
            {
            }
        }

        public sealed class Countdown : ICountdown
        {
            public Countdown(int initialCount = 0)
            {
                this.countdown = new CountdownEvent(initialCount);
            }

            private readonly CountdownEvent countdown;

            public void AddCount(int signalCount = 1)
            {
                countdown.AddCount(signalCount);
            }

            public void Rest()
            {
                countdown.Reset();
            }

            public void Rest(int count)
            {
                countdown.Reset(count);
            }

            public bool Signal(int signalCount)
            {
                return countdown.Signal(signalCount);
            }

            public bool Signal()
            {
                return countdown.Signal();
            }

            public bool TryAddCount()
            {
                return countdown.TryAddCount();
            }

            public bool TryAddCount(int signalCount)
            {
                return countdown.TryAddCount(signalCount);
            }

            public void Wait()
            {
                countdown.Wait();
            }

            public bool Wait(int millisecondsTimeout)
            {
                return countdown.Wait(millisecondsTimeout);
            }

            public void Wait(CancellationToken cancellationToken)
            {
                countdown.Wait(cancellationToken);
            }

            public bool Wait(TimeSpan timeout)
            {
                return countdown.Wait(timeout);
            }

            public bool Wait(int millisecondsTimeout, CancellationToken cancellationToken)
            {
                return countdown.Wait(millisecondsTimeout, cancellationToken);
            }

            public bool Wait(TimeSpan timeout, CancellationToken cancellationToken)
            {
                return countdown.Wait(timeout, cancellationToken);
            }

            public int CurrentCount => countdown.CurrentCount;

            public int InitialCount => countdown.InitialCount;

            public bool IsSet => countdown.IsSet;

            public WaitHandle WaitHandle => countdown.WaitHandle;

            private bool disposedValue;

            private void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                        countdown.Dispose();
                    disposedValue = true;
                }
            }

            // ~Countdown()
            // {
            //     Dispose(disposing: false);
            // }

            public void Dispose()
            {
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }
    }
}
