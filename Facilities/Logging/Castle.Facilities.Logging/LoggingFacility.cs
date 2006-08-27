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

namespace Castle.Facilities.Logging
{
	using System;
	using System.Configuration;

	using Castle.Core.Logging;
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
	/// <para>
	/// TODO: Document its inner working and configuration scheme
	/// </para>
	/// </summary>
	public class LoggingFacility : AbstractFacility
	{
		private static readonly String Log4NetLoggerFactoryTypeName =
			"Castle.Services.Logging.Log4netIntegration.Log4netFactory," +
				"Castle.Services.Logging.Log4netIntegration,Version=1.0.0.0, Culture=neutral," +
				"PublicKeyToken=407dd0808d44fbdc";

		private static readonly String NLogLoggerFactoryTypeName =
			"Castle.Services.Logging.NLogIntegration.NLogFactory," +
				"Castle.Services.Logging.NLogIntegration,Version=1.0.0.0, Culture=neutral," +
				"PublicKeyToken=407dd0808d44fbdc";

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
			Kernel.AddComponentInstance("ilogger.default", typeof(ILogger), factory.Create("Default"));
		}

		private void RegisterLoggerFactory()
		{
			Kernel.AddComponentInstance("iloggerfactory", typeof(ILoggerFactory), factory);
		}

		private void RegisterSubResolver()
		{
			Kernel.Resolver.AddSubResolver(new LoggerResolver(factory));
		}

		/// <summary>
		/// 
		/// </summary>
		private void ReadConfigurationAndCreateLoggerFactory()
		{
			LoggerImplementation logApi = LoggerImplementation.Console;

			String typeAtt = FacilityConfig.Attributes["loggingApi"];
			String customAtt = FacilityConfig.Attributes["customLoggerFactory"];
			String configFileAtt = FacilityConfig.Attributes["configFile"];

			if (typeAtt != null)
			{
				logApi = (LoggerImplementation)
					converter.PerformConversion(typeAtt, typeof(LoggerImplementation));
			}

			CreateProperLoggerFactory(logApi, customAtt, configFileAtt);

			RegisterLoggerFactory();
		}

		private void CreateProperLoggerFactory(LoggerImplementation logApi, String customType, String configFile)
		{
			Type loggerFactoryType = null;

            if(logApi == LoggerImplementation.Null)
            {
                loggerFactoryType = typeof(NullLogFactory);
            }
			else if (logApi == LoggerImplementation.Console)
			{
				loggerFactoryType = typeof(ConsoleFactory);
			}
			else if (logApi == LoggerImplementation.Log4net)
			{
				loggerFactoryType = (Type)
					converter.PerformConversion(Log4NetLoggerFactoryTypeName, typeof(Type));

			}
			else if (logApi == LoggerImplementation.NLog)
			{
				loggerFactoryType = (Type)
					converter.PerformConversion(NLogLoggerFactoryTypeName, typeof(Type));

			}
			else if (logApi == LoggerImplementation.Diagnostics)
			{
			}
			else if (logApi == LoggerImplementation.Null)
			{
			}
			else if (logApi == LoggerImplementation.Web)
			{
				loggerFactoryType = typeof(WebLoggerFactory);
			}
			else if (logApi == LoggerImplementation.Custom)
			{
				if (customType == null)
				{
					throw new ConfigurationException("If you specify loggingApi='custom' " +
						"then you must use the attribute customLoggerFactory to inform the " +
						"type name of the custom logger factory");
				}

				loggerFactoryType = (Type)
					converter.PerformConversion(customType, typeof(Type));

				if (!typeof(ILoggerFactory).IsAssignableFrom(loggerFactoryType))
				{
					throw new FacilityException("The specified type '" + customType +
						"' does not implement ILoggerFactory");
				}
			}

			object[] args = null;

			if (configFile != null) args = new object[] {configFile};

			factory = (ILoggerFactory) Activator.CreateInstance(loggerFactoryType, args);
		}

		private void SetUpTypeConverter()
		{
			converter = Kernel.GetSubSystem(
				SubSystemConstants.ConversionManagerKey) as IConversionManager;
		}
	}

}