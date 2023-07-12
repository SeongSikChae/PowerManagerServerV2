namespace Serilog
{
    using Configuration;
    using Enrichers;

    public static class CallerLoggerConfigurationExtensions
    {
        public static LoggerConfiguration WithCaller(this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            if (enrichmentConfiguration is null)
                throw new ArgumentNullException("enrichmentConfiguration");
            return enrichmentConfiguration.With<CallerEnricher>();
        }
    }
}
