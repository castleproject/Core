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
	/// Exception signalling a badly formed Context.
	/// 
	/// This can be thrown by Context object when a entry is not
	/// found. It can also be thrown manually in Contextualize()
	/// when Component detects a malformed context value.
	/// </summary>
	[Serializable]
	public class ContextException : Exception
	{
		/// <summary>
		/// Constructs a new <see cref="ContextException"/> instance.
		/// </summary>
		public ContextException(): this( null )
		{
		}

		/// <summary>
		/// Constructs a new <see cref="ContextException"/> instance.
		/// </summary>
		/// <param name="message">The Detail message of the exception.</param>
		public ContextException(string message): this( message, null )
		{
		}

		/// <summary>
		/// Constructs a new <see cref="ContextException"/> instance.
		/// </summary>
		/// <param name="message">The Detail message of the exception.</param>
		/// <param name="inner">The Root cause of the exception.</param>
		public ContextException(string message, Exception inner): base( message, inner )
		{
		}

		/// <summary>
		/// Constructs a new <see cref="ContextException"/> instance.
		/// </summary>
		public ContextException(SerializationInfo info, StreamingContext context): base( info, context )
		{
		}
	}
}
