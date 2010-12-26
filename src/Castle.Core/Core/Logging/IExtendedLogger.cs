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
	///   Provides an interface that supports <see cref = "ILogger" /> and
	///   allows the storage and retrieval of Contexts. These are supported in
	///   both log4net and NLog.
	/// </summary>
	public interface IExtendedLogger : ILogger
	{
		/// <summary>
		///   Exposes the Global Context of the extended logger.
		/// </summary>
		IContextProperties GlobalProperties { get; }

		/// <summary>
		///   Exposes the Thread Context of the extended logger.
		/// </summary>
		IContextProperties ThreadProperties { get; }

		/// <summary>
		///   Exposes the Thread Stack of the extended logger.
		/// </summary>
		IContextStacks ThreadStacks { get; }
	}
}