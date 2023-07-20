namespace System.Threading.Tasks
{
    public class AsyncLock : IDisposable
    {
        private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        public async Task LockAsync(Func<Task> worker)
        {
            try
            {
                await semaphoreSlim.WaitAsync();
                await worker();
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                    semaphoreSlim.Dispose();
                disposedValue = true;
            }
        }

        // ~AsyncLock()
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
