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

	using System.Collections;
	
	/// <summary>
	/// Defines the entry point for validation.
	/// </summary>
	public interface IValidatorRunner
	{
		/// <summary>
		/// Determines whether the specified instance is valid.
		/// <para>
		/// All validators are run.
		/// </para>
		/// </summary>
		/// <param name="objectInstance">The object instance to be validated (cannot be null).</param>
		/// <returns>
		/// 	<see langword="true"/> if the specified obj is valid; otherwise, <see langword="false"/>.
		/// </returns>
		bool IsValid(object objectInstance);

		/// <summary>
		/// Determines whether the specified instance is valid.
		/// <para>
		/// All validators are run for the specified <see cref="RunWhen"/> phase.
		/// </para>
		/// </summary>
		/// <param name="objectInstance">The object instance to be validated (cannot be null).</param>
		/// <param name="runWhen">Restrict the set returned to the phase specified</param>
		/// <returns>
		/// <see langword="true"/> if the specified instance is valid; otherwise, <see langword="false"/>.
		/// </returns>
		bool IsValid(object objectInstance, RunWhen runWhen);

		/// <summary>
		/// Checks whether an error summary exists for this instance.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <returns>
		/// <see langword="true"/> if and only if an error summary exists. See <see cref="GetErrorSummary"/>
		/// for detailed conditions.
		/// </returns>
		bool HasErrors(object instance);
		
		/// <summary>
		/// Gets the error list per instance.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <returns>
		/// The error summary for the instance or <see langword="null"/> if the instance
		/// was either valid or has not been validated before.
		/// </returns>
		ErrorSummary GetErrorSummary(object instance);
		
		/// <summary>
		/// Gets the extended properties, which allows <see cref="IValidator"/> 
		/// implementation to store additional information to track state.
		/// </summary>
		/// <value>The extended properties.</value>
		IDictionary ExtendedProperties { get ; }
	}
}
