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

	using Castle.Services.Logging;

	using Castle.MicroKernel;
	using Castle.MicroKernel.Facilities;
	using Castle.MicroKernel.SubSystems.Conversion;
	
	/// <summary>
	/// The supported <see cref="ILogger"/> implementations
	/// </summary>
	public enum LoggerImplementation
	{
		Null,
		Console,
		Diagnostics,
		Web,
		NLog,
		Log4net,
		Custom
	}

	/// <summary>
	/// A facility for logging support.
	/// 
	/// TODO: Document its inner working and configuration scheme
	/// </summary>
	public class LoggingFacility : AbstractFacility
	{
		private static readonly String Log4NetLoggerFactoryTypeName = "Castle.Services.Logging.Log4netIntegration.Log4netFactory, Castle.Services.Logging.log4netIntegration";
		private static readonly String NLogLoggerFactoryTypeName = "Castle.Services.Logging.NLogIntegration.NLogFactory, Castle.Services.Logging.NLogIntegration";
		private static readonly String WebLoggerFactoryTypeName = "Castle.Services.Logging.Web.WebLoggerFactory, Castle.Services.Logging.Web";

		private ITypeConverter converter;
		private ILoggerFactory factory;

		public LoggingFacility()
		{
		}

		protected override void Init()
		{
			SetUpTypeConverter();

			ReadConfigurationAndCreateLoggerFactory();

			RegisterDefaultILogger();

			RegisterSubResolver();
		}

		private void RegisterDefaultILogger()
		{
			Kernel.AddComponentInstance( "ilogger.default", typeof(ILogger), factory.Create("Default") );
		}

		private void RegisterLoggerFactory()
		{
			Kernel.AddComponentInstance( "iloggerfactory", typeof(ILoggerFactory), factory );
		}

		private void RegisterSubResolver()
		{
			Kernel.Resolver.AddSubResolver( new LoggerResolver(factory) );
		}

		/// <summary>
		/// 
		/// </summary>
		private void ReadConfigurationAndCreateLoggerFactory()
        {
			LoggerImplementation logApi = LoggerImplementation.Console;

            String typeAtt = FacilityConfig.Attributes["loggingApi"];				
            String customAtt = FacilityConfig.Attributes["customLoggerFactory"];

            if (typeAtt != null)
            {
                logApi = (LoggerImplementation) 
					converter.PerformConversion( typeAtt, typeof(LoggerImplementation) );
            }

			CreateProperLoggerFactory(logApi, customAtt);

			RegisterLoggerFactory();
        }

		private void CreateProperLoggerFactory(LoggerImplementation logApi, String customType)
		{
			Type loggerFactoryType = null;

			if(logApi == LoggerImplementation.Console)
			{
				loggerFactoryType = typeof(ConsoleFactory);
			}
			else if(logApi == LoggerImplementation.Log4net)
			{
				loggerFactoryType = (Type) 
					converter.PerformConversion(Log4NetLoggerFactoryTypeName, typeof(Type));
			}
			else if(logApi == LoggerImplementation.NLog)
			{
				loggerFactoryType = (Type) 
					converter.PerformConversion(NLogLoggerFactoryTypeName, typeof(Type));
			}
			else if(logApi == LoggerImplementation.Diagnostics)
			{
				
			}
			else if(logApi == LoggerImplementation.Null)
			{
				
			}
			else if(logApi == LoggerImplementation.Web)
			{
				loggerFactoryType = (Type) 
					converter.PerformConversion(WebLoggerFactoryTypeName, typeof(Type));
			}
			else if(logApi == LoggerImplementation.Custom)
			{
				if (customType == null)
				{
					throw new ConfigurationException("If you specify loggingApi='custom' " + 
						"then you must use the attribute customLoggerFactory to inform the " + 
						"type name of the custom logger factory");
				}

				loggerFactoryType = (Type) 
					converter.PerformConversion( customType, typeof(Type) );

				if (!typeof(ILoggerFactory).IsAssignableFrom(loggerFactoryType))
				{
					throw new FacilityException("The specified type '" + customType + 
						"' does not implement ILoggerFactory");
				}
			}

			factory = (ILoggerFactory) Activator.CreateInstance(loggerFactoryType);
		}

		private void SetUpTypeConverter()
		{
			converter = Kernel.GetSubSystem( 
				SubSystemConstants.ConversionManagerKey ) as IConversionManager;
		}
	}

}