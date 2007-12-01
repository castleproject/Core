// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>
	/// Base exception for monorail exceptions
	/// </summary>
	[Serializable]
	public class MonoRailException : ApplicationException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MonoRailException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		public MonoRailException(String message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MonoRailException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="args">The args.</param>
		public MonoRailException(String message, params object[] args) : this(String.Format(message, args))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MonoRailException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="innerException">The inner exception.</param>
		public MonoRailException(String message, Exception innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MonoRailException"/> class.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		public MonoRailException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
