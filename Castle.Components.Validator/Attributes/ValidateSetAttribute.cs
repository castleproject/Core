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

	using Castle.Components.Validator;

	/// <summary>
	/// Validate that the field has a value in a set of values.
	/// </summary>
	[Serializable, CLSCompliant(false)]
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.ReturnValue | AttributeTargets.Parameter, AllowMultiple = true)]
	public class ValidateSetAttribute : AbstractValidationAttribute
	{
		private readonly IValidator validator;

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidateSetAttribute"/> class.
		/// </summary>
		public ValidateSetAttribute() : base()
		{
			validator = new SetValidator();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidateSetAttribute"/> class.
		/// </summary>
		/// <param name="errorMessage">The error message to be displayed if the validation fails.</param>
		public ValidateSetAttribute(string errorMessage) : base(errorMessage)
		{
			validator = new SetValidator();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidateSetAttribute"/> class.
		/// </summary>
		/// <param name="set">The set of values to compare against.</param>
		public ValidateSetAttribute(params string[] set) : base()
		{
			validator = new SetValidator(set);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidateSetAttribute"/> class.
		/// </summary>
		/// <param name="set">The set of values to compare against.</param>
		public ValidateSetAttribute(params int[] set)
			: base()
		{
			validator = new SetValidator(set);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidateSetAttribute"/> class.
		/// </summary>
		/// <param name="errorMessage">The error message to be displayed if the validation fails.</param>
		/// <param name="set">The set of values to compare against.</param>
		public ValidateSetAttribute(string errorMessage, params string[] set)
			: base(errorMessage)
		{
			validator = new SetValidator(set);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidateSetAttribute"/> class.
		/// </summary>
		/// <param name="errorMessage">The error message to be displayed if the validation fails.</param>
		/// <param name="set">The set of values to compare against.</param>
		public ValidateSetAttribute(string errorMessage, params int[] set)
			: base(errorMessage)
		{
			validator = new SetValidator(set);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidateSetAttribute"/> class.
		/// </summary>
		/// <param name="type">The <see cref="System.Type" /> of an <c>enum</c> class.
		/// The enum names will be added to the contents of the set.</param>
		public ValidateSetAttribute(Type type) : base()
		{
			validator = new SetValidator(type);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidateSetAttribute"/> class.
		/// </summary>
		/// <param name="type">The <see cref="System.Type" /> of an <c>enum</c> class.
		/// The enum names will be added to the contents of the set.</param>
		/// <param name="errorMessage">The error message to be displayed if the validation fails.</param>
		public ValidateSetAttribute(Type type, string errorMessage) : base(errorMessage)
		{
			validator = new SetValidator(type);
		}

		/// <summary>
		/// Constructs and configures an <see cref="IValidator"/>
		/// instance based on the properties set on the attribute instance.
		/// </summary>
		/// <returns></returns>
		public override IValidator Build()
		{
			ConfigureValidatorMessage(validator);
			return validator;
		}
	}
}
