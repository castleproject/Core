using Castle.Core.Logging;
using Serilog;
using Serilog.Events;
using ILogger = Castle.Core.Logging.ILogger;

namespace Castle.Services.Logging.SerilogIntegration
{
    public class SerilogFactory : AbstractLoggerFactory
    {
        private LoggerConfiguration configuration;

        public override ILogger Create(string name, LoggerLevel level)
        {
            LogEventLevel serilogLevel;
            switch (level)
            {
                case LoggerLevel.Debug:
                    serilogLevel = LogEventLevel.Debug;
                    break;
                case LoggerLevel.Error:
                    serilogLevel = LogEventLevel.Error;
                    break;
                case LoggerLevel.Fatal:
                    serilogLevel = LogEventLevel.Fatal;
                    break;
                case LoggerLevel.Info:
                    serilogLevel = LogEventLevel.Information;
                    break;
                case LoggerLevel.Off:
                    serilogLevel = LogEventLevel.Information;
                    break;
                case LoggerLevel.Warn:
                    serilogLevel = LogEventLevel.Warning;
                    break;
                default:
                    serilogLevel = LogEventLevel.Information;
                    break;
            }

            var log = configuration
                .MinimumLevel.Is(serilogLevel)
                .CreateLogger();

            return new SerilogLogger(log, this);
        }

        public override ILogger Create(string name)
        {
            var log = configuration
                .CreateLogger();

            return new SerilogLogger(log, this);
        }

        public SerilogFactory(LoggerConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public SerilogFactory()
        {
            configuration = new LoggerConfiguration()
                .MinimumLevel.Is(LogEventLevel.Debug)
                .WriteTo.ColoredConsole();
        }
    }
}
