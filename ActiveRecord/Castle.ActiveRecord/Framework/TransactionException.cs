// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord.Framework
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>
	/// This exception is thrown by TransactionScope.
	/// </summary>
	[Serializable]
	public class TransactionException : ActiveRecordException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TransactionException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		public TransactionException(string message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TransactionException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="innerException">The inner exception.</param>
		public TransactionException(string message, Exception innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TransactionException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"></see> is zero (0). </exception>
		/// <exception cref="T:System.ArgumentNullException">The info parameter is null. </exception>
		public TransactionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
