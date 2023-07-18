using Microsoft.Extensions.Logging;
using Quartz;

namespace System.Threading.Tasks
{
    public interface ITaskScheduler
    {
        ILoggerFactory LoggerFactory { get; }

        void AddTask(string id, ITask task, TimeSpan interval);

        void AddTask(string id, ITask task, CronExpression cronExpression);

        bool ContainTask(string id);

        DateTime? NextExecutionTime(string id);

        void WakeUp(string id);

        void RemoveTask(string id);

        void WaitForShutdown();
    }
}
