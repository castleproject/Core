// Copyright 2004-2016 Castle Project - http://www.castleproject.org/
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

    public class SerilogFactory : Castle.Core.Logging.AbstractLoggerFactory
    {
        private readonly ILogger logger;

        /// <summary>
        /// Creates a new SerilogFactory using the logger provided by <see cref="Log.Logger"/>.
        /// </summary>
        public SerilogFactory()
        {
            logger = Log.Logger;
        }

        public SerilogFactory(ILogger logger)
        {
            this.logger = logger;
        }

        public override Castle.Core.Logging.ILogger Create(string name)
        {
            return new SerilogLogger(
                logger.ForContext(Serilog.Core.Constants.SourceContextPropertyName, name),
                this);
        }

        public override Castle.Core.Logging.ILogger Create(string name, Castle.Core.Logging.LoggerLevel level)
        {
            throw new NotSupportedException("Logger levels cannot be set at runtime. Please see Serilog's LoggerConfiguration.MinimumLevel.");
        }
    }
}