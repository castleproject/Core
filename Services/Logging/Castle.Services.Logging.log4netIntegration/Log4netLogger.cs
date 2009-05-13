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

using Logger = Castle.Core.Logging.ILogger;

namespace Castle.Services.Logging.Log4netIntegration
{
	using System;
	using log4net;
	using log4net.Core;

	[Serializable]
	public class Log4netLogger : MarshalByRefObject, Logger
	{
		private static Type declaringType = typeof(Log4netLogger);

		private ILogger logger;
		private Log4netFactory factory;

		internal Log4netLogger()
		{
		}

		internal Log4netLogger(ILog log, Log4netFactory factory) : this(log.Logger, factory)
		{
		}

		public Log4netLogger(ILogger logger, Log4netFactory factory)
		{
			Logger = logger;
			Factory = factory;
		}

		public virtual Logger CreateChildLogger(String name)
		{
			return Factory.Create(Logger.Name + "." + name);
		}

		protected internal ILogger Logger
		{
			get { return logger; }
			set { logger = value; }
		}

		protected internal Log4netFactory Factory
		{
			get { return factory; }
			set { factory = value; }
		}

		public override string ToString()
		{
			return Logger.ToString();
		}

		#region Debug

		public void Debug(String message)
		{
			if (IsDebugEnabled)
			{
				Logger.Log(declaringType, Level.Debug, message, null);
			}
		}

		public void Debug(String message, Exception exception)
		{
			if (IsDebugEnabled)
			{
				Logger.Log(declaringType, Level.Debug, message, exception);
			}
		}

		public void DebugFormat(String format, params Object[] args)
		{
			if (IsDebugEnabled)
			{
				Logger.Log(declaringType, Level.Debug, String.Format(format, args), null);
			}
		}

		public void DebugFormat(Exception exception, String format, params Object[] args)
		{
			if (IsDebugEnabled)
			{
				Logger.Log(declaringType, Level.Debug, String.Format(format, args), exception);
			}
		}

		public void DebugFormat(IFormatProvider formatProvider, String format, params Object[] args)
		{
			if (IsDebugEnabled)
			{
				Logger.Log(declaringType, Level.Debug, String.Format(formatProvider, format, args), null);
			}
		}

		public void DebugFormat(Exception exception, IFormatProvider formatProvider, String format, params Object[] args)
		{
			if (IsDebugEnabled)
			{
				Logger.Log(declaringType, Level.Debug, String.Format(formatProvider, format, args), exception);
			}
		}

		[Obsolete("Use DebugFormat instead")]
		public void Debug(String format, params object[] args)
		{
			if (IsDebugEnabled)
			{
				Logger.Log(declaringType, Level.Debug, String.Format(format, args), null);
			}
		}

		#endregion

		#region Info

		public void Info(String message)
		{
			if (IsInfoEnabled)
			{
				Logger.Log(declaringType, Level.Info, message, null);
			}
		}

		public void Info(String message, Exception exception)
		{
			if (IsInfoEnabled)
			{
				Logger.Log(declaringType, Level.Info, message, exception);
			}
		}

		public void InfoFormat(String format, params Object[] args)
		{
			if (IsInfoEnabled)
			{
				Logger.Log(declaringType, Level.Info, String.Format(format, args), null);
			}
		}

		public void InfoFormat(Exception exception, String format, params Object[] args)
		{
			if (IsInfoEnabled)
			{
				Logger.Log(declaringType, Level.Info, String.Format(format, args), exception);
			}
		}

		public void InfoFormat(IFormatProvider formatProvider, String format, params Object[] args)
		{
			if (IsInfoEnabled)
			{
				Logger.Log(declaringType, Level.Info, String.Format(formatProvider, format, args), null);
			}
		}

		public void InfoFormat(Exception exception, IFormatProvider formatProvider, String format, params Object[] args)
		{
			if (IsInfoEnabled)
			{
				Logger.Log(declaringType, Level.Info, String.Format(formatProvider, format, args), exception);
			}
		}

		[Obsolete("Use InfoFormat instead")]
		public void Info(String format, params object[] args)
		{
			if (IsInfoEnabled)
			{
				Logger.Log(declaringType, Level.Info, String.Format(format, args), null);
			}
		}

		#endregion

		#region Warn

		public void Warn(String message)
		{
			if (IsWarnEnabled)
			{
				Logger.Log(declaringType, Level.Warn, message, null);
			}
		}

		public void Warn(String message, Exception exception)
		{
			if (IsWarnEnabled)
			{
				Logger.Log(declaringType, Level.Warn, message, exception);
			}
		}

		public void WarnFormat(String format, params Object[] args)
		{
			if (IsWarnEnabled)
			{
				Logger.Log(declaringType, Level.Warn, String.Format(format, args), null);
			}
		}

		public void WarnFormat(Exception exception, String format, params Object[] args)
		{
			if (IsWarnEnabled)
			{
				Logger.Log(declaringType, Level.Warn, String.Format(format, args), exception);
			}
		}

		public void WarnFormat(IFormatProvider formatProvider, String format, params Object[] args)
		{
			if (IsWarnEnabled)
			{
				Logger.Log(declaringType, Level.Warn, String.Format(formatProvider, format, args), null);
			}
		}

		public void WarnFormat(Exception exception, IFormatProvider formatProvider, String format, params Object[] args)
		{
			if (IsWarnEnabled)
			{
				Logger.Log(declaringType, Level.Warn, String.Format(formatProvider, format, args), exception);
			}
		}

		[Obsolete("Use WarnFormat instead")]
		public void Warn(String format, params object[] args)
		{
			if (IsWarnEnabled)
			{
				Logger.Log(declaringType, Level.Warn, String.Format(format, args), null);
			}
		}

		#endregion

		#region Error

		public void Error(String message)
		{
			if (IsErrorEnabled)
			{
				Logger.Log(declaringType, Level.Error, message, null);
			}
		}

		public void Error(String message, Exception exception)
		{
			if (IsErrorEnabled)
			{
				Logger.Log(declaringType, Level.Error, message, exception);
			}
		}

		public void ErrorFormat(String format, params Object[] args)
		{
			if (IsErrorEnabled)
			{
				Logger.Log(declaringType, Level.Error, String.Format(format, args), null);
			}
		}

		public void ErrorFormat(Exception exception, String format, params Object[] args)
		{
			if (IsErrorEnabled)
			{
				Logger.Log(declaringType, Level.Error, String.Format(format, args), exception);
			}
		}

		public void ErrorFormat(IFormatProvider formatProvider, String format, params Object[] args)
		{
			if (IsErrorEnabled)
			{
				Logger.Log(declaringType, Level.Error, String.Format(formatProvider, format, args), null);
			}
		}

		public void ErrorFormat(Exception exception, IFormatProvider formatProvider, String format, params Object[] args)
		{
			if (IsErrorEnabled)
			{
				Logger.Log(declaringType, Level.Error, String.Format(formatProvider, format, args), exception);
			}
		}

		[Obsolete("Use ErrorFormat instead")]
		public void Error(String format, params object[] args)
		{
			if (IsErrorEnabled)
			{
				Logger.Log(declaringType, Level.Error, String.Format(format, args), null);
			}
		}

		#endregion

		#region Fatal

		public void Fatal(String message)
		{
			if (IsFatalEnabled)
			{
				Logger.Log(declaringType, Level.Fatal, message, null);
			}
		}

		public void Fatal(String message, Exception exception)
		{
			if (IsFatalEnabled)
			{
				Logger.Log(declaringType, Level.Fatal, message, exception);
			}
		}

		public void FatalFormat(String format, params Object[] args)
		{
			if (IsFatalEnabled)
			{
				Logger.Log(declaringType, Level.Fatal, String.Format(format, args), null);
			}
		}

		public void FatalFormat(Exception exception, String format, params Object[] args)
		{
			if (IsFatalEnabled)
			{
				Logger.Log(declaringType, Level.Fatal, String.Format(format, args), exception);
			}
		}

		public void FatalFormat(IFormatProvider formatProvider, String format, params Object[] args)
		{
			if (IsFatalEnabled)
			{
				Logger.Log(declaringType, Level.Fatal, String.Format(formatProvider, format, args), null);
			}
		}

		public void FatalFormat(Exception exception, IFormatProvider formatProvider, String format, params Object[] args)
		{
			if (IsFatalEnabled)
			{
				Logger.Log(declaringType, Level.Fatal, String.Format(formatProvider, format, args), exception);
			}
		}

		[Obsolete("Use FatalFormat instead")]
		public void Fatal(String format, params object[] args)
		{
			if (IsFatalEnabled)
			{
				Logger.Log(declaringType, Level.Fatal, String.Format(format, args), null);
			}
		}

		#endregion

		#region FatalError (obsolete)

		[Obsolete("Use FatalFormat instead")]
		public void FatalError(String format, params object[] args)
		{
			if (IsFatalErrorEnabled)
			{
				Logger.Log(declaringType, Level.Fatal, String.Format(format, args), null);
			}
		}

		[Obsolete("Use Fatal instead")]
		public void FatalError(String message, Exception exception)
		{
			if (IsFatalErrorEnabled)
			{
				Logger.Log(declaringType, Level.Fatal, message, exception);
			}
		}

		[Obsolete("Use Fatal instead")]
		public void FatalError(String message)
		{
			if (IsFatalErrorEnabled)
			{
				Logger.Log(declaringType, Level.Fatal, message, null);
			}
		}

		#endregion

		#region Is (...) Enabled

		public bool IsErrorEnabled
		{
			get { return Logger.IsEnabledFor(Level.Error); }
		}

		public bool IsWarnEnabled
		{
			get { return Logger.IsEnabledFor(Level.Warn); }
		}

		public bool IsDebugEnabled
		{
			get { return Logger.IsEnabledFor(Level.Debug); }
		}

		public bool IsFatalEnabled
		{
			get { return Logger.IsEnabledFor(Level.Fatal); }
		}

		[Obsolete("Use IsFatalEnabled instead")]
		public bool IsFatalErrorEnabled
		{
			get { return Logger.IsEnabledFor(Level.Fatal); }
		}

		public bool IsInfoEnabled
		{
			get { return Logger.IsEnabledFor(Level.Info); }
		}

		#endregion
	}
}
