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

namespace Castle.Components.Validator
{
	using System;

	/// <summary>
	/// The base class for all the validation attributes.
	/// This class define a <seealso cref="Validator"/> property that is used to retrieve the validtor that is used to 
	/// validate the value of the property.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple=true), Serializable]
	public abstract class AbstractValidationAttribute : Attribute, IValidatorBuilder
	{
		private readonly string errorMessage;
		private string friendlyName;

		/// <summary>
		/// Initializes a new instance of the <see cref="AbstractValidationAttribute"/> class.
		/// </summary>
		protected AbstractValidationAttribute()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AbstractValidationAttribute"/> class.
		/// </summary>
		/// <param name="errorMessage">The error message.</param>
		protected AbstractValidationAttribute(string errorMessage)
		{
			this.errorMessage = errorMessage;
		}

		/// <summary>
		/// Gets or sets the a friendly name for the target property
		/// </summary>
		/// <value>The name.</value>
		public string FriendlyName
		{
			get { return friendlyName; }
			set { friendlyName = value; }
		}

		/// <summary>
		/// Gets the error message.
		/// </summary>
		/// <value>The error message.</value>
		protected string ErrorMessage
		{
			get { return errorMessage; }
		}

		public abstract IValidator Build();

		protected void ConfigureValidatorMessage(IValidator validator)
		{
			if (errorMessage != null)
			{
				validator.ErrorMessage = errorMessage;
			}
			if (friendlyName != null)
			{
				validator.FriendlyName = friendlyName;
			}
		}
	}
}
