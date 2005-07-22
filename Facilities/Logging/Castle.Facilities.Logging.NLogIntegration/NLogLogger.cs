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

namespace Castle.Facilities.Logging.NLogIntegration
{
	using System;
	using Castle.Services.Logging;
	using NLog;

	/// <summary>
	/// Summary description for NLogLogger.
	/// </summary>
	public class NLogLogger : ILogger
	{
		private Logger _log;

		public NLogLogger(Logger log)
		{
			_log = log;
		}

		#region ILogger Members

		public ILogger CreateChildLogger(String name)
		{
			throw new NotImplementedException("If you need a child logger please use the factory");
		}

		public void Info(String format, params object[] args)
		{
			_log.Info(format, args);
		}

		void ILogger.Info(String message, Exception exception)
		{
			_log.InfoException(message, exception);
		}

		void ILogger.Info(String message)
		{
			_log.Info(message);
		}

		public void Debug(String format, params object[] args)
		{
			_log.Debug(format, args);
		}

		void ILogger.Debug(String message, Exception exception)
		{
			_log.DebugException(message, exception);
		}

		void ILogger.Debug(String message)
		{
			_log.Debug(message);
		}

		public bool IsErrorEnabled
		{
			get { return _log.IsErrorEnabled; }
		}

		public bool IsWarnEnabled
		{
			get { return _log.IsWarnEnabled; }
		}

		public void Warn(String format, params object[] args)
		{
			_log.Warn(format, args);
		}

		void ILogger.Warn(String message, Exception exception)
		{
			_log.WarnException(message, exception);
		}

		void ILogger.Warn(String message)
		{
			_log.Warn(message);
		}

		public bool IsFatalErrorEnabled
		{
			get { return _log.IsFatalEnabled; }
		}

		public bool IsInfoEnabled
		{
			get { return _log.IsInfoEnabled; }
		}

		public void FatalError(String format, params object[] args)
		{
			_log.Fatal(format, args);
		}

		void ILogger.FatalError(String message, Exception exception)
		{
			_log.FatalException(message, exception);
		}

		void ILogger.FatalError(String message)
		{
			_log.Fatal(message);
		}

		public bool IsDebugEnabled
		{
			get { return _log.IsDebugEnabled; }
		}

		public void Error(String format, params object[] args)
		{
			_log.Error(format, args);
		}

		void ILogger.Error(String message, Exception exception)
		{
			_log.ErrorException(message, exception);
		}

		void ILogger.Error(String message)
		{
			_log.Error(message);
		}

		#endregion
	}
}