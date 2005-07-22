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

namespace Castle.Facilities.Logging.log4netIntegration
{
	using System;

	using log4net;

	using Castle.Services.Logging;

	/// <summary>
	/// Summary description for log4netLogger.
	/// </summary>
	public class log4netLogger : ILogger
	{
		private ILog _log;

		public log4netLogger()
		{
		}

		internal log4netLogger(ILog log)
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
			_log.InfoFormat(format, args);
		}

		void ILogger.Info(String message, Exception exception)
		{
			_log.Info(message, exception);
		}

		void ILogger.Info(String message)
		{
			_log.Info(message);
		}

		public void Debug(String format, params object[] args)
		{
			// TODO:  Add log4netLogger.Debug implementation
		}

		void ILogger.Debug(String message, Exception exception)
		{
			_log.Debug(message, exception);
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
			_log.WarnFormat(format, args);
		}

		void ILogger.Warn(String message, Exception exception)
		{
			_log.Warn(message, exception);
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
			_log.FatalFormat(format, args);
		}

		void ILogger.FatalError(String message, Exception exception)
		{
			_log.Fatal(message, exception);
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
			_log.ErrorFormat(format, args);
		}

		void ILogger.Error(String message, Exception exception)
		{
			_log.Error(message, exception);
		}

		void ILogger.Error(String message)
		{
			_log.Error(message);
		}

		#endregion
	}
}