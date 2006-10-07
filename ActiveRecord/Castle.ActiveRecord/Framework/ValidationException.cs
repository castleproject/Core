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

namespace Castle.ActiveRecord
{
	using System;
	using System.Runtime.Serialization;

	using Castle.ActiveRecord.Framework;

	/// <summary>
	/// This exception is raised when a validation error occurs
	/// </summary>
	[Serializable]
	public class ValidationException : ActiveRecordException
	{
		private String[] _validationErrorMessages;
		/// <summary>
		/// Returns a list of current validation errors messages, if available.
		/// </summary>
		public String[] ValidationErrorMessages
		{
			get
			{
				if (_validationErrorMessages == null)
				{
					_validationErrorMessages = new String[0];
				}
				return _validationErrorMessages;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidationException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		public ValidationException( String message ) : base( message )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidationException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="innerException">The inner exception.</param>
		public ValidationException( String message, Exception innerException ) : base( message, innerException )
		{
		}

		#region Serialization Support
		/// <summary>
		/// Initializes a new instance of the <see cref="ValidationException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"></see> is zero (0). </exception>
		/// <exception cref="T:System.ArgumentNullException">The info parameter is null. </exception>
		public ValidationException( SerializationInfo info, StreamingContext context ) : base( info, context )
		{
			_validationErrorMessages = (String[]) info.GetValue("validationErrorMessages", typeof(String[]));
		}

		/// <summary>
		/// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> with information about the exception.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
		/// <exception cref="T:System.ArgumentNullException">The info parameter is a null reference (Nothing in Visual Basic). </exception>
		/// <PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter"/></PermissionSet>
		public override void GetObjectData( SerializationInfo info, StreamingContext context )
		{
			info.AddValue("validationErrorMessages", _validationErrorMessages);
			base.GetObjectData(info, context);
		}
		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidationException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="errorMessages">The error messages.</param>
		public ValidationException( String message, String[] errorMessages ) : base( message )
		{
			_validationErrorMessages = errorMessages;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidationException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="innerException">The inner exception.</param>
		/// <param name="errorMessages">The error messages.</param>
		public ValidationException( String message, Exception innerException, String[] errorMessages )  : base( message, innerException )
		{
			_validationErrorMessages = errorMessages;
		}
	}
}
