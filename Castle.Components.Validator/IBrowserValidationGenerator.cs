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

	/// <summary>
	/// Abstracts a JS validation library implementation. 
	/// Each implementation should map the calls to their 
	/// own approach to enforce validation.
	/// </summary>
	public interface IBrowserValidationGenerator
	{
		/// <summary>
		/// Set that a field should only accept digits.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="violationMessage">The violation message.</param>
		void SetDigitsOnly(string target, string violationMessage);

		/// <summary>
		/// Set that a field should only accept numbers.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="violationMessage">The violation message.</param>
		void SetNumberOnly(string target, string violationMessage);

		/// <summary>
		/// Sets that a field is required.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="violationMessage">The violation message.</param>
		void SetAsRequired(string target, string violationMessage);

		/// <summary>
		/// Sets that a field value must match the specified regular expression.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="regExp">The reg exp.</param>
		/// <param name="violationMessage">The violation message.</param>
		void SetRegExp(string target, string regExp, string violationMessage);

		/// <summary>
		/// Sets that a field value must be a valid email address.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="violationMessage">The violation message.</param>
		void SetEmail(string target, string violationMessage);

		/// <summary>
		/// Sets that field must have an exact lenght.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="length">The length.</param>
		void SetExactLength(string target, int length);

		/// <summary>
		/// Sets that field must have an exact lenght.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="length">The length.</param>
		/// <param name="violationMessage">The violation message.</param>
		void SetExactLength(string target, int length, string violationMessage);

		/// <summary>
		/// Sets that field must have an minimum lenght.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="minLength">The minimum length.</param>
		void SetMinLength(string target, int minLength);

		/// <summary>
		/// Sets that field must have an minimum lenght.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="minLength">The minimum length.</param>
		/// <param name="violationMessage">The violation message.</param>
		void SetMinLength(string target, int minLength, string violationMessage);

		/// <summary>
		/// Sets that field must have an maximum lenght.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="maxLength">The maximum length.</param>
		void SetMaxLength(string target, int maxLength);

		/// <summary>
		/// Sets that field must have an maximum lenght.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="maxLength">The maximum length.</param>
		/// <param name="violationMessage">The violation message.</param>
		void SetMaxLength(string target, int maxLength, string violationMessage);

		/// <summary>
		/// Sets that field must be between a length range.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="minLength">The minimum length.</param>
		/// <param name="maxLength">The maximum length.</param>
		void SetLengthRange(string target, int minLength, int maxLength);

		/// <summary>
		/// Sets that field must be between a length range.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="minLength">The minimum length.</param>
		/// <param name="maxLength">The maximum length.</param>
		/// <param name="violationMessage">The violation message.</param>
		void SetLengthRange(string target, int minLength, int maxLength, string violationMessage);

		/// <summary>
		/// Sets that field must be between a value range.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="minValue">Minimum value.</param>
		/// <param name="maxValue">Maximum value.</param>
		/// <param name="violationMessage">The violation message.</param>
		void SetValueRange(string target, int minValue, int maxValue, string violationMessage);

		/// <summary>
		/// Sets that field must be between a value range.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="minValue">Minimum value.</param>
		/// <param name="maxValue">Maximum value.</param>
		/// <param name="violationMessage">The violation message.</param>
		void SetValueRange(string target, decimal minValue, decimal maxValue, string violationMessage);

		/// <summary>
		/// Sets that field must be between a value range.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="minValue">Minimum value.</param>
		/// <param name="maxValue">Maximum value.</param>
		/// <param name="violationMessage">The violation message.</param>
		void SetValueRange(string target, DateTime minValue, DateTime maxValue, string violationMessage);

		/// <summary>
		/// Sets that field must be between a value range.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="minValue">Minimum value.</param>
		/// <param name="maxValue">Maximum value.</param>
		/// <param name="violationMessage">The violation message.</param>
		void SetValueRange(string target, string minValue, string maxValue, string violationMessage);

		/// <summary>
		/// Set that a field value must be the same as another field's value.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="comparisonFieldName">The name of the field to compare with.</param>
		/// <param name="violationMessage">The violation message.</param>
		void SetAsSameAs(string target, string comparisonFieldName, string violationMessage);

		/// <summary>
		/// Set that a field value must _not_ be the same as another field's value.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="comparisonFieldName">The name of the field to compare with.</param>
		/// <param name="violationMessage">The violation message.</param>
		void SetAsNotSameAs(string target, string comparisonFieldName, string violationMessage);

		/// <summary>
		/// Set that a field value must be a valid date.
		/// </summary>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="violationMessage">The violation message.</param>
		void SetDate(string target, string violationMessage);

		/// <summary>
		/// Sets that a field's value must be greater than another field's value.
		/// </summary>
		/// <remarks>Not implemented by the JQuery validate plugin. Done via a custom rule.</remarks>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="comparisonFieldName">The name of the field to compare with.</param>
		/// <param name="validationType">The type of data to compare.</param>
		/// <param name="violationMessage">The violation message.</param>
		void SetAsGreaterThan(string target, string comparisonFieldName, IsGreaterValidationType validationType,string violationMessage);

		/// <summary>
		/// Sets that a field's value must be lesser than another field's value.
		/// </summary>
		/// <remarks>Not implemented by the JQuery validate plugin. Done via a custom rule.</remarks>
		/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
		/// <param name="comparisonFieldName">The name of the field to compare with.</param>
		/// <param name="validationType">The type of data to compare.</param>
		/// <param name="violationMessage">The violation message.</param>
		void SetAsLesserThan( string target, string comparisonFieldName, IsLesserValidationType validationType, string violationMessage );

		/// <summary>
		/// Sets that a flied is part of a group validation.
		/// </summary>
		/// <remarks>Not implemented by the JQuery validate plugin. Done via a custom rule.</remarks>
		/// <param name="target">The target.</param>
		/// <param name="groupName">Name of the group.</param>
		/// <param name="violationMessage">The violation message.</param>
		void SetAsGroupValidation(string target, string groupName, string violationMessage);
	}
}
