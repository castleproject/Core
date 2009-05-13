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

namespace Castle.Facilities.Logging
{
	using System;
	using System.Configuration;
	using System.Reflection;
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
		ExtendedLog4net,
        Trace
	}

	/// <summary>
	/// A facility for logging support.
	/// </summary>
	/// <remarks>TODO: Document its inner working and configuration scheme</remarks>
	public class LoggingFacility : AbstractFacility
	{
		private static readonly String Log4NetLoggerFactoryTypeName =
			"Castle.Services.Logging.Log4netIntegration.Log4netFactory," +
			"Castle.Services.Logging.Log4netIntegration,Version=1.0.3.0, Culture=neutral," +
			"PublicKeyToken=407dd0808d44fbdc";

		private static readonly String NLogLoggerFactoryTypeName =
			"Castle.Services.Logging.NLogIntegration.NLogFactory," +
			"Castle.Services.Logging.NLogIntegration,Version=1.0.3.0, Culture=neutral," +
			"PublicKeyToken=407dd0808d44fbdc";

		private static readonly String ExtendedLog4NetLoggerFactoryTypeName =
			"Castle.Services.Logging.Log4netIntegration.ExtendedLog4netFactory," +
			"Castle.Services.Logging.Log4netIntegration,Version=1.0.3.0, Culture=neutral," +
			"PublicKeyToken=407dd0808d44fbdc";

		private static readonly String ExtendedNLogLoggerFactoryTypeName =
			"Castle.Services.Logging.NLogIntegration.ExtendedNLogFactory," +
			"Castle.Services.Logging.NLogIntegration,Version=1.0.3.0, Culture=neutral," +
			"PublicKeyToken=407dd0808d44fbdc";

		private ITypeConverter converter;
		private ILoggerFactory factory;
		private LoggerImplementation logApi;

		//Configuration
		private LoggerImplementation? loggingApiConfig = null;
		private string customLoggerFactoryConfig = null;
		private string configFileConfig = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="LoggingFacility"/> class.
		/// </summary>
		public LoggingFacility()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LoggingFacility"/> class.
		/// </summary>
		/// <param name="loggingApi">
		/// The LoggerImplementation that should be used
		/// </param>
		public LoggingFacility(LoggerImplementation loggingApi) : this(loggingApi, null)
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LoggingFacility"/> class.
		/// </summary>
		/// <param name="loggingApi">
		/// The LoggerImplementation that should be used
		/// </param>
		/// <param name="configFile">
		/// The configuration file that should be used by the chosen LoggerImplementation
		/// </param>
		public LoggingFacility(LoggerImplementation loggingApi, string configFile) : this(loggingApi, null, configFile)
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LoggingFacility"/> class using a custom LoggerImplementation
		/// </summary>
		/// <param name="configFile">
		/// The configuration file that should be used by the chosen LoggerImplementation
		/// </param>
		/// <param name="customLoggerFactory">
		/// The type name of the type of the custom logger factory.
		/// </param>
		public LoggingFacility(string customLoggerFactory, string configFile) : this(LoggerImplementation.Custom, customLoggerFactory, configFile)
		{
		
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LoggingFacility"/> class.
		/// </summary>
		/// <param name="loggingApi">
		/// The LoggerImplementation that should be used
		/// </param>
		/// <param name="configFile">
		/// The configuration file that should be used by the chosen LoggerImplementation
		/// </param>
		/// <param name="customLoggerFactory">
		/// The type name of the type of the custom logger factory. (only used when loggingApi is set to LoggerImplementation.Custom)
		/// </param>
		public LoggingFacility(LoggerImplementation loggingApi, string customLoggerFactory, string configFile)
		{
			this.loggingApiConfig = loggingApi;
			this.customLoggerFactoryConfig = customLoggerFactory;
			this.configFileConfig = configFile;
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
				ILogger defaultLogger = factory.Create("Default");
				Kernel.AddComponentInstance("ilogger.default", typeof(IExtendedLogger), defaultLogger);
				Kernel.AddComponentInstance("ilogger.default.base", typeof(ILogger), defaultLogger);
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
				Kernel.AddComponentInstance("iloggerfactory.base", typeof(ILoggerFactory), factory);
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


			String typeAtt = (FacilityConfig  != null) ? FacilityConfig.Attributes["loggingApi"] : null;
			String customAtt = (FacilityConfig  != null)  ? FacilityConfig.Attributes["customLoggerFactory"] : null;
			String configFileAtt = (FacilityConfig  != null) ? FacilityConfig.Attributes["configFile"] : null;

			if (typeAtt != null)
			{
				logApi = (LoggerImplementation)
						 converter.PerformConversion(typeAtt, typeof(LoggerImplementation));
			}
			else if (loggingApiConfig.HasValue)
			{
				logApi = loggingApiConfig.Value;
			}

			if (customAtt == null)
			{
				customAtt = customLoggerFactoryConfig;
			}

			if (configFileAtt == null)
			{
				configFileAtt = configFileConfig;
			}

			CreateProperLoggerFactory(customAtt, configFileAtt);

			RegisterLoggerFactory();
		}

		private void CreateProperLoggerFactory(string customType, string configFile)
		{
			Type loggerFactoryType = GetLoggingFactoryType(customType);

			if (loggerFactoryType == null)
			{
				throw new FacilityException("LoggingFacility was unable to find an implementation of ILoggerFactory or IExtendedLoggerFactory.");
			}

			object[] args = GetLoggingFactoryArguments(configFile, loggerFactoryType);

			if (logApi == LoggerImplementation.ExtendedNLog || logApi == LoggerImplementation.ExtendedLog4net)
			{
				factory = (IExtendedLoggerFactory) Activator.CreateInstance(loggerFactoryType, args);
			}
			else
			{
				factory = (ILoggerFactory) Activator.CreateInstance(loggerFactoryType, args);
			}
		}

		private object[] GetLoggingFactoryArguments(string configFile, Type loggerFactoryType)
		{
			const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;

			object[] args = null;
			ConstructorInfo ctor = null;

			if (configFile != null && configFile.Length > 0)
			{
				ctor = loggerFactoryType.GetConstructor(flags, null, new Type[] { typeof(string) }, null);
			}

			if (ctor != null)
			{
				args = new object[] { configFile };
			}
			else
			{
				ctor = loggerFactoryType.GetConstructor(flags, null, Type.EmptyTypes, null);

				if (ctor == null)
				{
					throw new FacilityException("No support constructor found for logging type " + logApi);
				}
			}
			return args;
		}

		private Type GetLoggingFactoryType(string customType)
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

						throw new ConfigurationErrorsException(message);
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
                case LoggerImplementation.Trace:
			        loggerFactoryType = typeof (TraceLoggerFactory);
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

						throw new ConfigurationErrorsException(message);
					}
			}
			return loggerFactoryType;
		}

		private void SetUpTypeConverter()
		{
			converter = Kernel.GetSubSystem(
							SubSystemConstants.ConversionManagerKey) as IConversionManager;
		}
	}
}
