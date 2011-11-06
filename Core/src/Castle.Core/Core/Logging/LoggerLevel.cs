// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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
	/// <summary>
	///   Supporting Logger levels.
	/// </summary>
	public enum LoggerLevel
	{
		/// <summary>
		///   Logging will be off
		/// </summary>
		Off = 0,
		/// <summary>
		///   Fatal logging level
		/// </summary>
		Fatal = 1,
		/// <summary>
		///   Error logging level
		/// </summary>
		Error = 2,
		/// <summary>
		///   Warn logging level
		/// </summary>
		Warn = 3,
		/// <summary>
		///   Info logging level
		/// </summary>
		Info = 4,
		/// <summary>
		///   Debug logging level
		/// </summary>
		Debug = 5,
	}
}