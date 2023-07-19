using Microsoft.Extensions.Logging;

namespace System.IO
{
    using Text;

    public sealed class LoggerProxyWriter : TextWriter
    {
        public void Initialize(ILogger logger)
        {
            this.logger = logger;
        }

        private ILogger? logger;

        public override Encoding Encoding => Encoding.UTF8;

        public override void WriteLine(string? value)
        {
            if (value is not null)
                logger?.Information(value);
        }
    }
}
