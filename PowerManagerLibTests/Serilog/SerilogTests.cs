using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Serilog.Tests
{
    [TestClass]
    public class SerilogTests
    {
        [TestMethod]
        public void WithMachineNameTest()
        {
            ILogger logger = new LoggerConfiguration()
                .Enrich.WithMachineName()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {MachineName} {Message:lj}{NewLine}{Exception}").CreateLogger();
            logger.Information(string.Empty);
        }

        [TestMethod]
        public void WithEnvironmentUserNameTest()
        {
            ILogger logger = new LoggerConfiguration()
                .Enrich.WithEnvironmentUserName()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {EnvironmentUserName} {Message:lj}{NewLine}{Exception}").CreateLogger();
            logger.Information(string.Empty);
        }

        [TestMethod]
        public void WithEnvironmentNameTest()
        {
            ILogger logger = new LoggerConfiguration()
                .Enrich.WithEnvironmentName()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {EnvironmentName} {Message:lj}{NewLine}{Exception}").CreateLogger();
            logger.Information(string.Empty);
        }

        [TestMethod]
        public void WithProcessIdTest()
        {
            ILogger logger = new LoggerConfiguration()
                .Enrich.WithProcessId()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {ProcessId} {Message:lj}{NewLine}{Exception}").CreateLogger();
            logger.Information(string.Empty);
        }

        [TestMethod]
        public void WithProcessNameTest()
        {
            ILogger logger = new LoggerConfiguration()
                .Enrich.WithProcessName()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {ProcessName} {Message:lj}{NewLine}{Exception}").CreateLogger();
            logger.Information(string.Empty);
        }

        [TestMethod]
        public void WithThreadIdTest()
        {
            ILogger logger = new LoggerConfiguration()
                .Enrich.WithThreadId()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {ThreadId} {Message:lj}{NewLine}{Exception}").CreateLogger();
            logger.Information(string.Empty);
        }

        [TestMethod]
        public void WithThreadNameTest()
        {
            ILogger logger = new LoggerConfiguration()
                .Enrich.WithThreadName()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {ThreadName} {Message:lj}{NewLine}{Exception}").CreateLogger();
            logger.Information(string.Empty);
        }

        [TestMethod]
        public void WithCallerTest()
        {
            ILogger logger = new LoggerConfiguration()
                .Enrich.WithCaller()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}.{MethodName}:{LineNumber} {Message:lj}{NewLine}{Exception}").CreateLogger();
            logger = logger.ForContext(typeof(SerilogTests));
            logger.Information(string.Empty);
        }

        [TestMethod]
        public void WriteToFileTest()
        {
            CountdownEvent countdown = new CountdownEvent(3);

            FileInfo f = new FileInfo("/logging/test.log");

            ILogger logger = new LoggerConfiguration()
                .Enrich.WithThreadName()
                .WriteTo.File(f.FullName, outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {ThreadName} {Message:lj}{NewLine}{Exception}", buffered: false, rollingInterval: RollingInterval.Minute, rollOnFileSizeLimit: false, retainedFileCountLimit: 3, retainedFileTimeLimit: null)
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {ThreadName} {Message:lj}{NewLine}{Exception}").CreateLogger();

            bool stopping = false;

            ThreadPool.QueueUserWorkItem((state) =>
            {
                while (true)
                {
                    if (stopping)
                        break;
                    logger.Information(string.Empty);
                    countdown.Signal();
                    Thread.Sleep(TimeSpan.FromMinutes(1));
                }
            });

            countdown.Wait();
            stopping = true;

            Thread.Sleep(TimeSpan.FromSeconds(5));
        }

        [TestMethod]
        public void WriteToFileTest2()
        {
            CountdownEvent countdown = new CountdownEvent(3);

            FileInfo f = new FileInfo("/logging/test.log");

            ILogger logger = new LoggerConfiguration()
                .Enrich.WithThreadName()
                .WriteTo.File(f.FullName, outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {ThreadName} {Message:lj}{NewLine}{Exception}", buffered: false, rollingInterval: RollingInterval.Minute, rollOnFileSizeLimit: false, retainedFileCountLimit: 31, retainedFileTimeLimit: TimeSpan.FromMinutes(2))
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {ThreadName} {Message:lj}{NewLine}{Exception}").CreateLogger();

            bool stopping = false;

            ThreadPool.QueueUserWorkItem((state) =>
            {
                while (true)
                {
                    if (stopping)
                        break;
                    logger.Information(string.Empty);
                    countdown.Signal();
                    Thread.Sleep(TimeSpan.FromMinutes(1));
                }
            });

            countdown.Wait();
            stopping = true;

            Thread.Sleep(TimeSpan.FromSeconds(5));
        }

        [TestMethod]
        public void WriteToFileTest3()
        {
            CountdownEvent countdown = new CountdownEvent(6);

            FileInfo f = new FileInfo("/logging/test.log");

            ILogger logger = new LoggerConfiguration()
                .Enrich.WithThreadName()
                .WriteTo.File(f.FullName, outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {ThreadName} {Message:lj}{NewLine}{Exception}", buffered: false, rollingInterval: RollingInterval.Infinite, rollOnFileSizeLimit: true, fileSizeLimitBytes: 100, retainedFileCountLimit: 10)
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {ThreadName} {Message:lj}{NewLine}{Exception}").CreateLogger();

            bool stopping = false;

            ThreadPool.QueueUserWorkItem((state) =>
            {
                while (true)
                {
                    if (stopping)
                        break;
                    logger.Information(string.Empty);
                    countdown.Signal();
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            });

            countdown.Wait();
            stopping = true;

            Thread.Sleep(TimeSpan.FromSeconds(5));
        }
    }
}
