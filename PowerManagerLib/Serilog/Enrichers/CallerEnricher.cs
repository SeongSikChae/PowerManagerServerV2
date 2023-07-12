namespace Serilog.Enrichers
{
    using Core;
    using Events;
    using System.Diagnostics;
    using System.Reflection;

    public class CallerEnricher : ILogEventEnricher
    {
        public const string ClassNamePropertyName = "SourceContext";
        public const string MethodNamePropertyName = "MethodName";
        public const string LineNumberPropertyName = "LineNumber";

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            StackTrace stackTrace;
            StackFrame? stackFrame;
            if (logEvent.Exception is not null)
            {
                stackTrace = new StackTrace(logEvent.Exception);
                stackFrame = stackTrace.GetFrame(0);
            }
            else
            {
                stackTrace = new StackTrace(true);
                stackFrame = stackTrace.GetFrame(1);
            }

            if (logEvent.Properties.TryGetValue(ClassNamePropertyName, out LogEventPropertyValue? classNamePropertyValue))
            {
                string? className = ((ScalarValue)classNamePropertyValue).Value?.ToString();
                int _frame = 1;
                do
                {
                    if (stackFrame is null)
                        break;
                    if (!stackFrame.HasMethod())
                        break;
                    Type? t = stackFrame.GetMethod()?.DeclaringType;
                    if (t is null)
                        break;
                    if (t.FullName is null)
                        break;
                    if (t.FullName.Equals(className))
                        break;
                    stackFrame = stackTrace?.GetFrame(_frame++);
                } while (true);
                string method_name = "<unknown method>";
                int line_number = 0;
                if (stackFrame is not null)
                {
                    MethodBase? methodBase = stackFrame.GetMethod();
                    if (methodBase is not null)
                        method_name = methodBase.Name;
                    line_number = stackFrame.GetFileLineNumber();
                }

                logEvent.AddPropertyIfAbsent(new LogEventProperty(MethodNamePropertyName, new ScalarValue(method_name)));
                logEvent.AddPropertyIfAbsent(new LogEventProperty(LineNumberPropertyName, new ScalarValue(line_number)));
            }
        }
    }
}
