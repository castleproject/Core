// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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
	using Castle.Core.Logging;

	using NLog;
	using NLog.Config;

	/// <summary>
	/// Implementation of <see cref="ILoggerFactory"/> for NLog.
	/// </summary>
	public class NLogFactory : AbstractLoggerFactory
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NLogFactory"/> class.
		/// </summary>
		public NLogFactory()
			: this("nlog.config")
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NLogFactory"/> class.
		/// </summary>
		/// <param name="configFile">The config file.</param>
		public NLogFactory(string configFile)
		{
			FileInfo file = GetConfigFile(configFile);
			LogManager.Configuration = new XmlLoggingConfiguration(file.FullName);
		}

		/// <summary>
		/// Creates a logger with specified <paramref name="name"/>.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public override ILogger Create(String name)
		{
			Logger log = LogManager.GetLogger(name);
			return new NLogLogger(log, this);
		}

		/// <summary>
		/// Not implemented, NLog logger levels cannot be set at runtime.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="level">The level.</param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException" />
		public override ILogger Create(String name, LoggerLevel level)
		{
			throw new NotImplementedException("Logger levels cannot be set at runtime. Please review your configuration file.");
		}
	}
}
