using Microsoft.Extensions.Logging;
using Quartz;

namespace System.Threading.Tasks
{
    using Collections.Concurrent;
    using Atomic;

    public delegate void StoppedTaskHandler(string taskId, ITask task);

    public sealed class CustomTaskScheduler : ITaskScheduler
    {
        public CustomTaskScheduler(ILogger<CustomTaskScheduler> logger, ILoggerFactory loggerFactory)
        {
            this.logger = logger;
            this.LoggerFactory = loggerFactory;
        }

        private readonly ILogger<CustomTaskScheduler> logger;
        private readonly ConcurrentDictionary<string, ITaskWorker> taskDictionary = new();
        private bool waitForShutdown = false;

        public ILoggerFactory LoggerFactory { get; private set; }

        public void AddTask(string id, ITask task, TimeSpan interval)
        {
            if (waitForShutdown)
                return;
            if (taskDictionary.ContainsKey(id))
                throw new Exception($"Task '{id}' exist already");
            ITaskWorker worker = new IntervalTaskWorker(id, task, interval, LoggerFactory.CreateLogger<IntervalTaskWorker>());
            worker.StoppedTaskEvent += (tasiId, jobTask) =>
            {
                if (StoppedTaskEvent is not null)
                    StoppedTaskEvent(tasiId, jobTask);
            };
            worker.Start();
            taskDictionary.TryAdd(id, worker);
        }

        public void AddTask(string id, ITask task, CronExpression cronExpression)
        {
            if (waitForShutdown)
                return;
            if (taskDictionary.ContainsKey(id))
                throw new Exception($"Task '{id}' exist already");
            ITaskWorker worker = new CronTaskWorker(id, task, cronExpression, LoggerFactory.CreateLogger<CronTaskWorker>());
            worker.StoppedTaskEvent += (tasiId, jobTask) =>
            {
                if (StoppedTaskEvent is not null)
                    StoppedTaskEvent(tasiId, jobTask);
            };
            worker.Start();
            taskDictionary.TryAdd(id, worker);
        }

        public bool ContainTask(string id)
        {
            return taskDictionary.ContainsKey(id);
        }

        public DateTime? NextExecutionTime(string id)
        {
            if (taskDictionary.ContainsKey(id))
                return taskDictionary[id].NextExecutionTime();
            else
                return null;
        }

        public void WakeUp(string id)
        {
            if (taskDictionary.TryGetValue(id, out ITaskWorker? worker))
                worker.WakeUp();
        }

        public void RemoveTask(string id)
        {
            if (taskDictionary.TryGetValue(id, out ITaskWorker? worker))
            {
                worker.Stop();
                taskDictionary.TryRemove(id, out _);
            }
        }

        public void WaitForShutdown()
        {
            waitForShutdown = true;
            IEnumerable<string> idEnumerable = taskDictionary.Keys.ToList();
            foreach (string id in idEnumerable)
            {
                logger.Information($"shutdown wait '{id}");
                taskDictionary[id].Join();
                logger.Information($"shutdown completed '{id}'");
            }
        }

        public event StoppedTaskHandler? StoppedTaskEvent;

        private interface ITaskWorker
        {
            DateTime? NextExecutionTime();

            void Start();

            void WakeUp();

            void Run();

            void Join();

            void Stop();

            event StoppedTaskHandler StoppedTaskEvent;
        }

        private sealed class IntervalTaskWorker : ITaskWorker
        {
            public IntervalTaskWorker(string id, ITask jobTask, TimeSpan interval, ILogger<IntervalTaskWorker> logger)
            {
                this.id = id;
                this.jobTask = jobTask;
                this.interval = interval;
                this.logger = logger;
            }

            private readonly string id;
            private readonly ITask jobTask;
            private readonly TimeSpan interval;
            private readonly ILogger<IntervalTaskWorker> logger;
            private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            private Task? task;
            private CancellationTokenSource interruptTokenSource = new CancellationTokenSource();
            private readonly AtomicInt64 nextExecutionTime = new AtomicInt64(-1);

            public DateTime? NextExecutionTime()
            {
                return nextExecutionTime.Value > -1 ? nextExecutionTime.Value.FromMilliseconds(TimeZoneInfo.Local) : null;
            }

            public void Start()
            {
                logger.Information($"task '{id}' starting.");
                task = new Task(Run);
                task.Start();
            }

            public void WakeUp()
            {
                interruptTokenSource.Cancel();
            }

            public void Run()
            {
                Thread.CurrentThread.Name = id;
                logger.Information($"task '{id}' started.");
                while (true)
                {
                    if (cancellationTokenSource.IsCancellationRequested)
                        break;
                    try
                    {
                        try
                        {
                            nextExecutionTime.Value = DateTime.Now.AddMilliseconds(interval.TotalMilliseconds).ToMilliseconds();
                            task?.Wait((int)interval.TotalMilliseconds, interruptTokenSource.Token);
                        }
                        catch (OperationCanceledException)
                        {
                            interruptTokenSource = new CancellationTokenSource();
                        }
                        if (cancellationTokenSource.IsCancellationRequested)
                            break;
                        if (jobTask.Async)
                            jobTask.RunAsync(cancellationTokenSource.Token).Wait();
                        else
                            jobTask.Run(cancellationTokenSource.Token);
                        if (jobTask.Type == ITask.TaskType.OneTime)
                        {
                            Task.Run(() =>
                            {
                                if (StoppedTaskEvent is not null)
                                    StoppedTaskEvent(id, jobTask);
                            });
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Error($"error in task '{id}'", e);
                    }
                }
                if (jobTask.Type == ITask.TaskType.NoOneTime)
                    logger.Information($"task '{id}' stopped.");
            }

            public void Join()
            {
                task?.Wait(cancellationTokenSource.Token);
            }

            public void Stop()
            {
                logger.Information($"task '{id}' stopping.");
                cancellationTokenSource.Cancel();
                interruptTokenSource.Cancel();
                task?.Wait();
                if (jobTask.Type == ITask.TaskType.OneTime)
                    logger.Information($"task '{id}' stopped.");
            }

            public event StoppedTaskHandler? StoppedTaskEvent;
        }

        private sealed class CronTaskWorker : ITaskWorker
        {
            public CronTaskWorker(string id, ITask jobTask, CronExpression cronExpression, ILogger<CronTaskWorker> logger)
            {
                this.id = id;
                this.jobTask = jobTask;
                this.cronExpression = cronExpression;
                this.logger = logger;
            }

            private readonly string id;
            private readonly ITask jobTask;
            private readonly CronExpression cronExpression;
            private readonly ILogger<CronTaskWorker> logger;
            private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            private Task? task;
            private CancellationTokenSource interruptTokenSource = new CancellationTokenSource();
            private AtomicInt64 nextExecutionTime = new AtomicInt64(-1);

            public DateTime? NextExecutionTime()
            {
                return nextExecutionTime.Value > -1 ? nextExecutionTime.Value.FromMilliseconds(TimeZoneInfo.Local) : null;
            }

            public void Start()
            {
                logger.Information($"task '{id}' starting.");
                task = new Task(Run);
                task.Start();
            }

            public void WakeUp()
            {
                interruptTokenSource.Cancel();
            }

            public void Run()
            {
                Thread.CurrentThread.Name = id;
                logger.Information($"task '{id}' started.");
                while (true)
                {
                    if (cancellationTokenSource.IsCancellationRequested)
                        break;
                    try
                    {
                        try
                        {
                            DateTimeOffset currentOffset = DateTimeOffset.UtcNow;
                            DateTimeOffset? afterOffset = cronExpression.GetTimeAfter(currentOffset);
                            if (afterOffset.HasValue)
                            {
                                TimeSpan span = afterOffset.Value.AddSeconds(TimeZoneInfo.Local.BaseUtcOffset.TotalSeconds) - currentOffset.AddSeconds(TimeZoneInfo.Local.BaseUtcOffset.TotalSeconds);
                                nextExecutionTime.Value = DateTime.Now.AddMilliseconds(span.TotalMilliseconds).ToMilliseconds();
                                task?.Wait((int)span.TotalMilliseconds, interruptTokenSource.Token);
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            interruptTokenSource = new CancellationTokenSource();
                        }
                        if (cancellationTokenSource.IsCancellationRequested)
                            break;
                        if (jobTask.Async)
                            jobTask.RunAsync(cancellationTokenSource.Token).Wait();
                        else
                            jobTask.Run(cancellationTokenSource.Token);
                        if (jobTask.Type == ITask.TaskType.OneTime)
                        {
                            Task.Run(() =>
                            {
                                if (StoppedTaskEvent is not null)
                                    StoppedTaskEvent(id, jobTask);
                            });
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Error($"error in task '{id}'", e);
                    }
                }
                if (jobTask.Type == ITask.TaskType.NoOneTime)
                    logger.Information($"task '{id}' stopped.");
            }

            public void Join()
            {
                task?.Wait(cancellationTokenSource.Token);
            }

            public void Stop()
            {
                logger.Information($"task '{id}' stopping.");
                cancellationTokenSource.Cancel();
                interruptTokenSource.Cancel();
                task?.Wait();
                if (jobTask.Type == ITask.TaskType.OneTime)
                    logger.Information($"task '{id}' stopped.");
            }

            public event StoppedTaskHandler? StoppedTaskEvent;
        }
    }
}
