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

	/// <summary>  Logger used in case of failure. Does nothing.
	/// *
	/// </summary>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
	/// </author>
	/// <version> $Id: NullLogSystem.cs,v 1.4 2003/10/27 13:54:10 corts Exp $
	///
	/// </version>
	public class NullLogSystem : ILogSystem
	{
		public NullLogSystem()
		{
		}

		public void Init(IRuntimeServices rs)
		{
		}

		/// <summary>
		/// logs messages to the great Garbage Collector in the sky
		/// </summary>
		/// <param name="level">severity level</param>
		/// <param name="message">complete error message</param>
		public void LogVelocityMessage(LogLevel level, String message)
		{
		}
	}
}