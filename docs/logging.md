# Logging

Castle Project does not contain its own logging framework, there are already excellent frameworks out there. Instead `ILogger` and `ILoggerFactory` are abstractions to decouple Castle libraries from the framework you decide to use.

## Loggers

Castle Core provides the following logger implementations, however you can create your own:

Logger      | Implementation
----------- | --------------
Null        | `Castle.Core.Logging.NullLogFactory` (`Castle.Core.dll`)
Console     | `Castle.Core.Logging.ConsoleFactory` (`Castle.Core.dll`)
Diagnostics | `Castle.Core.Logging.DiagnosticsLoggerFactory` (`Castle.Core.dll`)
Trace       | `Castle.Core.Logging.TraceLoggerFactory` (`Castle.Core.dll`)
Stream      | `Castle.Core.Logging.StreamLoggerFactory` (`Castle.Core.dll`)
[log4net](http://logging.apache.org/log4net/) | `Castle.Services.Logging.Log4netIntegration.Log4netFactory` (`Castle.Services.Logging.Log4netIntegration.dll`)
[log4net](http://logging.apache.org/log4net/) extended | `Castle.Services.Logging.Log4netIntegration.ExtendedLog4netFactory` (`Castle.Services.Logging.Log4netIntegration.dll`)
[NLog](http://nlog-project.org/) | `Castle.Services.Logging.NLogIntegration.NLogFactory` (`Castle.Services.Logging.NLogIntegration.dll`)
[NLog](http://nlog-project.org/) extended | `Castle.Services.Logging.NLogIntegration.ExtendedNLogFactory` (`Castle.Services.Logging.NLogIntegration.dll`)
[Serilog](http://serilog.net/) | `Castle.Services.Logging.SerilogIntegration.SerilogFactory` (`Castle.Services.Logging.SerilogIntegration.dll`)
