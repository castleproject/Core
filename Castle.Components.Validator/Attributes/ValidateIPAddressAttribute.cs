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

namespace Castle.Components.Validator.Attributes
{
	using System;

	/// <summary>
	/// Validate that this is a valid address.
	/// </summary>
	[Serializable]
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.ReturnValue | AttributeTargets.Parameter, AllowMultiple = true)]
	public class ValidateIPAddressAttribute : AbstractValidationAttribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ValidateIPAddressAttribute"/> class.
		/// </summary>
		public ValidateIPAddressAttribute()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidateIPAddressAttribute"/> class.
		/// </summary>
		/// <param name="errorMessage">The error message.</param>
		public ValidateIPAddressAttribute(string errorMessage) : base(errorMessage)
		{
		}

		/// <summary>
		/// Constructs and configures an <see cref="IValidator"/>
		/// instance based on the properties set on the attribute instance.
		/// </summary>
		/// <returns></returns>
		public override IValidator Build()
		{
			IValidator validator = new IPAddressValidator();

			ConfigureValidatorMessage(validator);

			return validator;
		}
	}
}