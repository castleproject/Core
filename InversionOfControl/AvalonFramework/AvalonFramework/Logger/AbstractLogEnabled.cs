// Copyright 2003-2004 The Apache Software Foundation
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

namespace Apache.Avalon.Framework
{
	using System;

	/// <summary>
	/// The Utility class to allow construction of easy components
	/// that will perform logging.
	/// </summary>
	public abstract class AbstractLogEnabled : ILogEnabled
	{
		// Base ILogger instance
		private ILogger m_logger;

		/// <summary>
		/// Gets the ILogger instance.
		/// </summary>
		protected ILogger Logger 
		{
			get
			{
				return m_logger;
			}
		}

		/// <summary>
		/// The Method sets the component logger.
		/// </summary>
		/// <param name="logger">The ILogger instance.</param>
		public void EnableLogging(ILogger logger )
		{
			m_logger = logger;
		}
	}
}
