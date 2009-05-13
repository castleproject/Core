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

	public enum LogLevel
	{
		/// <summary>
		/// Prefix for debug messages.
		/// </summary>
		Debug = 0,

		/// <summary>
		/// Prefix for info messages.
		/// </summary>
		Info = 1,

		/// <summary>
		/// Prefix for warning messages.
		/// </summary>
		Warn = 2,

		/// <summary>
		/// Prefix for error messages.
		/// </summary>
		Error = 3,
	}

	/// <summary>
	/// Base interface that Logging systems need to implement.
	/// </summary>
	/// <author> <a href="mailto:jon@latchkey.com">Jon S. Stevens</a></author>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a></author>
	public interface ILogSystem
	{
		/// <summary>
		/// init()
		/// </summary>
		void Init(IRuntimeServices rs);

		/// <summary>
		/// Send a log message from Velocity.
		/// </summary>
		void LogVelocityMessage(LogLevel level, String message);
	}
}