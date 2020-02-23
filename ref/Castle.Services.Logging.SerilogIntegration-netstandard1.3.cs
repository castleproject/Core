[assembly: System.Runtime.Versioning.TargetFrameworkAttribute(".NETStandard,Version=v1.3", FrameworkDisplayName="")]
namespace Castle.Services.Logging.SerilogIntegration
{
    public class SerilogFactory : Castle.Core.Logging.AbstractLoggerFactory
    {
        public SerilogFactory() { }
        public SerilogFactory(Serilog.ILogger logger) { }
        public override Castle.Core.Logging.ILogger Create(string name) { }
        public override Castle.Core.Logging.ILogger Create(string name, Castle.Core.Logging.LoggerLevel level) { }
    }
    public class SerilogLogger : Castle.Core.Logging.ILogger
    {
        public SerilogLogger(Serilog.ILogger logger, Castle.Services.Logging.SerilogIntegration.SerilogFactory factory) { }
        protected Castle.Services.Logging.SerilogIntegration.SerilogFactory Factory { get; set; }
        public bool IsDebugEnabled { get; }
        public bool IsErrorEnabled { get; }
        public bool IsFatalEnabled { get; }
        public bool IsInfoEnabled { get; }
        public bool IsTraceEnabled { get; }
        public bool IsWarnEnabled { get; }
        protected Serilog.ILogger Logger { get; set; }
        public Castle.Core.Logging.ILogger CreateChildLogger(string loggerName) { }
        public void Debug(string message, System.Exception exception) { }
        public void Debug(System.Func<string> messageFactory) { }
        public void Debug(string message) { }
        public void DebugFormat(System.Exception exception, System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void DebugFormat(System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void DebugFormat(System.Exception exception, string format, params object[] args) { }
        public void DebugFormat(string format, params object[] args) { }
        public void Error(string message, System.Exception exception) { }
        public void Error(System.Func<string> messageFactory) { }
        public void Error(string message) { }
        public void ErrorFormat(System.Exception exception, System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void ErrorFormat(System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void ErrorFormat(System.Exception exception, string format, params object[] args) { }
        public void ErrorFormat(string format, params object[] args) { }
        public void Fatal(string message, System.Exception exception) { }
        public void Fatal(System.Func<string> messageFactory) { }
        public void Fatal(string message) { }
        public void FatalFormat(System.Exception exception, System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void FatalFormat(System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void FatalFormat(System.Exception exception, string format, params object[] args) { }
        public void FatalFormat(string format, params object[] args) { }
        public void Info(string message, System.Exception exception) { }
        public void Info(System.Func<string> messageFactory) { }
        public void Info(string message) { }
        public void InfoFormat(System.Exception exception, System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void InfoFormat(System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void InfoFormat(System.Exception exception, string format, params object[] args) { }
        public void InfoFormat(string format, params object[] args) { }
        public override string ToString() { }
        public void Trace(string message, System.Exception exception) { }
        public void Trace(System.Func<string> messageFactory) { }
        public void Trace(string message) { }
        public void TraceFormat(System.Exception exception, System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void TraceFormat(System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void TraceFormat(System.Exception exception, string format, params object[] args) { }
        public void TraceFormat(string format, params object[] args) { }
        public void Warn(string message, System.Exception exception) { }
        public void Warn(System.Func<string> messageFactory) { }
        public void Warn(string message) { }
        public void WarnFormat(System.Exception exception, System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void WarnFormat(System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void WarnFormat(System.Exception exception, string format, params object[] args) { }
        public void WarnFormat(string format, params object[] args) { }
    }
}