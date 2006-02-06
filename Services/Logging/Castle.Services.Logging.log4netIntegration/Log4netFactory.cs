// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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
	using System;
	using System.IO;

	using Castle.Services.Logging;
	
	using log4net;
	using log4net.Config;

	/// <summary>
	/// Summary description for log4netFactory.
	/// </summary>
	public class Log4netFactory : AbstractLoggerFactory
	{
		public Log4netFactory() : this ("log4net.config")
		{
		}

		public Log4netFactory(String configFile)
		{
			FileInfo file = GetConfigFile(configFile);
			XmlConfigurator.ConfigureAndWatch(file);
		}

		public override ILogger Create(String name)
		{
			ILog log = LogManager.GetLogger(name);
			return new Log4netLogger(log);
		}

		public override ILogger Create(String name, LoggerLevel level)
		{
			throw new NotSupportedException("Logger levels cannot be set at runtime. Please review your configuration file.");
		}
	}
}
