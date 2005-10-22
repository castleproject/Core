// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.Services.Logging.NLogIntegration
{
	using System;
	using System.IO;
	using Castle.Services.Logging;

	using NLog;
	using NLog.Config;

	/// <summary>
	/// Summary description for NLogFactory.
	/// </summary>
	public class NLogFactory : AbstractLoggerFactory
	{
		public NLogFactory()
		{
		    FileInfo file = this.GetConfigFile("nlog.config");
			LogManager.Configuration = new XmlLoggingConfiguration(file.FullName);
		}

		public override ILogger Create(String name)
		{
			Logger log = LogManager.GetLogger(name);
			return new NLogLogger(log);
		}

		public override ILogger Create(String name, LoggerLevel level)
		{
			throw new NotImplementedException("Logger levels cannot be set at runtime. Please review your configuration file.");
		}
	}
}