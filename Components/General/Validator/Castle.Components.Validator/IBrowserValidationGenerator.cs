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
	/// Abstracts an JS validation library implementation. 
	/// Each implementation should map the calls to their 
	/// own approach to enforce validation.
	/// </summary>
	public interface IBrowserValidationGenerator
	{
		/// <summary>
		/// Sets the digits only.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="violationMessage">The violation message.</param>
		void SetDigitsOnly(string target, string violationMessage);

		/// <summary>
		/// Sets the number only.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="violationMessage">The violation message.</param>
		void SetNumberOnly(string target, string violationMessage);

		/// <summary>
		/// Sets as required.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="violationMessage">The violation message.</param>
		void SetAsRequired(string target, string violationMessage);

		/// <summary>
		/// Sets the reg exp.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="regExp">The reg exp.</param>
		/// <param name="violationMessage">The violation message.</param>
		void SetRegExp(string target, string regExp, string violationMessage);

		/// <summary>
		/// Sets the email.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="violationMessage">The violation message.</param>
		void SetEmail(string target, string violationMessage);

		/// <summary>
		/// Sets the length of the exact.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="length">The length.</param>
		void SetExactLength(string target, int length);

		/// <summary>
		/// Sets the length of the exact.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="length">The length.</param>
		/// /// <param name="violationMessage">The violation message.</param>
		void SetExactLength(string target, int length, string violationMessage);

		/// <summary>
		/// Sets the length of the min.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="minLength">Length of the min.</param>
		void SetMinLength(string target, int minLength);

		/// <summary>
		/// Sets the length of the min.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="minLength">Length of the min.</param>
		/// <param name="violationMessage">The violation message.</param>
		void SetMinLength(string target, int minLength, string violationMessage);

		/// <summary>
		/// Sets the length of the max.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="maxLength">Length of the max.</param>
		void SetMaxLength(string target, int maxLength);

		/// <summary>
		/// Sets the length of the max.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="maxLength">Length of the max.</param>
		/// <param name="violationMessage">The violation message.</param>
		void SetMaxLength(string target, int maxLength, string violationMessage);

		/// <summary>
		/// Sets the length range.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="minLength">Length of the min.</param>
		/// <param name="maxLength">Length of the max.</param>
		void SetLengthRange(string target, int minLength, int maxLength);

		/// <summary>
		/// Sets the length range.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="minLength">Length of the min.</param>
		/// <param name="maxLength">Length of the max.</param>
		/// <param name="violationMessage">The violation message.</param>
		void SetLengthRange(string target, int minLength, int maxLength, string violationMessage);

		/// <summary>
		/// Sets the value range.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="minValue">Value of the min.</param>
		/// <param name="maxValue">Value of the max.</param>
		/// <param name="violationMessage">The violation message.</param>
		void SetValueRange(string target, int minValue, int maxValue, string violationMessage);

		/// <summary>
		/// Sets the value range.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="minValue">Value of the min.</param>
		/// <param name="maxValue">Value of the max.</param>
		/// <param name="violationMessage">The violation message.</param>
		void SetValueRange(string target, decimal minValue, decimal maxValue, string violationMessage);

		/// <summary>
		/// Sets the value range.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="minValue">Value of the min.</param>
		/// <param name="maxValue">Value of the max.</param>
		/// <param name="violationMessage">The violation message.</param>
		void SetValueRange(string target, DateTime minValue, DateTime maxValue, string violationMessage);

		/// <summary>
		/// Sets the value range.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="minValue">Value of the min.</param>
		/// <param name="maxValue">Value of the max.</param>
		/// <param name="violationMessage">The violation message.</param>
		void SetValueRange(string target, string minValue, string maxValue, string violationMessage);
		
		/// <summary>
		/// Set as same as.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="comparisonFieldName">The name of the field to compare with.</param>
		/// <param name="violationMessage">The violation message.</param>
		void SetAsSameAs(string target, string comparisonFieldName, string violationMessage);

		/// <summary>
		/// Set as not same as.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="comparisonFieldName">The name of the field to compare with.</param>
		/// <param name="violationMessage">The violation message.</param>
		void SetAsNotSameAs(string target, string comparisonFieldName, string violationMessage);

		/// <summary>
		/// Set date.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="violationMessage">The violation message.</param>
		void SetDate(string target, string violationMessage);
	}
}
