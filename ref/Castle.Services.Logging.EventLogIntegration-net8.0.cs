[assembly: System.Reflection.AssemblyMetadata("RepositoryUrl", "https://github.com/castleproject/Core")]
[assembly: System.Runtime.Versioning.TargetFramework(".NETCoreApp,Version=v8.0", FrameworkDisplayName=".NET 8.0")]
namespace Castle.Services.Logging.EventLogIntegration
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public class DiagnosticsLogger : Castle.Core.Logging.LevelFilteredLogger, System.IDisposable
    {
        public DiagnosticsLogger(string logName) { }
        public DiagnosticsLogger(string logName, string source) { }
        public DiagnosticsLogger(string logName, string machineName, string source) { }
        public override Castle.Core.Logging.ILogger CreateChildLogger(string loggerName) { }
        public void Dispose() { }
        protected virtual void Dispose(bool disposing) { }
        protected override void Finalize() { }
        protected override void Log(Castle.Core.Logging.LoggerLevel loggerLevel, string loggerName, string message, System.Exception exception) { }
    }
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public class DiagnosticsLoggerFactory : Castle.Core.Logging.AbstractLoggerFactory
    {
        public DiagnosticsLoggerFactory() { }
        public override Castle.Core.Logging.ILogger Create(string name) { }
        public override Castle.Core.Logging.ILogger Create(string name, Castle.Core.Logging.LoggerLevel level) { }
    }
}