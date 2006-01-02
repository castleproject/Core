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

	[Serializable]
	public class ValidationException : Exception
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
		
		public ValidationException( String message ) : base( message )
		{
		}

		public ValidationException( String message, Exception innerException ) : base( message, innerException )
		{
		}

		#region Serialization Support
		public ValidationException( SerializationInfo info, StreamingContext context ) : base( info, context )
		{
			_validationErrorMessages = (String[]) info.GetValue("validationErrorMessages", typeof(String[]));
		}
		
		public override void GetObjectData( SerializationInfo info, StreamingContext context )
		{
			info.AddValue("validationErrorMessages", _validationErrorMessages);
			base.GetObjectData(info, context);
		}
		#endregion
		
		public ValidationException( String message, String[] errorMessages ) : base( message )
		{
			_validationErrorMessages = errorMessages;
		}

		public ValidationException( String message, Exception innerException, String[] errorMessages )  : base( message, innerException )
		{
			_validationErrorMessages = errorMessages;
		}
	}
}
