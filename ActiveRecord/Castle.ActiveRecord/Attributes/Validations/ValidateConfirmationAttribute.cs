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

	using Castle.ActiveRecord.Framework.Validators;

	/// <summary>
	/// This it used when you need to accept two identical inputs from the user, for instnace, 
	/// a password and its confirmation.
	/// </summary>
	[Serializable, CLSCompliant(false)]
	public class ValidateConfirmationAttribute : AbstractValidationAttribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ValidateConfirmationAttribute"/> class.
		/// </summary>
		/// <param name="confirmationFieldOrProperty">The confirmation field or property that should be verified against this one.</param>
		public ValidateConfirmationAttribute(String confirmationFieldOrProperty)
			: 
			base(new ConfirmationValidator(confirmationFieldOrProperty))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidateConfirmationAttribute"/> class.
		/// </summary>
		/// <param name="confirmationFieldOrProperty">The confirmation field or property that should be verified against this one.</param>
		/// <param name="errorMessage">The error message to display if this property and the configuration property are not the same.</param>
		public ValidateConfirmationAttribute(String confirmationFieldOrProperty, String errorMessage) : 
			base(new ConfirmationValidator(confirmationFieldOrProperty), errorMessage)
		{
		}
	}
}
