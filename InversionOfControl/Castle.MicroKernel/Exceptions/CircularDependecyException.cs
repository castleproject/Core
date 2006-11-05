// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.MicroKernel.Exceptions
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>
	/// Exception throw when a circular dependency is detected
	/// </summary>
	[Serializable]
	public class CircularDependecyException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CircularDependecyException"/> class.
		/// </summary>
		public CircularDependecyException() : base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CircularDependecyException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		public CircularDependecyException(string message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CircularDependecyException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="innerException">The inner exception.</param>
		public CircularDependecyException(string message, Exception innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CircularDependecyException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is <see langword="null"/>.</exception>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is <see langword="null"/> or <see cref="P:System.Exception.HResult"/> is zero (0).</exception>
		protected CircularDependecyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}