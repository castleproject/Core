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
		Custom,
		Null,
		Console,
		Diagnostics,
		Web,
		NLog,
		Log4net,
		ExtendedNLog,
		ExtendedLog4net
	}

	/// <summary>
	/// A facility for logging support.
	/// </summary>
	/// <remarks>TODO: Document its inner working and configuration scheme</remarks>
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

		private static readonly String ExtendedLog4NetLoggerFactoryTypeName =
			"Castle.Services.Logging.Log4netIntegration.ExtendedLog4netFactory," +
			"Castle.Services.Logging.Log4netIntegration,Version=1.0.0.0, Culture=neutral," +
			"PublicKeyToken=407dd0808d44fbdc";

		private static readonly String ExtendedNLogLoggerFactoryTypeName =
			"Castle.Services.Logging.NLogIntegration.ExtendedNLogFactory," +
			"Castle.Services.Logging.NLogIntegration,Version=1.0.0.0, Culture=neutral," +
			"PublicKeyToken=407dd0808d44fbdc";

		private ITypeConverter converter;
		private ILoggerFactory factory;
		private LoggerImplementation logApi;

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
			if (logApi == LoggerImplementation.ExtendedNLog || logApi == LoggerImplementation.ExtendedLog4net)
			{
				Kernel.AddComponentInstance("ilogger.default", typeof(IExtendedLogger), factory.Create("Default"));
			}
			else
			{
				Kernel.AddComponentInstance("ilogger.default", typeof(ILogger), factory.Create("Default"));
			}
		}

		private void RegisterLoggerFactory()
		{
			if (logApi == LoggerImplementation.ExtendedNLog || logApi == LoggerImplementation.ExtendedLog4net)
			{
				Kernel.AddComponentInstance("iloggerfactory", typeof(IExtendedLoggerFactory), factory);
			}
			else
			{
				Kernel.AddComponentInstance("iloggerfactory", typeof(ILoggerFactory), factory);
			}
		}

		private void RegisterSubResolver()
		{
			Kernel.Resolver.AddSubResolver(new LoggerResolver(factory));
		}

		private void ReadConfigurationAndCreateLoggerFactory()
		{
			logApi = LoggerImplementation.Console;

			String typeAtt = FacilityConfig.Attributes["loggingApi"];
			String customAtt = FacilityConfig.Attributes["customLoggerFactory"];
			String configFileAtt = FacilityConfig.Attributes["configFile"];

			if (typeAtt != null)
			{
				logApi = (LoggerImplementation)
				         converter.PerformConversion(typeAtt, typeof(LoggerImplementation));
			}

			CreateProperLoggerFactory(customAtt, configFileAtt);

			RegisterLoggerFactory();
		}

		private void CreateProperLoggerFactory(string customType, string configFile)
		{
			Type loggerFactoryType;

			switch(logApi)
			{
				case LoggerImplementation.Custom:
					if (customType == null)
					{
						String message = "If you specify loggingApi='custom' " +
														 "then you must use the attribute customLoggerFactory to inform the " +
														 "type name of the custom logger factory";
#if DOTNET2
						throw new ConfigurationErrorsException(message);
#else
						throw new ConfigurationException(message);
#endif
					}

					loggerFactoryType = (Type)
															converter.PerformConversion(customType, typeof(Type));

					if (!typeof(ILoggerFactory).IsAssignableFrom(loggerFactoryType) && !typeof(IExtendedLoggerFactory).IsAssignableFrom(loggerFactoryType))
					{
						throw new FacilityException("The specified type '" + customType +
																				"' does not implement either ILoggerFactory or IExtendedLoggerFactory.");
					}
					break;
				case LoggerImplementation.Null:
					loggerFactoryType = typeof(NullLogFactory);
					break;
				case LoggerImplementation.Console:
					loggerFactoryType = typeof(ConsoleFactory);
					break;
				case LoggerImplementation.Diagnostics:
					loggerFactoryType = typeof(DiagnosticsLoggerFactory);
					break;
				case LoggerImplementation.Web:
					loggerFactoryType = typeof(WebLoggerFactory);
					break;
				case LoggerImplementation.Log4net:
					loggerFactoryType = (Type)converter.PerformConversion(Log4NetLoggerFactoryTypeName, typeof(Type));
					break;
				case LoggerImplementation.NLog:
					loggerFactoryType = (Type)converter.PerformConversion(NLogLoggerFactoryTypeName, typeof(Type));
					break;
				case LoggerImplementation.ExtendedLog4net:
					loggerFactoryType = (Type)converter.PerformConversion(ExtendedLog4NetLoggerFactoryTypeName, typeof(Type));
					break;
				case LoggerImplementation.ExtendedNLog:
					loggerFactoryType = (Type)converter.PerformConversion(ExtendedNLogLoggerFactoryTypeName, typeof(Type));
					break;
				default:
					{
						String message = "An invalid loggingApi was specified: " + logApi;
#if DOTNET2
						throw new ConfigurationErrorsException(message);
#else
						throw new ConfigurationException(message);
#endif
					}
			}

			if (loggerFactoryType == null)
			{
				throw new FacilityException("LoggingFacility was unable to find an implementation of ILoggerFactory or IExtendedLoggerFactory.");
			}

			object[] args = null;

			if (configFile != null) args = new object[] {configFile};

			if (logApi == LoggerImplementation.ExtendedNLog || logApi == LoggerImplementation.ExtendedLog4net)
			{
				factory = (IExtendedLoggerFactory) Activator.CreateInstance(loggerFactoryType, args);
			}
			else
			{
				factory = (ILoggerFactory) Activator.CreateInstance(loggerFactoryType, args);
			}
		}

		private void SetUpTypeConverter()
		{
			converter = Kernel.GetSubSystem(
			            	SubSystemConstants.ConversionManagerKey) as IConversionManager;
		}
	}
}