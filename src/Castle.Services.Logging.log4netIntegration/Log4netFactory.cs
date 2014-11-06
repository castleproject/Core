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

namespace Castle.Services.Logging.Log4netIntegration
{
	extern alias log4net1;
	using System;
	using System.IO;

	using Castle.Core.Logging;

	using log4net1::log4net;
	using log4net1::log4net.Config;

	public class Log4netFactory : AbstractLoggerFactory
	{
		internal const string defaultConfigFileName = "log4net.config";

		public Log4netFactory() : this(defaultConfigFileName)
		{
		}

		public Log4netFactory(String configFile)
		{
			var file = GetConfigFile(configFile);
			XmlConfigurator.ConfigureAndWatch(file);
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="Log4netFactory" /> class.
		/// </summary>
		/// <param name="configuredExternally">
		///     If <c>true</c> . Skips the initialization of log4net assuming it will happen
		///     externally. Useful if you're using another framework that wants to take over configuration of log4net.
		/// </param>
		public Log4netFactory(bool configuredExternally)
		{
			if (configuredExternally)
			{
				return;
			}

			var file = GetConfigFile(defaultConfigFileName);
			XmlConfigurator.ConfigureAndWatch(file);
		}

		/// <summary>
		///     Configures log4net with a stream containing XML.
		/// </summary>
		/// <param name="config"> </param>
		public Log4netFactory(Stream config)
		{
			XmlConfigurator.Configure(config);
		}

		public override ILogger Create(String name)
		{
			var log = LogManager.GetLogger(name);
			return new Log4netLogger(log, this);
		}

		public override ILogger Create(String name, LoggerLevel level)
		{
			throw new NotSupportedException("Logger levels cannot be set at runtime. Please review your configuration file.");
		}
	}
}