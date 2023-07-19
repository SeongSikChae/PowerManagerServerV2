namespace Serilog
{
    using Configuration;

    public static class TemplateLoggerConfigurationExtensions
    {
        public static LoggerConfiguration File(this LoggerSinkConfiguration sinkConfiguration, string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] [{SourceContext}.{MethodName}:{LineNumber}] {Message}{NewLine}{Exception}", long fileSizeLimitBytes = 104857600, RollingInterval rollingInterval = RollingInterval.Day, bool rollOnFileSizeLimit = true, int retainedFileCountLimit = 31, TimeSpan? retainedFileTimeLimit = null)
        {
            string logDir = Environment.GetEnvironmentVariable("log.dir") ?? ".";
            string logFile = Environment.GetEnvironmentVariable("log.file") ?? "log.log";

            DirectoryInfo directoryInfo = new DirectoryInfo(logDir);
            if (!directoryInfo.Exists)
                directoryInfo.Create();

            FileInfo f = directoryInfo.GetFileInfo(logFile);

            if (sinkConfiguration is null)
                throw new ArgumentNullException("sinkConfiguration");
            return sinkConfiguration.File(f.FullName, outputTemplate: outputTemplate, fileSizeLimitBytes: fileSizeLimitBytes, rollingInterval: rollingInterval, rollOnFileSizeLimit: rollOnFileSizeLimit, retainedFileCountLimit: retainedFileCountLimit, retainedFileTimeLimit: retainedFileTimeLimit);
        }

        public static LoggerConfiguration Console2(this LoggerSinkConfiguration sinkConfiguration, string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] [{SourceContext}.{MethodName}:{LineNumber}] {Message}{NewLine}{Exception}")
        {
            if (sinkConfiguration is null)
                throw new ArgumentNullException("sinkConfiguration");
            return sinkConfiguration.Console(outputTemplate: outputTemplate);
        }
    }
}
