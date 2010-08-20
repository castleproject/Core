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
	using System.Reflection;

	/// <summary>
	/// Abstracts a validation registry per <see cref="Type"/>.
	/// </summary>
	public interface IValidatorRegistry
	{
		/// <summary>
		/// Gets all validators associated with a <see cref="Type"/>.
		/// <para>
		/// The validators returned are initialized.
		/// </para>
		/// </summary>
		/// <param name="validatorRunner">The validator runner.</param>
		/// <param name="targetType">Target type.</param>
		/// <param name="runWhen">Restrict the set returned to the phase specified</param>
		/// <returns>A Validator array</returns>
		IValidator[] GetValidators(IValidatorRunner validatorRunner, Type targetType, RunWhen runWhen);

		/// <summary>
		/// Gets all validators associated with a property.
		/// <para>
		/// The validators returned are initialized.
		/// </para>
		/// </summary>
		/// <param name="validatorRunner">The validator runner.</param>
		/// <param name="targetType">Target type.</param>
		/// <param name="property">The property.</param>
		/// <param name="runWhen">Restrict the set returned to the phase specified</param>
		/// <returns>A Validator array</returns>
		IValidator[] GetValidators(IValidatorRunner validatorRunner, Type targetType, PropertyInfo property, RunWhen runWhen);

		/// <summary>
		/// Gets the property value accessor.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <returns>The property value accessor.</returns>
		Accessor GetPropertyAccessor(PropertyInfo property);

		/// <summary>
		/// Gets the expression value accessor.
		/// </summary>
		/// <param name="targetType">The target type.</param>
		/// <param name="path">The expression path.</param>
		/// <returns>The expression accessor.</returns>
		Accessor GetFieldOrPropertyAccessor(Type targetType, string path);

		/// <summary>
		/// Gets the string from resource by key
		/// </summary>
		/// <param name="key">The key.</param>
		string GetStringFromResource(string key);
	}
}
