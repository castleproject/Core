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
	/// The Exception thrown to indicate a problem with service.
	/// </summary>
	/// <remarks>
	/// It is usually thrown by <see cref="ILookupManager"/>
	/// </remarks> 
	[Serializable]
	public class LookupException : Exception
	{
		/// <summary>
		/// Constructs a new <c>LookupException</c> instance.
		/// </summary>
		public LookupException() : this(null)
		{
		}

		/// <summary>
		/// Constructs a new <c>LookupException</c> instance.
		/// </summary>
		/// <param name="message">The Detail message for this exception.</param>
		public LookupException(String message) : this(null, message)
		{
		}

		/// <summary>
		/// Constructs a new <c>LookupException</c> instance.
		/// </summary>
		/// <param name="role">The Role that caused the exception.</param>
		/// <param name="message">The Detail message for this exception.</param>
		public LookupException(String role, String message) : this(role, message, null)
		{
		}

		/// <summary>
		/// Constructs a new <c>LookupException</c> instance.
		/// </summary>
		/// <param name="message">The Detail message for this exception.</param>
		/// <param name="inner">The Root cause of the exception.</param>
		public LookupException(String message, Exception inner) : this(String.Empty, message, inner)
		{
		}

		/// <summary>
		/// Constructs a new <c>LookupException</c> instance.
		/// </summary>
		/// <param name="role">The Role that caused the exception.</param>
		/// <param name="message">The Detail message for this exception.</param>
		/// <param name="inner">The Root cause of the exception.</param>
		public LookupException(String role, String message, Exception inner) : 
			base(String.Format("Component for role '{0}' could not be resolved. " + 
							   "Detailed message: {1}", role, message), inner)
		{
		}
	}
}
