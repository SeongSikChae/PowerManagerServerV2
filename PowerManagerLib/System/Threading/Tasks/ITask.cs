namespace System.Threading.Tasks
{
    public interface ITask
    {
        public enum TaskType
        {
            OneTime, NoOneTime
        }

        public TaskType Type { get; }

        public bool Async { get; }

        void Run(CancellationToken cancellationToken);

        Task RunAsync(CancellationToken cancellationToken);
    }
}
