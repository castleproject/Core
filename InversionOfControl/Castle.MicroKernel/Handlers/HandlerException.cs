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

namespace Castle.MicroKernel.Handlers
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>
	/// Summary description for HandlerException.
	/// </summary>
	[Serializable]
	public class HandlerException : ApplicationException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="HandlerException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		public HandlerException(string message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HandlerException"/> class.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		public HandlerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
