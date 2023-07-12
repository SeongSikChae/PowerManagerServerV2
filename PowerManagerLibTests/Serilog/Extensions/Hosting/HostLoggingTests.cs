using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Serilog.Extensions.Hosting.Tests
{
    [TestClass]
    public class HostLoggingTests
    {
        [TestMethod]
        public void Test()
        {
            CountdownEvent countdown = new CountdownEvent(1);

            IHost host = Host.CreateDefaultBuilder().ConfigureServices(collections =>
            {
                collections.AddSerilog(configure =>
                {
                    configure.Enrich.WithThreadName();
                    configure.Enrich.WithCaller();
                    configure.WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] [{SourceContext}.{MethodName}:{LineNumber}] {Message}{NewLine}{Exception}");
                });
                collections.AddSingleton(countdown);
                collections.AddHostedService<Worker>();
            }).Build();

            host.StartAsync();

            countdown.Wait();

            host.StopAsync();
        }

        private sealed class Worker : IHostedService
        {
            public Worker(CountdownEvent countdown, ILogger<Worker> logger)
            {
                this.countdown = countdown;
                this.logger = logger;
            }

            private readonly CountdownEvent countdown;
            private readonly ILogger<Worker> logger;

            public Task StartAsync(CancellationToken cancellationToken)
            {
                logger.Information("Start");
                countdown.Signal();
                return Task.CompletedTask;
            }

            public Task StopAsync(CancellationToken cancellationToken)
            {
                logger.Information("Stop");
                return Task.CompletedTask;
            }
        }
    }
}
