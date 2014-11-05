// Copyright 2004-2014 Castle Project - http://www.castleproject.org/
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
    using System;

    using Serilog;
    using Serilog.Events;

    public class SerilogFactory : Castle.Core.Logging.AbstractLoggerFactory
    {
        private readonly LoggerConfiguration configuration;

        /// <summary>
        /// Creates a new SerilogFactory with the <c>MinimumLevel</c> set to <c>LogEventLevel.Debug</c>
        /// writing to the console.
        /// </summary>
        public SerilogFactory()
        {
            configuration = new LoggerConfiguration()
                .MinimumLevel.Is(LogEventLevel.Debug) // Default to debug rather than info so all events show up
                .WriteTo.ColoredConsole();
        }

        public SerilogFactory(LoggerConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public override Castle.Core.Logging.ILogger Create(string name)
        {
            ILogger logger = configuration
                .CreateLogger();

            return new SerilogLogger(logger, this);
        }

        public override Castle.Core.Logging.ILogger Create(string name, Castle.Core.Logging.LoggerLevel level)
        {
            throw new NotSupportedException("Logger levels cannot be set at runtime. Please see Serilog's LoggerConfiguration.MinimumLevel.");
        }
    }
}