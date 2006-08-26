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

namespace Castle.Core.Logging
{
	using System;

	/// <summary>
	///	The Null Logger class.  This is useful for implementations where you need
	/// to provide a logger to a utility class, but do not want any output from it.
	/// It also helps when you have a utility that does not have a logger to supply.
	/// </summary>
	public class NullLogger : ILogger
	{
		public static readonly NullLogger Instance = new NullLogger();
		
		/// <summary>
		/// Creates a new <c>NullLogger</c>.
		/// </summary>
		public NullLogger()
		{
		}

		/// <summary>
		/// No-op.
		/// </summary>
		/// <param name="message">Ignored</param>
		public void Debug(string message)
		{
		}

		/// <summary>
		/// No-op.
		/// </summary>
		/// <param name="message">Ignored</param>
		/// <param name="exception">Ignored</param>
		public void Debug(string message, Exception exception)
		{
		}

		/// <summary>
		/// No-op.
		/// </summary>
		/// <param name="format">Ignored</param>
		/// <param name="args">Ignored</param>
		public void Debug(string format, params Object[] args)
		{
		}

		/// <summary>
		/// No-op.
		/// </summary>
		/// <value>false</value> 
		public bool IsDebugEnabled
		{
			get { return false; }
		}

		/// <summary>
		/// No-op.
		/// </summary>
		/// <param name="message">Ignored</param>
		public void Info(string message)
		{
		}

		/// <summary>
		/// No-op.
		/// </summary>
		/// <param name="message">Ignored</param>
		/// <param name="exception">Ignored</param>
		public void Info(string message, Exception exception)
		{
		}

		/// <summary>
		/// No-op.
		/// </summary>
		/// <param name="format">Ignored</param>
		/// <param name="args">Ignored</param>
		public void Info(string format, params Object[] args)
		{
		}

		/// <summary>
		/// No-op.
		/// </summary>
		/// <value>false</value> 
		public bool IsInfoEnabled
		{
			get { return false; }
		}

		/// <summary>
		/// No-op.
		/// </summary>
		/// <param name="message">Ignored</param>
		public void Warn(string message)
		{
		}

		/// <summary>
		/// No-op.
		/// </summary>
		/// <param name="message">Ignored</param>
		/// <param name="exception">Ignored</param>
		public void Warn(string message, Exception exception)
		{
		}

		/// <summary>
		/// No-op.
		/// </summary>
		/// <param name="format">Ignored</param>
		/// <param name="args">Ignored</param>
		public void Warn(string format, params Object[] args)
		{
		}

		/// <summary>
		/// No-op.
		/// </summary>
		/// <value>false</value> 
		public bool IsWarnEnabled
		{
			get { return false; }
		}

		/// <summary>
		/// No-op.
		/// </summary>
		/// <param name="message">Ignored</param>
		public void Error(string message)
		{
		}

		/// <summary>
		/// No-op.
		/// </summary>
		/// <param name="message">Ignored</param>
		/// <param name="exception">Ignored</param>
		public void Error(string message, Exception exception)
		{
		}

		/// <summary>
		/// No-op.
		/// </summary>
		/// <param name="format">Ignored</param>
		/// <param name="args">Ignored</param>
		public void Error(string format, params Object[] args)
		{
		}

		/// <summary>
		/// No-op.
		/// </summary>
		/// <value>false</value> 
		public bool IsErrorEnabled
		{
			get { return false; }
		}

		/// <summary>
		/// No-op.
		/// </summary>
		/// <param name="message">Ignored</param>
		public void FatalError(string message)
		{
		}

		/// <summary>
		/// No-op.
		/// </summary>
		/// <param name="message">Ignored</param>
		/// <param name="exception">Ignored</param>
		public void FatalError(string message, Exception exception)
		{
		}

		/// <summary>
		/// No-op.
		/// </summary>
		/// <param name="format">Ignored</param>
		/// <param name="args">Ignored</param>
		public void FatalError(string format, params Object[] args)
		{
		}

		/// <summary>
		/// No-op.
		/// </summary>
		/// <value>false</value> 
		public bool IsFatalErrorEnabled
		{
			get { return false; }
		}

		/// <summary>
		/// Returns this <c>NullLogger</c>.
		/// </summary>
		/// <param name="name">Ignored</param>
		/// <returns>This ILogger instance.</returns> 
		public ILogger CreateChildLogger(string name)
		{
			return this;
		}
	}
}