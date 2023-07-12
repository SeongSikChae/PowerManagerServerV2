namespace Microsoft.Extensions.Logging
{
    public static class LoggerExtensions
    {
        public static void Critical(this ILogger logger, string message, params object?[] args)
        {
            logger.LogCritical(message, args);
        }

        public static void Critical(this ILogger logger, string message, Exception? exception = null)
        {
            logger.LogCritical(exception, message);
        }

        public static void Debug(this ILogger logger, string message, params object?[] args)
        {
            logger.LogDebug(message, args);
        }

        public static void Debug(this ILogger logger, string message, Exception? exception = null)
        {
            logger.LogDebug(exception, message);
        }

        public static void Error(this ILogger logger, string message, params object?[] args)
        {
            logger.LogError(message, args);
        }

        public static void Error(this ILogger logger, string message, Exception? exception = null)
        {
            logger.LogError(exception, message);
        }

        public static void Information(this ILogger logger, string message, params object?[] args)
        {
            logger.LogInformation(message, args);
        }

        public static void Information(this ILogger logger, string message, Exception? exception = null)
        {
            logger.LogInformation(exception, message);
        }

        public static void Trace(this ILogger logger, string message, params object?[] args)
        {
            logger.LogTrace(message, args);
        }

        public static void Trace(this ILogger logger, string message, Exception? exception = null)
        {
            logger.LogTrace(exception, message);
        }

        public static void Warning(this ILogger logger, string message, params object?[] args)
        {
            logger.LogWarning(message, args);
        }

        public static void Warning(this ILogger logger, string message, Exception? exception = null)
        {
            logger.LogWarning(exception, message);
        }
    }
}
