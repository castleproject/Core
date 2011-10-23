// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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
	using System.Collections;

	/// <summary>
	/// This is a meta validator. 
	/// It is only useful to test a source content before setting it on the 
	/// target instance.
	/// </summary>
	public class DoubleValidator : AbstractValidator
	{
		/// <summary>
		/// Checks if the <c>fieldValue</c> can be converted to a valid Double.
		/// Null or empty value are allowed.
		/// </summary>
		/// <param name="instance">The target type instance</param>
		/// <param name="fieldValue">The property/field value. It can be null.</param>
		/// <returns>
		/// 	<c>true</c> if the value is accepted (has passed the validation test)
		/// </returns>
		public override bool IsValid(object instance, object fieldValue)
		{
			if (fieldValue == null || fieldValue.ToString() == "") return true;

			Double doubleValue;
			return Double.TryParse(fieldValue.ToString(), out doubleValue);
		}

		/// <summary>
		/// Gets a value indicating whether this validator supports browser validation.
		/// </summary>
		/// <value>
		/// 	<see langword="true"/> if browser validation is supported; otherwise, <see langword="false"/>.
		/// </value>
		public override bool SupportsBrowserValidation
		{
			get { return true; }
		}

		/// <summary>
		/// Applies the browser validation by setting up one or
		/// more input rules on <see cref="IBrowserValidationGenerator"/>.
		/// </summary>
		/// <param name="config">The config.</param>
		/// <param name="inputType">Type of the input.</param>
		/// <param name="generator">The generator.</param>
		/// <param name="attributes">The attributes.</param>
		/// <param name="target">The target.</param>
		public override void ApplyBrowserValidation(BrowserValidationConfiguration config, InputElementType inputType,
		                                            IBrowserValidationGenerator generator, IDictionary attributes,
		                                            string target)
		{
			base.ApplyBrowserValidation(config, inputType, generator, attributes, target);

			generator.SetNumberOnly(target, BuildErrorMessage());
		}

		/// <summary>
		/// Returns the key used to internationalize error messages
		/// </summary>
		/// <value></value>
		protected override string MessageKey
		{
			get { return MessageConstants.InvalidDoubleMessage; }
		}
	}
}