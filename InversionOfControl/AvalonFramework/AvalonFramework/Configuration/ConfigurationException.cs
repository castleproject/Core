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
	using System.Runtime.Serialization; 

	/// <summary>
	/// Thrown when a Configurable component cannot be configured
	///	properly.
	/// </summary>
	[Serializable]
	public class ConfigurationException : Exception
	{
		/// <summary>
		/// Constructs a new <see cref="ConfigurationException"/> instance.
		/// </summary>
		public ConfigurationException(): this(null)
		{
		}

		/// <summary>
		/// Constructs a new <see cref="ConfigurationException"/> instance.
		/// </summary>
		/// <param name="message">The Detail message of the exception.</param>
		public ConfigurationException(string message): this(message, null)
		{
		}

		/// <summary>
		/// Constructs a new <see cref="ConfigurationException"/> instance.
		/// </summary>
		/// <param name="message">The Detail message of the exception.</param>
		/// <param name="inner">The Root cause of the exception.</param>
		public ConfigurationException(string message, Exception inner): base(message, inner)
		{
		}

		/// <summary>
		/// Constructs a new <see cref="ConfigurationException"/> instance.
		/// </summary>
		public ConfigurationException(SerializationInfo info, StreamingContext context): base(info, context)
		{
		}
	}
}
