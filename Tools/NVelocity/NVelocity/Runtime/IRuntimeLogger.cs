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

namespace NVelocity.Runtime
{
	using System;

	/// <summary>
	/// Interface for internal runtime logging services that are needed by the
	/// </summary>
	/// <author><a href="mailto:geirm@apache.org">Geir Magusson Jr.</a></author>
	/// <version>$Id: RuntimeLogger.cs,v 1.1 2004/01/02 00:04:50 corts Exp $</version>
	public interface IRuntimeLogger
	{
		/// <summary>
		/// Log a warning message.
		/// </summary>
		/// <param name="message">message to log</param>
		void Warn(Object message);

		/// <summary>
		/// Log an info message.
		/// </summary>
		/// <param name="message">message to log</param>
		void Info(Object message);

		/// <summary>
		/// Log an error message.
		/// </summary>
		/// <param name="message">message to log</param>
		void Error(Object message);

		/// <summary>
		/// Log a debug message.
		/// </summary>
		/// <param name="message">message to log</param>
		void Debug(Object message);
	}
}