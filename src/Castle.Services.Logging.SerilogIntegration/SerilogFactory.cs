// Copyright 2004-2012 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.Services.Logging.SerilogIntegration
{
    using Serilog;
    using Serilog.Events;

    public class SerilogFactory : Castle.Core.Logging.AbstractLoggerFactory
    {
        private readonly LoggerConfiguration configuration;

        public override Castle.Core.Logging.ILogger Create(string name, Castle.Core.Logging.LoggerLevel level)
        {
            LogEventLevel serilogLevel;
            switch (level)
            {
                case Castle.Core.Logging.LoggerLevel.Debug:
                    serilogLevel = LogEventLevel.Debug;
                    break;
                case Castle.Core.Logging.LoggerLevel.Error:
                    serilogLevel = LogEventLevel.Error;
                    break;
                case Castle.Core.Logging.LoggerLevel.Fatal:
                    serilogLevel = LogEventLevel.Fatal;
                    break;
                case Castle.Core.Logging.LoggerLevel.Info:
                    serilogLevel = LogEventLevel.Information;
                    break;
                case Castle.Core.Logging.LoggerLevel.Off:
                    serilogLevel = LogEventLevel.Information;
                    break;
                case Castle.Core.Logging.LoggerLevel.Warn:
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

        public override Castle.Core.Logging.ILogger Create(string name)
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