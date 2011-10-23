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
	using Validators;

	/// <summary>
	/// Validate that this GUID is a valid one.
	/// </summary>
	[Serializable, CLSCompliant(false)]
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.ReturnValue | AttributeTargets.Parameter, AllowMultiple = true)]
	public class ValidateGuidAttribute : AbstractValidationAttribute
	{
		private readonly bool acceptEmptyGuid;
		/// <summary>Whether or not to treat Guid.Empty as valid - true for yes</summary>
		public bool AcceptEmptyGuid { get { return acceptEmptyGuid; } }


		/// <summary>
		/// Initializes a new instance of the <see cref="ValidateGuidAttribute"/> class.
		/// </summary>
		/// <param name="acceptEmptyGuid">Whether or not to treat Guid.Empty as valid - true for yes</param>
		public ValidateGuidAttribute(bool acceptEmptyGuid)
		{
			this.acceptEmptyGuid = acceptEmptyGuid;
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="ValidateGuidAttribute"/> class.
		/// </summary>
		/// <param name="errorMessage">The error message.</param>
		/// <param name="acceptEmptyGuid">Whether or not to treat Guid.Empty as valid - true for yes</param>
		public ValidateGuidAttribute(bool acceptEmptyGuid, string errorMessage) : base(errorMessage)
		{
			this.acceptEmptyGuid = acceptEmptyGuid;
		}

		/// <summary>
		/// Constructs and configures an <see cref="IValidator"/>
		/// instance based on the properties set on the attribute instance.
		/// </summary>
		/// <returns></returns>
		public override IValidator Build()
		{
			IValidator validator = new GuidValidator(acceptEmptyGuid);

			ConfigureValidatorMessage(validator);

			return validator;
		}
	}
}
