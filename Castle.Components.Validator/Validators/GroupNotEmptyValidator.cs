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
	using System.Collections;
	using System.Reflection;
	using System.Text;

	/// <summary>
	/// Ensures that at least one property in the group was filled with some value
	/// </summary>
	[Serializable]
	public class GroupNotEmptyValidator : IValidator
	{
		private readonly IDictionary properties = new Hashtable();
		private RunWhen runWhen = RunWhen.Everytime;
		private int executionOrder;
		private string errorMessage;
		private string friendlyName;
		private readonly string groupName;
		private IValidatorRegistry validationRegistry;

		/// <summary>
		/// Initializes a new instance of the <see cref="GroupNotEmptyValidator"/> class.
		/// </summary>
		/// <param name="groupName">Name of the group.</param>
		public GroupNotEmptyValidator(string groupName)
		{
			this.groupName = groupName;
		}

		/// <summary>
		/// Implementors should perform any initialization logic
		/// </summary>
		/// <param name="validationRegistry">The validation registry.</param>
		/// <param name="property">The target property</param>
		public void Initialize(IValidatorRegistry validationRegistry, PropertyInfo property)
		{
			this.validationRegistry = validationRegistry;
			properties[property] = FriendlyName ?? property.Name;
		}

		/// <summary>
		/// The target property
		/// </summary>
		public PropertyInfo Property
		{
			get { throw new NotSupportedException("Group validator has more than a single property"); }
		}

		/// <summary>
		/// Defines when to run the validation. 
		/// Defaults to <c>RunWhen.Everytime</c>
		/// </summary>
		public RunWhen RunWhen
		{
			get { return runWhen; }
			set { runWhen = value; }
		}

		/// <summary>
		/// Gets or sets the validation execution order.
		/// </summary>
		/// <value>The execution order.</value>
		public int ExecutionOrder
		{
			get { return executionOrder; }
			set { executionOrder = value; }
		}

		/// <summary>
		/// The error message to be displayed if the validation fails
		/// </summary>
		/// <value>The error message.</value>
		public string ErrorMessage
		{
			get
			{
				if (errorMessage == null)
				{
					errorMessage = BuildErrorMessage();
				}

				return errorMessage;
			}
			set { errorMessage = value; }
		}

		private string BuildErrorMessage()
		{
			return validationRegistry.GetStringFromResource(MessageConstants.GroupNotEmpty);
		}

		/// <summary>
		/// Gets or sets the a friendly name for the target property
		/// </summary>
		/// <value>The name.</value>
		public string FriendlyName
		{
			get { return friendlyName; }
			set { friendlyName = value; }
		}

		/// <summary>
		/// Implementors should perform the actual validation upon
		/// the property value
		/// </summary>
		/// <param name="instance"></param>
		/// <returns><c>true</c> if the field is OK</returns>
		public bool IsValid(object instance)
		{
			bool result = false;

			foreach(PropertyInfo info in properties.Keys)
			{
				object o = info.GetValue(instance, null);

				result |= o != null && o.ToString().Length != 0;
			}

			return result;
		}

		/// <summary>
		/// Implementors should perform the actual validation upon
		/// the property value
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="fieldValue"></param>
		/// <returns><c>true</c> if the field is OK</returns>
		public bool IsValid(object instance, object fieldValue)
		{
			throw new NotSupportedException("Must validate on the entire instance, not a single property");
		}

		/// <summary>
		/// Gets a value indicating whether this validator supports browser validation.
		/// </summary>
		/// <value>
		/// <see langword="true"/> if browser validation is supported; otherwise, <see langword="false"/>.
		/// </value>
		public bool SupportsBrowserValidation
		{
			get { return true; }
		}

		/// <summary>
		/// Applies the browser validation by setting up one or
		/// more input rules on <see cref="IBrowserValidationGenerator"/>.
		/// </summary>
		/// <param name="config">The config.</param>
		/// <param name="inputType">Type of the input.</param>
		/// <param name="generator">The generator.</param>
		/// <param name="attributes">The attributes.</param>
		/// <param name="name">The name.</param>
		public void ApplyBrowserValidation(BrowserValidationConfiguration config, InputElementType inputType,
		                                   IBrowserValidationGenerator generator, IDictionary attributes,
		                                   string name)
		{
			generator.SetAsGroupValidation(name, Name, BuildErrorMessage());
		}

		/// <summary>
		/// Gets the property name. The <see cref="FriendlyName"/>
		/// is returned if non-null, otherwise it will return the property name.
		/// </summary>
		public string Name
		{
			get { return groupName; }
		}
	}
}