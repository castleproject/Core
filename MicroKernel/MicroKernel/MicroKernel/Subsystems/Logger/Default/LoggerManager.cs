// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.MicroKernel.Subsystems.Logger.Default
{
	using System;
	using System.Configuration;

	using Apache.Avalon.Framework;

	/// <summary>
	/// This is a very simplistic implementation of ILoggerManager
	/// that creates loggers based on a root ILogger implementation.
	/// To choose a implementation for ILogger it reads from the
	/// configuration of the current AppDomain 
	/// </summary>
	public class LoggerManager : AbstractSubsystem, ILoggerManager
	{

		public LoggerManager()
		{
		}

		#region ILoggerManager Members

		public ILogger CreateLogger(String loggerName, String implementationName, AvalonLoggerAttribute loggerAtt)
		{
			String name = null;

			if (loggerAtt != null)
			{
				name = loggerAtt.Name;
			}
			if (name == null || name.Length == 0)
			{
				name = loggerName;
			}
			if (name == null || name.Length == 0)
			{
				name = implementationName;
			}

			return new ConsoleLogger().CreateChildLogger( name );
		}

		#endregion
	}
}
