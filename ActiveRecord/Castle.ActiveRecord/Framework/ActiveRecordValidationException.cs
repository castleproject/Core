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

namespace Castle.ActiveRecord
{
	using System;
	using System.Runtime.Serialization;
	using Castle.Components.Validator;
	using System.Collections;
	using System.Reflection;

	/// <summary>
	/// Thrown when a AR object does not pass the validation when it is saved or updated.
	/// The <see cref="PropertiesValidationErrorMessages"/> is a map 
	/// of failed properties and their validation errors. 
	/// </summary>
	[Serializable]
	public class ActiveRecordValidationException : ValidationException
	{
		private IDictionary props2errorMessages;

		/// <summary>
		/// Initializes a new instance of the <see cref="ActiveRecordValidationException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		public ActiveRecordValidationException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ActiveRecordValidationException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="innerException">The inner exception.</param>
		public ActiveRecordValidationException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ActiveRecordValidationException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="validationErrorMessages">The error messages.</param>
		/// <param name="propertiesValidationErrorMessages">An optional map of failed properties and their validation errors.</param>
		public ActiveRecordValidationException(string message, string[] validationErrorMessages, IDictionary propertiesValidationErrorMessages)
			: base(message, validationErrorMessages)
		{
			this.props2errorMessages = propertiesValidationErrorMessages;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ActiveRecordValidationException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="innerException">The inner exception.</param>
		/// <param name="validationErrorMessages">The error messages.</param>
		/// <param name="propertiesValidationErrorMessages">An optional map of failed properties and their validation errors.</param>
		public ActiveRecordValidationException(string message, Exception innerException, string[] validationErrorMessages, IDictionary propertiesValidationErrorMessages)
			: base(message, innerException, validationErrorMessages)
		{
			this.props2errorMessages = propertiesValidationErrorMessages;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ActiveRecordValidationException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"></see> is zero (0). </exception>
		/// <exception cref="T:System.ArgumentNullException">The info parameter is null. </exception>
		public ActiveRecordValidationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		/// <summary>
		/// Maps a specific PropertyInfo to a list of error messages. 
		/// Useful to see which property failed the validation when the object tried to be persisted.
		/// </summary>
		public IDictionary PropertiesValidationErrorMessages
		{
			get { return props2errorMessages; }
		}

		/// <summary>
		/// Gets the failed properties as an array of PropertyInfos.
		/// </summary>
		/// <value>The failed properties.</value>
		public PropertyInfo[] FailedProperties
		{
			get
			{
				if (props2errorMessages == null)
				{
					return null;
				}

				PropertyInfo[] props = new PropertyInfo[props2errorMessages.Count];
				props2errorMessages.Keys.CopyTo(props, 0);
				return props;
			}
		}
	}
}
