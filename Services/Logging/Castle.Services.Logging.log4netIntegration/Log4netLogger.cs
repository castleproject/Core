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

namespace Castle.Services.Logging.Log4netIntegration
{
	using System;

	using log4net;

	using Castle.Services.Logging;

	/// <summary>
	/// Summary description for log4netLogger.
	/// </summary>
	public class Log4netLogger : ILogger
	{
		private ILog _log;

		public Log4netLogger()
		{
		}

		internal Log4netLogger(ILog log)
		{
			_log = log;
		}

		#region ILogger Members

		public ILogger CreateChildLogger(String name)
		{
			throw new NotImplementedException();
		}

		public void Info(String format, params object[] args)
		{
			if (_log.IsInfoEnabled)
				_log.InfoFormat(format, args);
		}

		void ILogger.Info(String message, Exception exception)
		{
			if (_log.IsInfoEnabled)
				_log.Info(message, exception);
		}

		void ILogger.Info(String message)
		{
			if (_log.IsInfoEnabled)
				_log.Info(message);
		}

		public void Debug(String format, params object[] args)
		{
			if (_log.IsDebugEnabled)
				_log.DebugFormat(format, args);
		}

		void ILogger.Debug(String message, Exception exception)
		{
			if (_log.IsDebugEnabled)
				_log.Debug(message, exception);
		}

		void ILogger.Debug(String message)
		{
			if (_log.IsDebugEnabled)
				_log.Debug(message);
		}

		public void Warn(String format, params object[] args)
		{
			if (_log.IsWarnEnabled)
				_log.WarnFormat(format, args);
		}

		public void Warn(String message, Exception exception)
		{
			if (_log.IsWarnEnabled)
				_log.Warn(message, exception);
		}

		public void Warn(String message)
		{
			if (_log.IsWarnEnabled)
				_log.Warn(message);
		}

		public void FatalError(String format, params object[] args)
		{
			if (_log.IsFatalEnabled)
				_log.FatalFormat(format, args);
		}

		public void FatalError(String message, Exception exception)
		{
			if (_log.IsFatalEnabled)
				_log.Fatal(message, exception);
		}

		public void FatalError(String message)
		{
			if (_log.IsFatalEnabled)
				_log.Fatal(message);
		}

		public void Error(String format, params object[] args)
		{
			if (_log.IsErrorEnabled)
				_log.ErrorFormat(format, args);
		}

		public void Error(String message, Exception exception)
		{
			if (_log.IsErrorEnabled)
				_log.Error(message, exception);
		}

		public void Error(String message)
		{
			if (_log.IsErrorEnabled)
				_log.Error(message);
		}

		public bool IsErrorEnabled
		{
			get { return _log.IsErrorEnabled; }
		}

		public bool IsWarnEnabled
		{
			get { return _log.IsWarnEnabled; }
		}

		public bool IsDebugEnabled
		{
			get { return _log.IsDebugEnabled; }
		}

		public bool IsFatalErrorEnabled
		{
			get { return _log.IsFatalEnabled; }
		}

		public bool IsInfoEnabled
		{
			get { return _log.IsInfoEnabled; }
		}

		#endregion
	}
}