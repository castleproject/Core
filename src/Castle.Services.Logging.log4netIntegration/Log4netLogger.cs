// Copyright 2004-2017 Castle Project - http://www.castleproject.org/
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

namespace Castle.Services.Logging.Log4netIntegration
{
	using System;
	using System.Globalization;

	using log4net;
	using log4net.Core;
	using log4net.Util;

#if FEATURE_SERIALIZATION
	[Serializable]
#endif
	public class Log4netLogger : Castle.Core.Logging.ILogger
	{
		private static readonly Type declaringType = typeof(Log4netLogger);

		public Log4netLogger(ILogger logger, Log4netFactory factory)
		{
			Logger = logger;
			Factory = factory;
		}

		internal Log4netLogger()
		{
		}

		internal Log4netLogger(ILog log, Log4netFactory factory) : this(log.Logger, factory)
		{
		}

		public bool IsTraceEnabled
		{
			get { return Logger.IsEnabledFor(Level.Trace); }
		}

		public bool IsDebugEnabled
		{
			get { return Logger.IsEnabledFor(Level.Debug); }
		}

		public bool IsErrorEnabled
		{
			get { return Logger.IsEnabledFor(Level.Error); }
		}

		public bool IsFatalEnabled
		{
			get { return Logger.IsEnabledFor(Level.Fatal); }
		}

		public bool IsInfoEnabled
		{
			get { return Logger.IsEnabledFor(Level.Info); }
		}

		public bool IsWarnEnabled
		{
			get { return Logger.IsEnabledFor(Level.Warn); }
		}

		protected internal Log4netFactory Factory { get; set; }

		protected internal ILogger Logger { get; set; }

		public override string ToString()
		{
			return Logger.ToString();
		}

		public virtual Castle.Core.Logging.ILogger CreateChildLogger(string name)
		{
			return Factory.Create(Logger.Name + "." + name);
		}

		public void Trace(string message)
		{
			if (IsTraceEnabled)
			{
				Logger.Log(declaringType, Level.Trace, message, null);
			}
		}

		public void Trace(Func<string> messageFactory)
		{
			if (IsTraceEnabled)
			{
				Logger.Log(declaringType, Level.Trace, messageFactory.Invoke(), null);
			}
		}

		public void Trace(string message, Exception exception)
		{
			if (IsTraceEnabled)
			{
				Logger.Log(declaringType, Level.Trace, message, exception);
			}
		}

		public void TraceFormat(string format, params object[] args)
		{
			if (IsTraceEnabled)
			{
				Logger.Log(declaringType, Level.Trace, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
			}
		}

		public void TraceFormat(Exception exception, string format, params object[] args)
		{
			if (IsTraceEnabled)
			{
				Logger.Log(declaringType, Level.Trace, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), exception);
			}
		}

		public void TraceFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if (IsTraceEnabled)
			{
				Logger.Log(declaringType, Level.Trace, new SystemStringFormat(formatProvider, format, args), null);
			}
		}

		public void TraceFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			if (IsTraceEnabled)
			{
				Logger.Log(declaringType, Level.Trace, new SystemStringFormat(formatProvider, format, args), exception);
			}
		}

		public void Debug(string message)
		{
			if (IsDebugEnabled)
			{
				Logger.Log(declaringType, Level.Debug, message, null);
			}
		}

		public void Debug(Func<string> messageFactory)
		{
			if (IsDebugEnabled)
			{
				Logger.Log(declaringType, Level.Debug, messageFactory.Invoke(), null);
			}
		}

		public void Debug(string message, Exception exception)
		{
			if (IsDebugEnabled)
			{
				Logger.Log(declaringType, Level.Debug, message, exception);
			}
		}

		public void DebugFormat(string format, params object[] args)
		{
			if (IsDebugEnabled)
			{
				Logger.Log(declaringType, Level.Debug, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
			}
		}

		public void DebugFormat(Exception exception, string format, params object[] args)
		{
			if (IsDebugEnabled)
			{
				Logger.Log(declaringType, Level.Debug, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), exception);
			}
		}

		public void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if (IsDebugEnabled)
			{
				Logger.Log(declaringType, Level.Debug, new SystemStringFormat(formatProvider, format, args), null);
			}
		}

		public void DebugFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			if (IsDebugEnabled)
			{
				Logger.Log(declaringType, Level.Debug, new SystemStringFormat(formatProvider, format, args), exception);
			}
		}

		public void Error(string message)
		{
			if (IsErrorEnabled)
			{
				Logger.Log(declaringType, Level.Error, message, null);
			}
		}

		public void Error(Func<string> messageFactory)
		{
			if (IsErrorEnabled)
			{
				Logger.Log(declaringType, Level.Error, messageFactory.Invoke(), null);
			}
		}

		public void Error(string message, Exception exception)
		{
			if (IsErrorEnabled)
			{
				Logger.Log(declaringType, Level.Error, message, exception);
			}
		}

		public void ErrorFormat(string format, params object[] args)
		{
			if (IsErrorEnabled)
			{
				Logger.Log(declaringType, Level.Error, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
			}
		}

		public void ErrorFormat(Exception exception, string format, params object[] args)
		{
			if (IsErrorEnabled)
			{
				Logger.Log(declaringType, Level.Error, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), exception);
			}
		}

		public void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if (IsErrorEnabled)
			{
				Logger.Log(declaringType, Level.Error, new SystemStringFormat(formatProvider, format, args), null);
			}
		}

		public void ErrorFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			if (IsErrorEnabled)
			{
				Logger.Log(declaringType, Level.Error, new SystemStringFormat(formatProvider, format, args), exception);
			}
		}

		public void Fatal(string message)
		{
			if (IsFatalEnabled)
			{
				Logger.Log(declaringType, Level.Fatal, message, null);
			}
		}

		public void Fatal(Func<string> messageFactory)
		{
			if (IsFatalEnabled)
			{
				Logger.Log(declaringType, Level.Fatal, messageFactory.Invoke(), null);
			}
		}

		public void Fatal(string message, Exception exception)
		{
			if (IsFatalEnabled)
			{
				Logger.Log(declaringType, Level.Fatal, message, exception);
			}
		}

		public void FatalFormat(string format, params object[] args)
		{
			if (IsFatalEnabled)
			{
				Logger.Log(declaringType, Level.Fatal, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
			}
		}

		public void FatalFormat(Exception exception, string format, params object[] args)
		{
			if (IsFatalEnabled)
			{
				Logger.Log(declaringType, Level.Fatal, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), exception);
			}
		}

		public void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if (IsFatalEnabled)
			{
				Logger.Log(declaringType, Level.Fatal, new SystemStringFormat(formatProvider, format, args), null);
			}
		}

		public void FatalFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			if (IsFatalEnabled)
			{
				Logger.Log(declaringType, Level.Fatal, new SystemStringFormat(formatProvider, format, args), exception);
			}
		}

		public void Info(string message)
		{
			if (IsInfoEnabled)
			{
				Logger.Log(declaringType, Level.Info, message, null);
			}
		}

		public void Info(Func<string> messageFactory)
		{
			if (IsInfoEnabled)
			{
				Logger.Log(declaringType, Level.Info, messageFactory.Invoke(), null);
			}
		}

		public void Info(string message, Exception exception)
		{
			if (IsInfoEnabled)
			{
				Logger.Log(declaringType, Level.Info, message, exception);
			}
		}

		public void InfoFormat(string format, params object[] args)
		{
			if (IsInfoEnabled)
			{
				Logger.Log(declaringType, Level.Info, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
			}
		}

		public void InfoFormat(Exception exception, string format, params object[] args)
		{
			if (IsInfoEnabled)
			{
				Logger.Log(declaringType, Level.Info, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), exception);
			}
		}

		public void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if (IsInfoEnabled)
			{
				Logger.Log(declaringType, Level.Info, new SystemStringFormat(formatProvider, format, args), null);
			}
		}

		public void InfoFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			if (IsInfoEnabled)
			{
				Logger.Log(declaringType, Level.Info, new SystemStringFormat(formatProvider, format, args), exception);
			}
		}

		public void Warn(string message)
		{
			if (IsWarnEnabled)
			{
				Logger.Log(declaringType, Level.Warn, message, null);
			}
		}

		public void Warn(Func<string> messageFactory)
		{
			if (IsWarnEnabled)
			{
				Logger.Log(declaringType, Level.Warn, messageFactory.Invoke(), null);
			}
		}

		public void Warn(string message, Exception exception)
		{
			if (IsWarnEnabled)
			{
				Logger.Log(declaringType, Level.Warn, message, exception);
			}
		}

		public void WarnFormat(string format, params object[] args)
		{
			if (IsWarnEnabled)
			{
				Logger.Log(declaringType, Level.Warn, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
			}
		}

		public void WarnFormat(Exception exception, string format, params object[] args)
		{
			if (IsWarnEnabled)
			{
				Logger.Log(declaringType, Level.Warn, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), exception);
			}
		}

		public void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if (IsWarnEnabled)
			{
				Logger.Log(declaringType, Level.Warn, new SystemStringFormat(formatProvider, format, args), null);
			}
		}

		public void WarnFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			if (IsWarnEnabled)
			{
				Logger.Log(declaringType, Level.Warn, new SystemStringFormat(formatProvider, format, args), exception);
			}
		}
	}
}