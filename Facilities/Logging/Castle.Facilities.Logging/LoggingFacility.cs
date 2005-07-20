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

namespace Castle.Facilities.Logging
{
	using System;
	using System.IO;

	using Castle.Facilities.Logging.log4netIntegration;
	using Castle.Facilities.Logging.NLogIntegration;
	using Castle.MicroKernel.Facilities;
	using Castle.Model;
	using Castle.Model.Configuration;
	using Castle.Services.Logging;

	public enum LoggingFramework
	{
		None,
		log4net,
		NLog
	}

	/// <summary>
	/// 
	/// </summary>
	public class LoggingFacility : AbstractFacility
	{
		private LoggingFramework framework;
		private ILoggerFactory factory;
		private bool intercept = true;
		private FileInfo configFile;

		public LoggingFacility()
		{
			this.framework = LoggingFramework.None;
		}

		protected override void Init()
		{
			GetConfigurationInformation();

			SetupLogManager();

			if (intercept) this.Kernel.AddComponent("logging.intercepter", typeof(LoggingInterceptor));

			////////////////////////
			//For Attributal Logging
			// Kernel.ComponentModelCreated += new ComponentModelDelegate(OnComponentModelCreated);
			// Kernel.ComponentRegistered += new ComponentDataDelegate(OnComponentRegistered);

			// For ILogger Injection

			// This is going to be deffered
			// this.Kernel.Resolver.DependencyResolving += new Castle.MicroKernel.DependancyDelegate(Resolver_DependencyResolving);
		}

		private void GetConfigurationInformation()
		{
			if (this.FacilityConfig != null)
			{
				IConfiguration frameworkConfig = FacilityConfig.Children["framework"];
				String fw = frameworkConfig.Value;
				this.framework = (LoggingFramework) Enum.Parse(typeof(LoggingFramework), fw, true);
				this.intercept = bool.Parse(FacilityConfig.Children["interception"].Value);
				this.configFile = new FileInfo(FacilityConfig.Children["config"].Value);
			}
			else
			{
				this.framework = LoggingFramework.None;
				this.intercept = true;
				this.configFile = null;
			}
		}

		/// <summary>
		/// Setups the log manager.
		/// </summary>
		/// <remarks>
		/// How do I decide which logfactory to install?
		/// </remarks>
		private void SetupLogManager()
		{
			if (this.FacilityConfig == null)
			{
				this.factory = new NullLogFactory();
				this.Kernel.AddComponent("logging.logger", typeof(ILogger), typeof(NullLogger));
			}
			else
			{
				if (this.framework == LoggingFramework.log4net)
				{
					this.factory = new log4netFactory(configFile);
					this.Kernel.AddComponent("logging.logger", typeof(ILogger), typeof(NullLogger));
				}
				else if (this.framework == LoggingFramework.NLog)
				{
					this.factory = new NLogFactory();
					this.Kernel.AddComponent("logging.logger", typeof(ILogger), typeof(NullLogger));
				}
				else
				{
					this.factory = new NullLogFactory();
					this.Kernel.AddComponent("logging.logger", typeof(ILogger), typeof(NullLogger));
				}
			}
		}

		private void Resolver_DependencyResolving(ComponentModel client, DependencyModel model, ref object dependency)
		{
			dependency = factory.Create(client.Implementation);
		}
	}
}