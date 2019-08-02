[assembly: System.CLSCompliantAttribute(true)]
[assembly: System.Runtime.InteropServices.ComVisibleAttribute(false)]
[assembly: System.Runtime.Versioning.TargetFrameworkAttribute(".NETFramework,Version=v4.0", FrameworkDisplayName=".NET Framework 4")]
[assembly: System.Security.SecurityRulesAttribute(System.Security.SecurityRuleSet.Level2)]
namespace Castle.Services.Logging.Log4netIntegration
{
    public class ExtendedLog4netFactory : Castle.Core.Logging.AbstractExtendedLoggerFactory
    {
        public ExtendedLog4netFactory() { }
        public ExtendedLog4netFactory(bool configuredExternally) { }
        public ExtendedLog4netFactory(string configFile) { }
        public ExtendedLog4netFactory(System.IO.Stream config) { }
        public override Castle.Core.Logging.IExtendedLogger Create(string name) { }
        public override Castle.Core.Logging.IExtendedLogger Create(string name, Castle.Core.Logging.LoggerLevel level) { }
    }
    public class ExtendedLog4netLogger : Castle.Services.Logging.Log4netIntegration.Log4netLogger, Castle.Core.Logging.IExtendedLogger, Castle.Core.Logging.ILogger
    {
        public ExtendedLog4netLogger(log4net.ILog log, Castle.Services.Logging.Log4netIntegration.ExtendedLog4netFactory factory) { }
        public ExtendedLog4netLogger(log4net.Core.ILogger logger, Castle.Services.Logging.Log4netIntegration.ExtendedLog4netFactory factory) { }
        protected Castle.Services.Logging.Log4netIntegration.ExtendedLog4netFactory Factory { get; set; }
        public Castle.Core.Logging.IContextProperties GlobalProperties { get; }
        public Castle.Core.Logging.IContextProperties ThreadProperties { get; }
        public Castle.Core.Logging.IContextStacks ThreadStacks { get; }
        public override Castle.Core.Logging.ILogger CreateChildLogger(string name) { }
        public Castle.Core.Logging.IExtendedLogger CreateExtendedChildLogger(string name) { }
    }
    public class GlobalContextProperties : Castle.Core.Logging.IContextProperties
    {
        public GlobalContextProperties() { }
        public object this[string key] { get; set; }
    }
    public class Log4netFactory : Castle.Core.Logging.AbstractLoggerFactory
    {
        public Log4netFactory() { }
        public Log4netFactory(string configFile) { }
        public Log4netFactory(bool configuredExternally) { }
        public Log4netFactory(System.IO.Stream config) { }
        public override Castle.Core.Logging.ILogger Create(string name) { }
        public override Castle.Core.Logging.ILogger Create(string name, Castle.Core.Logging.LoggerLevel level) { }
    }
    public class Log4netLogger : System.MarshalByRefObject, Castle.Core.Logging.ILogger
    {
        public Log4netLogger(log4net.Core.ILogger logger, Castle.Services.Logging.Log4netIntegration.Log4netFactory factory) { }
        protected Castle.Services.Logging.Log4netIntegration.Log4netFactory Factory { get; set; }
        public bool IsDebugEnabled { get; }
        public bool IsErrorEnabled { get; }
        public bool IsFatalEnabled { get; }
        public bool IsInfoEnabled { get; }
        public bool IsTraceEnabled { get; }
        public bool IsWarnEnabled { get; }
        protected log4net.Core.ILogger Logger { get; set; }
        public virtual Castle.Core.Logging.ILogger CreateChildLogger(string name) { }
        public void Debug(string message) { }
        public void Debug(System.Func<string> messageFactory) { }
        public void Debug(string message, System.Exception exception) { }
        public void DebugFormat(string format, params object[] args) { }
        public void DebugFormat(System.Exception exception, string format, params object[] args) { }
        public void DebugFormat(System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void DebugFormat(System.Exception exception, System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void Error(string message) { }
        public void Error(System.Func<string> messageFactory) { }
        public void Error(string message, System.Exception exception) { }
        public void ErrorFormat(string format, params object[] args) { }
        public void ErrorFormat(System.Exception exception, string format, params object[] args) { }
        public void ErrorFormat(System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void ErrorFormat(System.Exception exception, System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void Fatal(string message) { }
        public void Fatal(System.Func<string> messageFactory) { }
        public void Fatal(string message, System.Exception exception) { }
        public void FatalFormat(string format, params object[] args) { }
        public void FatalFormat(System.Exception exception, string format, params object[] args) { }
        public void FatalFormat(System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void FatalFormat(System.Exception exception, System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void Info(string message) { }
        public void Info(System.Func<string> messageFactory) { }
        public void Info(string message, System.Exception exception) { }
        public void InfoFormat(string format, params object[] args) { }
        public void InfoFormat(System.Exception exception, string format, params object[] args) { }
        public void InfoFormat(System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void InfoFormat(System.Exception exception, System.IFormatProvider formatProvider, string format, params object[] args) { }
        public override string ToString() { }
        public void Trace(string message) { }
        public void Trace(System.Func<string> messageFactory) { }
        public void Trace(string message, System.Exception exception) { }
        public void TraceFormat(string format, params object[] args) { }
        public void TraceFormat(System.Exception exception, string format, params object[] args) { }
        public void TraceFormat(System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void TraceFormat(System.Exception exception, System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void Warn(string message) { }
        public void Warn(System.Func<string> messageFactory) { }
        public void Warn(string message, System.Exception exception) { }
        public void WarnFormat(string format, params object[] args) { }
        public void WarnFormat(System.Exception exception, string format, params object[] args) { }
        public void WarnFormat(System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void WarnFormat(System.Exception exception, System.IFormatProvider formatProvider, string format, params object[] args) { }
    }
    public class ThreadContextProperties : Castle.Core.Logging.IContextProperties
    {
        public ThreadContextProperties() { }
        public object this[string key] { get; set; }
    }
    public class ThreadContextStack : Castle.Core.Logging.IContextStack
    {
        public ThreadContextStack(log4net.Util.ThreadContextStack log4netStack) { }
        public int Count { get; }
        public void Clear() { }
        public string Pop() { }
        public System.IDisposable Push(string message) { }
    }
    public class ThreadContextStacks : Castle.Core.Logging.IContextStacks
    {
        public ThreadContextStacks() { }
        public Castle.Core.Logging.IContextStack this[string key] { get; }
    }
}