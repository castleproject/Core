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
	/// <summary>
	/// Abstracts an JS validation library implementation. 
	/// Each implementation should map the calls to their 
	/// own approach to enforce validation.
	/// </summary>
	public interface IWebValidationGenerator
	{
		/// <summary>
		/// Sets the digits only.
		/// </summary>
		/// <param name="violationMessage">The violation message.</param>
		void SetDigitsOnly(string violationMessage);

		/// <summary>
		/// Sets the number only.
		/// </summary>
		/// <param name="violationMessage">The violation message.</param>
		void SetNumberOnly(string violationMessage);

		/// <summary>
		/// Sets as required.
		/// </summary>
		/// <param name="violationMessage">The violation message.</param>
		void SetAsRequired(string violationMessage);

		/// <summary>
		/// Sets the reg exp.
		/// </summary>
		/// <param name="regExp">The reg exp.</param>
		/// <param name="violationMessage">The violation message.</param>
		void SetRegExp(string regExp, string violationMessage);

		/// <summary>
		/// Sets the email.
		/// </summary>
		/// <param name="violationMessage">The violation message.</param>
		void SetEmail(string violationMessage);

		/// <summary>
		/// Sets the length of the exact.
		/// </summary>
		/// <param name="length">The length.</param>
		void SetExactLength(int length);

		/// <summary>
		/// Sets the length of the min.
		/// </summary>
		/// <param name="minLength">Length of the min.</param>
		void SetMinLength(int minLength);

		/// <summary>
		/// Sets the length of the max.
		/// </summary>
		/// <param name="maxLength">Length of the max.</param>
		void SetMaxLength(int maxLength);

		/// <summary>
		/// Sets the length range.
		/// </summary>
		/// <param name="minLength">Length of the min.</param>
		/// <param name="maxLength">Length of the max.</param>
		void SetLengthRange(int minLength, int maxLength);
	}
}