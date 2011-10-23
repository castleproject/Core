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
	using System.Reflection;

	/// <summary>
	/// Defines the basic contract for validators. 
	/// <para>
	/// To create a new validation you should use <see cref="AbstractValidator"/> as it 
	/// implements most of the common methods and properties.
	/// </para>
	/// <para>
	/// The validation should happen at <c>IsValid</c>, and if the validator can configure
	/// a client-side validation script, it should use the <see cref="SupportsBrowserValidation"/>
	/// to indicate that it does support client-side validation and also implement the 
	/// <see cref="ApplyBrowserValidation"/> to configure it.
	/// </para>
	/// </summary>
	public interface IValidator
	{
		/// <summary>
		/// Implementors should perform any initialization logic
		/// </summary>
		/// <param name="validationRegistry">The validation registry.</param>
		/// <param name="property">The target property</param>
		void Initialize(IValidatorRegistry validationRegistry, PropertyInfo property);

		/// <summary>
		/// The target property
		/// </summary>
		PropertyInfo Property { get; }

		/// <summary>
		/// Defines when to run the validation. 
		/// Defaults to <c>RunWhen.Everytime</c>
		/// </summary>
		RunWhen RunWhen { get; set; }

		/// <summary>
		/// Gets or sets the validation execution order.
		/// </summary>
		/// <value>The execution order.</value>
		int ExecutionOrder { get; set; }

		/// <summary>
		/// The error message to be displayed if the validation fails
		/// </summary>
		/// <value>The error message.</value>
		string ErrorMessage { get; set; }

		/// <summary>
		/// Gets or sets the a friendly name for the target property
		/// </summary>
		/// <value>The name.</value>
		string FriendlyName { get; set; }

		/// <summary>
		/// Implementors should perform the actual validation upon
		/// the property value
		/// </summary>
		/// <param name="instance"></param>
		/// <returns><c>true</c> if the field is OK</returns>
		bool IsValid(object instance);

		/// <summary>
		/// Implementors should perform the actual validation upon
		/// the property value
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="fieldValue"></param>
		/// <returns><c>true</c> if the field is OK</returns>
		bool IsValid(object instance, object fieldValue);

		/// <summary>
		/// Gets a value indicating whether this validator supports browser validation.
		/// </summary>
		/// <value>
		/// <see langword="true"/> if browser validation is supported; otherwise, <see langword="false"/>.
		/// </value>
		bool SupportsBrowserValidation { get; }

		/// <summary>
		/// Applies the browser validation by setting up one or 
		/// more input rules on <see cref="IBrowserValidationGenerator"/>.
		/// </summary>
		/// <param name="config">The config.</param>
		/// <param name="inputType">Type of the input.</param>
		/// <param name="generator">The generator.</param>
		/// <param name="attributes">The attributes.</param>
		/// <param name="target">The target.</param>
		void ApplyBrowserValidation(BrowserValidationConfiguration config, InputElementType inputType,
		                            IBrowserValidationGenerator generator, IDictionary attributes, string target);

		/// <summary>
		/// Gets the property name. The <see cref="FriendlyName"/>
		/// is returned if non-null, otherwise it will return the property name.
		/// </summary>
		string Name { get; }
	}
}