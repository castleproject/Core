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

	using Framework.Validators;

	[Serializable, CLSCompliant(false)]
	public class ValidateAustralianBusinessNumberAttribute : AbstractValidationAttribute
	{
		/// <summary>
		/// Initializes a new ABN validator.
		/// </summary>
		public ValidateAustralianBusinessNumberAttribute()
			: base(new AustralianBusinessNumberValidator())
		{
		}

		/// <summary>
		/// Initializes a new ABN validator.
		/// </summary>
		/// <param name="errorMessage">The error message to be displayed if the validation fails.</param>
		public ValidateAustralianBusinessNumberAttribute(String errorMessage)
			: base(new AustralianBusinessNumberValidator(), errorMessage)
		{
		}
	}
}