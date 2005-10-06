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
	using System.Configuration;
	using Castle.Model.Configuration;
	using Castle.Services.Logging;
	using Castle.MicroKernel;
	using Castle.MicroKernel.Facilities;
	using Castle.MicroKernel.SubSystems.Conversion;
	
	/// <summary>
	/// 
	/// </summary>
	public enum LoggerImplementation
	{
		Null,
		Console,
		Diagnostics,
		Web,
		Log4net,
		NLog,
		Custom
	}

	/// <summary>
	/// A facility for logging support.
	/// </summary>
	public class LoggingFacility : AbstractFacility
	{
		private IConversionManager converter;
		private ILoggerFactory customFactory;
		private LoggerImplementation defaultLogImpl = LoggerImplementation.Console;
		private bool allowInterception = false;

		public LoggingFacility()
		{
		}

		protected override void Init()
		{
			converter = Kernel.GetSubSystem( 
				SubSystemConstants.ConversionManagerKey ) as IConversionManager;

			ConfigureFactory();
			EnableKernelLoggerInjection();

			Kernel.ComponentRegistered += new ComponentDataDelegate(Kernel_ComponentRegistered);
		}

		private void EnableKernelLoggerInjection()
		{
			Kernel.AddComponent("fac.logging.logger", typeof(ILogger), typeof(NullLogger));
			// This is going to be deffered
			// this.Kernel.Resolver.DependencyResolving += new Castle.MicroKernel.DependancyDelegate(InjectClassLogger);
		}

        private void ConfigureFactory()
        {
            if(FacilityConfig == null)
            {
                throw new ConfigurationException("The logging facility requires an external configuration");
            }

            IConfiguration defaultNode = FacilityConfig.Children["default"];
                

            String enableInterceptionAtt = defaultNode.Attributes["enableInterception"];
            String typeAtt = defaultNode.Attributes["type"];				
            String customAtt = defaultNode.Attributes["custom"];

            if (enableInterceptionAtt != null)
            {
                allowInterception = (bool) converter.PerformConversion( 
                    enableInterceptionAtt, typeof(bool) );
            }

            if (typeAtt != null)
            {
                defaultLogImpl = (LoggerImplementation) Enum.Parse(
                    typeof(LoggerImplementation), typeAtt, true);
            }

            Type customLoggerFactoryType = null;

            if (customAtt != null)
            {
                customLoggerFactoryType = (Type) converter.PerformConversion( customAtt, typeof(Type) );
            }

            
            customFactory = (ILoggerFactory) Activator.CreateInstance(customLoggerFactoryType);
            
            if(customFactory == null)
            {
                throw new ConfigurationException("{0} does not implement ILoggerFactory");
            }

			

            if (allowInterception)
            {
                Kernel.AddComponent("logging.intercepter", typeof(LoggingInterceptor));
            }
        }

		private void Kernel_ComponentRegistered(String key, IHandler handler)
		{
			LoggerImplementation logImpl = defaultLogImpl;
			Type customLoggerType = null;
			bool enableAutomaticLogging = allowInterception;

			String desiredLoggerAtt = handler.ComponentModel.Configuration.Attributes[ "logger" ];
			String customLoggerAtt = handler.ComponentModel.Configuration.Attributes[ "customlogger" ];
			String enableAutomaticLoggingAtt = handler.ComponentModel.Configuration.Attributes[ "automaticlogging" ];

			if (desiredLoggerAtt != null)
			{
				logImpl = (LoggerImplementation) Enum.Parse(
					typeof(LoggerImplementation), desiredLoggerAtt, true);
			}

			if (customLoggerAtt != null)
			{
				customLoggerType = (Type) converter.PerformConversion(
					customLoggerAtt, typeof(Type));
			}

			if (enableAutomaticLoggingAtt != null)
			{
				enableAutomaticLogging = (bool) converter.PerformConversion(
					enableAutomaticLoggingAtt, typeof(bool));
			}
		}

//		private void InjectClassLogger(ComponentModel client, DependencyModel model, ref object dependency)
//		{
//			if (model.TargetType == typeof(ILogger))
//			{
//				string clientLoggingKey = String.Format("{0}.{1}", client.Implementation.ToString(), model.DependencyKey);
//				if (!Kernel.HasComponent(clientLoggingKey))
//				{
//					ILogger logger = factory.Create(client.Implementation);
//					Kernel.AddComponentInstance(clientLoggingKey, logger);
//				}
//				dependency = (ILogger) Kernel[clientLoggingKey];
//			}
//		}
	}
}