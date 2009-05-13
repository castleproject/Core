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

namespace NVelocity.Runtime.Log
{
	using System;
	using System.Collections;

	/// <summary>
	/// <p>
	/// This class is responsible for instantiating the correct LoggingSystem
	/// </p>
	/// <p>
	/// The approach is :
	/// </p>
	/// <ul>
	/// <li>
	/// First try to see if the user is passing in a living object
	/// that is a LogSystem, allowing the app to give is living
	/// custom loggers.
	/// </li>
	/// <li>
	/// Next, run through the (possible) list of classes specified
	/// specified as loggers, taking the first one that appears to
	/// work.  This is how we support finding either log4j or
	/// logkit, whichever is in the classpath, as both are
	/// listed as defaults.
	/// </li>
	/// <li>
	/// Finally, we turn to 'faith-based' logging, and hope that
	/// logkit is in the classpath, and try for an AvalonLogSystem
	/// as a final gasp.  After that, there is nothing we can do.
	/// </li>
	/// </ul>
	/// </summary>
	/// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a></author>
	/// <author> <a href="mailto:jon@latchkey.com">Jon S. Stevens</a></author>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a></author>
	public class LogManager
	{
		/// <summary>  Creates a new logging system or returns an existing one
		/// specified by the application.
		/// </summary>
		public static ILogSystem CreateLogSystem(IRuntimeServices runtimeServices)
		{
			ILogSystem logSystem;
			// if a logSystem was set as a configuration value, use that.
			// This is any class the user specifies.
			Object o = runtimeServices.GetProperty(RuntimeConstants.RUNTIME_LOG_LOGSYSTEM);
			logSystem = o as ILogSystem;
			if (logSystem != null)
			{
				logSystem.Init(runtimeServices);

				return logSystem;
			}

			// otherwise, see if a class was specified.  You
			// can put multiple classes, and we use the first one we find.
			//
			// Note that the default value of this property contains both the
			// AvalonLogSystem and the SimpleLog4JLogSystem for convenience -
			// so we use whichever we find.
			IList classes = new ArrayList();
			Object obj = runtimeServices.GetProperty(RuntimeConstants.RUNTIME_LOG_LOGSYSTEM_CLASS);

			// we might have a list, or not - so check
			if (obj is IList)
			{
				classes = (IList) obj;
			}
			else if (obj is String)
			{
				classes.Add(obj);
			}

			// now run through the list, trying each.  It's ok to
			// fail with a class not found, as we do this to also
			// search out a default simple file logger
			foreach(String className in classes)
			{
				if (className != null && className.Length > 0)
				{
					runtimeServices.Info(string.Format("Trying to use logger class {0}", className));

					try
					{
						Type type = Type.GetType(className);
						o = Activator.CreateInstance(type);
						logSystem = o as ILogSystem;
						if (logSystem == null)
						{
							runtimeServices.Error(string.Format("The specified logger class {0} isn't a valid LogSystem", className));
						}
						else
						{
							logSystem.Init(runtimeServices);

							runtimeServices.Info(string.Format("Using logger class {0}", className));

							return logSystem;
						}
					}
					catch(ApplicationException applicationException)
					{
						runtimeServices.Debug(
							string.Format("Couldn't find class {0} or necessary supporting classes in classpath. Exception : {1}", className,
							              applicationException));
					}
				}
			}

			// if the above failed, then we are in deep doo-doo, as the
			// above means that either the user specified a logging class
			// that we can't find, there weren't the necessary
			// dependencies in the classpath for it, or there were no
			// dependencies for the default logger.
			// Since we really don't know,
			// then take a wack at the log4net as a last resort.
			try
			{
				logSystem = new NullLogSystem();
				logSystem.Init(runtimeServices);
			}
			catch(ApplicationException applicationException)
			{
				String error =
					string.Format(
						"PANIC : NVelocity cannot find any of the specified or default logging systems in the classpath, or the classpath doesn't contain the necessary classes to support them. Please consult the documentation regarding logging. Exception : {0}",
						applicationException);

				Console.Out.WriteLine(error);
				Console.Error.WriteLine(error);

				throw;
			}

			runtimeServices.Info("Using log4net as logger of final resort.");

			return logSystem;
		}
	}
}