using Serilog;

namespace PowerManagerServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration, Program.CmdMain cmdMain, Configuration config)
        {
            this.configuration = configuration;
            this.cmdMain = cmdMain;
            this.config = config;
        }

        private readonly IConfiguration configuration;
        private readonly Program.CmdMain cmdMain;
        private readonly Configuration config;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSerilog(configure =>
            {
                configure.Enrich.WithThreadName().Enrich.WithCaller().WriteTo.Console2().WriteTo.File();
            });
            services.AddControllersWithViews();

            services.AddHsts(configure =>
            {
                configure.Preload = true;
                configure.IncludeSubDomains = true;
                configure.MaxAge = TimeSpan.FromDays(365);
            });
            services.AddSingleton<LoggerProxyWriter>();
            services.AddHttpsRedirection(options =>
            {
                options.HttpsPort = config.WebHttpsPort;
            });
        }

        public void Configure(WebApplication app)
        {
            logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger<Startup>();
            LoggerProxyWriter writer = app.Services.GetRequiredService<LoggerProxyWriter>();
            writer.Initialize(logger);
            this.writer = writer;

            app.Lifetime.ApplicationStarted.Register(OnStarted);
            app.Lifetime.ApplicationStopping.Register(OnShutdown);
            app.Lifetime.ApplicationStopped.Register(OnShutdowned);

            if (!app.Environment.IsDevelopment())
            {
            }

            app.UseWhen(context => context.Connection.LocalPort == config.WebHttpsPort, configure =>
            {
                configure.UseHttpsRedirection();
            });

            app.UseHsts();
            app.UseStaticFiles();
            app.UseRouting();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller}/{action=Index}/{id?}");

            app.MapFallbackToFile("index.html");
        }

        private Microsoft.Extensions.Logging.ILogger<Startup>? logger;
        private LoggerProxyWriter? writer;

        private void OnStarted()
        {
            Program.PrintVersion(writer);
            logger?.Information("PowerManagerServer Started.");
        }

        private void OnShutdown()
        {
            logger?.Information("PowerManagerServer Stopping.");
        }

        private void OnShutdowned()
        {
            logger?.Information("PowerManagerServer Stopped.");
        }
    }
}
