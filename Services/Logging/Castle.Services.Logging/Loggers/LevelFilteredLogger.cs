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

namespace Castle.Services.Logging
{
	using System;
	using System.ComponentModel;

	/// <summary>
	///	The Level Filtered Logger class.  This is a base clase which
	///	provides a LogLevel attribute and reroutes all functions into
	///	one Log method.
	/// </summary>
	public abstract class LevelFilteredLogger : ILogger
	{
		/// <summary>
		/// Creates a new <c>LevelFilteredLogger</c>.
		/// </summary>
		public LevelFilteredLogger()
		{
		}

		public LevelFilteredLogger(String name)
		{
			ChangeName(name);
		}

		public LevelFilteredLogger(LoggerLevel level)
		{
			Level = level;
		}

		public LevelFilteredLogger(String name, LoggerLevel level)
		{
			ChangeName(name);
			Level = level;
		}


		public abstract ILogger CreateChildLogger(string name);
		protected abstract void Log(LoggerLevel level, String name, String message, Exception exception);
		protected void ChangeName(String name)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			this.name = name;
		}
		private void Log(LoggerLevel level, String message, Exception exception)
		{
			Log(level, Name, message, exception);
		}


		/// <value>
		/// The <c>LoggerLevel</c> that this logger
		/// will be using. Defaults to <c>LoggerLevel.Off</c>
		/// </value>
		public virtual LoggerLevel Level
		{
			get { return level; }
			set 
			{ 
				if (!Enum.IsDefined(typeof(LoggerLevel), value))
					throw new InvalidEnumArgumentException();

				level = value;
			}
		}

		/// <value>
		/// The name that this logger will be using. 
		/// Defaults to <c>String.Empty</c>
		/// </value>
		public virtual String Name
		{
			get { return name; }
		}


		#region ILogger implementation

		public void Debug(string message)
		{
			if (!IsDebugEnabled)
				return;

			Log(LoggerLevel.Debug, message, null);
		}

		public void Debug(string message, Exception exception)
		{
			if (!IsDebugEnabled)
				return;

			Log(LoggerLevel.Debug, message, exception);
		}

		public void Debug(string format, params Object[] args)
		{
			if (!IsDebugEnabled)
				return;

			Log(LoggerLevel.Debug, String.Format(format, args), null);
		}


		public void Info(string message )
		{
			if (!IsInfoEnabled)
				return;

			Log(LoggerLevel.Info, message, null);
		}

		public void Info(string message, Exception exception)
		{
			if (!IsInfoEnabled)
				return;

			Log(LoggerLevel.Info, message, exception);	
		}

		public void Info(string format, params Object[] args)
		{
			if (!IsInfoEnabled)
				return;

			Log(LoggerLevel.Info, String.Format(format, args), null);
		}


		public void Warn(string message)
		{
			if (!IsWarnEnabled)
				return;

			Log(LoggerLevel.Warn, message, null);
		}

		public void Warn(string message, Exception exception)
		{
			if (!IsWarnEnabled)
				return;

			Log(LoggerLevel.Warn, message, exception);
		}

		public void Warn(string format, params Object[] args)
		{
			if (!IsWarnEnabled)
				return;

			Log(LoggerLevel.Warn, String.Format(format, args), null);
		}


		public void Error(string message )
		{
			if (!IsErrorEnabled)
				return;

			Log(LoggerLevel.Error, message, null);
		}

		public void Error(string message, Exception exception)
		{
			if (!IsErrorEnabled)
				return;

			Log(LoggerLevel.Error, message, exception);
		}

		public void Error(string format, params Object[] args)
		{
			if (!IsErrorEnabled)
				return;

			Log(LoggerLevel.Error, String.Format(format, args), null);
		}


		public void FatalError(string message )
		{
			if (!IsFatalErrorEnabled)
				return;

			Log(LoggerLevel.Fatal, message, null);
		}

		public void FatalError(string message, Exception exception)
		{
			if (!IsFatalErrorEnabled)
				return;

			Log(LoggerLevel.Fatal, message, exception);
		}

		public void FatalError(string format, params Object[] args)
		{
			if (!IsFatalErrorEnabled)
				return;

			Log(LoggerLevel.Fatal, String.Format(format, args), null);
		}


		/// <value>
		/// True if the <c>Level</c> is lower
		/// or equal to <c>LoggerLevel.Debug</c>
		/// </value>
		public bool IsDebugEnabled
		{
			get
			{
				return Level <= LoggerLevel.Debug;
			}
		}

		/// <value>
		/// True if the <c>Level</c> is lower
		/// or equal to <c>LoggerLevel.Info</c>
		/// </value>
		public bool IsInfoEnabled
		{
			get
			{
				return Level <= LoggerLevel.Info;
			}
		}

		/// <value>
		/// True if the <c>Level</c> is lower
		/// or equal to <c>LoggerLevel.Warn</c>
		/// </value>
		public bool IsWarnEnabled
		{
			get
			{
				return Level <= LoggerLevel.Warn;
			}
		}
		
		/// <value>
		/// True if the <c>Level</c> is lower
		/// or equal to <c>LoggerLevel.Error</c>
		/// </value>
		public bool IsErrorEnabled
		{
			get
			{
				return Level <= LoggerLevel.Error;
			}
		}

		/// <value>
		/// True if the <c>Level</c> is lower
		/// or equal to <c>LoggerLevel.Fatal</c>
		/// </value>
		public bool IsFatalErrorEnabled
		{
			get 
			{
				return Level <= LoggerLevel.Fatal;
			}
		}


		#endregion

		private LoggerLevel level = LoggerLevel.Off;
		private String name = String.Empty;
	}
}