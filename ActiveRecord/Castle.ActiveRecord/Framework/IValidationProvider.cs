// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord.Framework
{
	using System;

	using Castle.Components.Validator;
	using System.Collections;

	/// <summary>
	/// Indicates that something has validation support built in. 
	/// <para>
	/// For a concrete implementation, see <see cref="ActiveRecordValidator"/> and 
	/// <see cref="ActiveRecordHooksValidationBase"/>.
	/// ActiveRecordHooksValidationBase and ActiveRecordValidator both implement IHaveValidation.
	/// The hooks base uses a private IHaveValidation "ActualValidator" to do the actual validation.
	/// The default ActualValidator is a ActiveRecordValidator, but you can override this.
	/// </para>
	/// </summary>
	public interface IValidationProvider
	{
		/// <summary>
		/// Performs the fields validation. Returns true if no 
		/// validation error was found.
		/// </summary>
		/// <returns></returns>
		bool IsValid();

		/// <summary>
		/// Performs the fields validation for the specified action.
		/// </summary>
		/// <param name="runWhen">Use validators appropriate to the action being performed.</param>
		/// <returns>True if no validation error was found</returns>
		bool IsValid(RunWhen runWhen);

		/// <summary>
		/// Returns a list of current validation errors messages.
		/// </summary>
		String[] ValidationErrorMessages
		{
			get;
		}

		/// <summary>
		/// Maps a specific PropertyInfo to a list of
		/// error messages. Useful for frameworks.
		/// </summary>
		IDictionary PropertiesValidationErrorMessages
		{
			get;
		}
	}
}
