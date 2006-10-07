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

	using Castle.ActiveRecord.Framework;


	/// <summary>
	/// The base class for all the validation attributes.
	/// This class define a <seealso cref="Validator"/> property that is used to retrieve the validtor that is used to 
	/// validate the value of the property.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple=true), Serializable, CLSCompliant(false)]
	public abstract class AbstractValidationAttribute : Attribute
	{
		private IValidator _validator;

		/// <summary>
		/// Initializes a new instance of the <see cref="AbstractValidationAttribute"/> class.
		/// </summary>
		/// <param name="validator">The validator.</param>
		public AbstractValidationAttribute(IValidator validator)
		{
			_validator = validator;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AbstractValidationAttribute"/> class.
		/// </summary>
		/// <param name="validator">The validator.</param>
		/// <param name="errorMessage">The error message.</param>
		public AbstractValidationAttribute(IValidator validator, String errorMessage) : this(validator)
		{
			validator.ErrorMessage = errorMessage;
		}

		/// <summary>
		/// Gets the validator for this attribute.
		/// Each attribute that inherits from this class will have its own validtor.
		/// </summary>
		/// <value>The validator.</value>
		public IValidator Validator
		{
			get { return _validator; }
		}
	}
}
