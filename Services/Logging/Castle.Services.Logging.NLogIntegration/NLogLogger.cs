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

namespace Castle.Services.Logging.NLogIntegration
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

		public ILogger CreateChildLogger(String name)
		{
			throw new NotImplementedException("If you need a child logger please use the factory");
		}

		public void Info(String format, params object[] args)
		{
			if(_log.IsInfoEnabled)
				_log.Info(format, args);
		}

		public void Info(String message, Exception exception)
		{
			if(_log.IsInfoEnabled)
				_log.InfoException(message, exception);
		}

		public void Info(String message)
		{
			if(_log.IsInfoEnabled)
				_log.Info(message);
		}

		public void Debug(String format, params object[] args)
		{
			if(_log.IsDebugEnabled)
				_log.Debug(format, args);
		}

		public void Debug(String message, Exception exception)
		{
			if(_log.IsDebugEnabled)
				_log.DebugException(message, exception);
		}

		public void Debug(String message)
		{
			if(_log.IsDebugEnabled)
				_log.Debug(message);
		}

		public void Warn(String format, params object[] args)
		{
			if(_log.IsWarnEnabled)
				_log.Warn(format, args);
		}

		void ILogger.Warn(String message, Exception exception)
		{
			if(_log.IsWarnEnabled)
				_log.WarnException(message, exception);
		}

		void ILogger.Warn(String message)
		{
			if(_log.IsWarnEnabled)
				_log.Warn(message);
		}

		public void FatalError(String format, params object[] args)
		{
			if(_log.IsFatalEnabled)
				_log.Fatal(format, args);
		}

		public void FatalError(String message, Exception exception)
		{
			if(_log.IsFatalEnabled)
				_log.FatalException(message, exception);
		}

		public void FatalError(String message)
		{
			if(_log.IsFatalEnabled)
				_log.Fatal(message);
		}

		public void Error(String format, params object[] args)
		{
			if(_log.IsErrorEnabled)
				_log.Error(format, args);
		}

		public void Error(String message, Exception exception)
		{
			if(_log.IsErrorEnabled)
				_log.ErrorException(message, exception);
		}

		public void Error(String message)
		{
			if(_log.IsErrorEnabled)
				_log.Error(message);
		}

		public bool IsFatalErrorEnabled
		{
			get { return _log.IsFatalEnabled; }
		}

		public bool IsInfoEnabled
		{
			get { return _log.IsInfoEnabled; }
		}

		public bool IsDebugEnabled
		{
			get { return _log.IsDebugEnabled; }
		}

		public bool IsErrorEnabled
		{
			get { return _log.IsErrorEnabled; }
		}

		public bool IsWarnEnabled
		{
			get { return _log.IsWarnEnabled; }
		}
	}
}