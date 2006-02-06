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

using Logger = Castle.Services.Logging.ILogger;

namespace Castle.Services.Logging.Log4netIntegration
{
	using System;

	using log4net;
	using log4net.Core;

	/// <summary>
	/// Summary description for log4netLogger.
	/// </summary>
	public class Log4netLogger : Logger
	{
		private static Type declaringType = typeof(Log4netLogger);

		private log4net.Core.ILogger _logger;

		public Log4netLogger()
		{
		}

		internal Log4netLogger(ILog log) : this(log.Logger)
		{
		}

		internal Log4netLogger(ILogger logger)
		{
			_logger = logger;
		}

		public Logger CreateChildLogger(String name)
		{
			return new Log4netLogger(_logger.Repository.GetLogger(name));
		}

		public void Info(String format, params object[] args)
		{
			if (IsInfoEnabled)
				_logger.Log(declaringType, Level.Info, String.Format(format, args), null);
		}

		public void Info(String message, Exception exception)
		{
			if (IsInfoEnabled)
				_logger.Log(declaringType, Level.Info, message, exception);
		}

		public void Info(String message)
		{
			if (IsInfoEnabled)
				_logger.Log(declaringType, Level.Info, message, null);
		}

		public void Debug(String format, params object[] args)
		{
			if (IsDebugEnabled)
				_logger.Log(declaringType, Level.Debug, String.Format(format, args), null);
		}

		public void Debug(String message, Exception exception)
		{
			if (IsDebugEnabled)
				_logger.Log(declaringType, Level.Debug, message, exception);
		}

		public void Debug(String message)
		{
			if (IsDebugEnabled)
				_logger.Log(declaringType, Level.Debug, message, null);
		}

		public void Warn(String format, params object[] args)
		{
			if (IsWarnEnabled)
				_logger.Log(declaringType, Level.Warn, String.Format(format, args), null);
		}

		public void Warn(String message, Exception exception)
		{
			if (IsWarnEnabled)
				_logger.Log(declaringType, Level.Warn, message, exception);
		}

		public void Warn(String message)
		{
			if (IsWarnEnabled)
				_logger.Log(declaringType, Level.Warn, message, null);
		}

		public void FatalError(String format, params object[] args)
		{
			if (IsFatalErrorEnabled)
				_logger.Log(declaringType, Level.Fatal, String.Format(format, args), null);
		}

		public void FatalError(String message, Exception exception)
		{
			if (IsFatalErrorEnabled)
				_logger.Log(declaringType, Level.Fatal, message, exception);
		}

		public void FatalError(String message)
		{
			if (IsFatalErrorEnabled)
				_logger.Log(declaringType, Level.Fatal, message, null);
		}

		public void Error(String format, params object[] args)
		{
			if (IsErrorEnabled)
				_logger.Log(declaringType, Level.Error, String.Format(format, args), null);
		}

		public void Error(String message, Exception exception)
		{
			if (IsErrorEnabled)
				_logger.Log(declaringType, Level.Error, message, exception);
		}

		public void Error(String message)
		{
			if (IsErrorEnabled)
				_logger.Log(declaringType, Level.Error, message, null);
		}

		public bool IsErrorEnabled
		{
			get { return _logger.IsEnabledFor(Level.Error); }
		}

		public bool IsWarnEnabled
		{
			get { return _logger.IsEnabledFor(Level.Warn); }
		}

		public bool IsDebugEnabled
		{
			get { return _logger.IsEnabledFor(Level.Debug); }
		}

		public bool IsFatalErrorEnabled
		{
			get { return _logger.IsEnabledFor(Level.Fatal); }
		}

		public bool IsInfoEnabled
		{
			get { return _logger.IsEnabledFor(Level.Info); }
		}
	}
}
