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
	using System.Collections;
	using System.Reflection;

	/// <summary>
	/// Validates that the content has the same 
	/// value as the property informed.
	/// </summary>
	public class SameAsValidator : AbstractValidator
	{
		private readonly string propertyToCompare;

		/// <summary>
		/// Initializes a new instance of the <see cref="SameAsValidator"/> class.
		/// </summary>
		/// <param name="propertyToCompare">The property to compare.</param>
		public SameAsValidator(string propertyToCompare)
		{
			this.propertyToCompare = propertyToCompare;
		}


		/// <summary>
		/// Gets the property to compare.
		/// </summary>
		/// <value>The property to compare.</value>
		public string PropertyToCompare
		{
			get { return propertyToCompare; }
		}

		/// <summary>
		/// Validates that the <c>fieldValue</c>
		/// is the same as the property set through the constructor.
		/// </summary>
		/// <param name="instance">The target type instance</param>
		/// <param name="fieldValue">The property/field value. It can be null.</param>
		/// <returns>
		/// 	<c>true</c> if the value is accepted (has passed the validation test)
		/// </returns>
		public override bool IsValid(object instance, object fieldValue)
		{
			PropertyInfo property = instance.GetType().GetProperty(propertyToCompare);

			if (property == null)
			{
				throw new ValidationInternalError("Could not find property " + propertyToCompare + " on type " + instance.GetType());
			}

			object referenceValue = property.GetValue(instance, null);

			if (fieldValue == null && referenceValue == null)
			{
				return true;
			}
			else if (fieldValue != null)
			{
				return fieldValue.Equals(referenceValue);
			}
			else
			{
				return referenceValue.Equals(fieldValue);
			}
		}

		/// <summary>
		/// Gets a value indicating whether this validator supports web validation.
		/// </summary>
		/// <value>
		/// 	<see langword="true"/> if web validation is supported; otherwise, <see langword="false"/>.
		/// </value>
		public override bool SupportsWebValidation
		{
			get { return true; }
		}

		/// <summary>
		/// Applies the web validation by setting up one or
		/// more input rules on <see cref="IWebValidationGenerator"/>.
		/// </summary>
		/// <param name="config">The config.</param>
		/// <param name="inputType">Type of the input.</param>
		/// <param name="generator">The generator.</param>
		/// <param name="attributes">The attributes.</param>
		/// <param name="target">The target.</param>
		public override void ApplyWebValidation(WebValidationConfiguration config, InputElementType inputType,
												IWebValidationGenerator generator, IDictionary attributes, string target)
		{
			base.ApplyWebValidation(config, inputType, generator, attributes, target);

			generator.SetAsSameAs(propertyToCompare, BuildErrorMessage());
		}

		/// <summary>
		/// Returns the key used to internationalize error messages
		/// </summary>
		/// <value></value>
		protected override string MessageKey
		{
			get { return MessageConstants.SameAsMessage; }
		}
	}
}
