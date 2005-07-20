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
	using Castle.Model.Configuration;
	using Castle.Services.Logging;

	public enum LoggingFramework
	{
		None,
		log4net,
		NLog
	}

	/// <summary>
	/// A facility for the Castle framework that supports logging.
	/// </summary>
	public class LoggingFacility : AbstractFacility
	{
		private ILoggerFactory factory;

		public LoggingFacility()
		{
		}

		protected override void Init()
		{
		    ConfigureFactory();

		    EnableKernelLoggerInjection();
		}

	    private void EnableKernelLoggerInjection()
	    {
	        this.Kernel.AddComponent("fac.logging.logger", typeof(ILogger), typeof(NullLogger));
            // This is going to be deffered
	        // this.Kernel.Resolver.DependencyResolving += new Castle.MicroKernel.DependancyDelegate(InjectClassLogger);
	    }

	    private void ConfigureFactory()
	    {
            bool intercept = true;
	
            if(this.FacilityConfig != null)
	        {
                IConfiguration frameworkConfig = FacilityConfig.Children["framework"];
                LoggingFramework framework = (LoggingFramework) Enum.Parse(typeof(LoggingFramework), frameworkConfig.Value, true);
                FileInfo configFile = new FileInfo(FacilityConfig.Children["config"].Value);
                intercept = bool.Parse(FacilityConfig.Children["interception"].Value);

                if(framework == LoggingFramework.log4net)
                {
                    this.factory = new log4netFactory(configFile);
                }
                else if(framework == LoggingFramework.NLog)
                {
                    this.factory = new NLogFactory(configFile);
                }
                else
                {
                    this.factory = new NullLogFactory();
                }
            }
            else
	        {
                this.factory = new NullLogFactory();
            }
            if (intercept)
            {
                this.Kernel.AddComponent("logging.intercepter", typeof(LoggingInterceptor));
            }
        }

        private void InjectClassLogger(Castle.Model.ComponentModel client, Castle.Model.DependencyModel model, ref object dependency)
        {
            if(model.TargetType == typeof(ILogger)) 
            {
                string clientLoggingKey = String.Format("{0}.{1}", client.Implementation.ToString(), model.DependencyKey);
                if (!Kernel.HasComponent(clientLoggingKey)) 
                {
                    ILogger logger = factory.Create(client.Implementation);
                    Kernel.AddComponentInstance(clientLoggingKey, logger);
                }
                dependency = (ILogger)Kernel[clientLoggingKey];
            }
        }
    }
}
