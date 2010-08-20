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

	/// <summary>
	/// Abstract <see cref="IValidator"/> implementation
	/// </summary>
	[Serializable]
	public abstract class AbstractValidator : IValidator, IPropertyAccessAware
	{
		private int executionOrder;
		private string errorMessage, friendlyName;
		private PropertyInfo property;
		private RunWhen runWhen;
		private IValidatorRegistry validationRegistry;
		private Accessor propertyAccessor;

		/// <summary>
		/// Implementors should perform any initialization logic
		/// </summary>
		/// <param name="validationRegistry"></param>
		/// <param name="property">The target property</param>
		public virtual void Initialize(IValidatorRegistry validationRegistry, PropertyInfo property)
		{
			this.property = property;
			this.validationRegistry = validationRegistry;
			if (errorMessage == null)
			{
				errorMessage = BuildErrorMessage();
			}
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
		/// Defines when to run the validation. 
		/// Defaults to <c>RunWhen.Everytime</c>
		/// </summary>
		public RunWhen RunWhen
		{
			get { return runWhen; }
			set { runWhen = value; }
		}

		/// <summary>
		/// The target property
		/// </summary>
		public PropertyInfo Property
		{
			get { return property; }
		}

		/// <summary>
		/// The error message to be displayed if the validation fails
		/// </summary>
		public String ErrorMessage
		{
			get { return errorMessage; }
			set { errorMessage = value; }
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
		/// Sets the property accessor.
		/// </summary>
		public Accessor PropertyAccessor
		{
			set { propertyAccessor = value; }
		}

		/// <summary>
		/// Gets a value indicating whether this validator supports browser validation.
		/// </summary>
		/// <value>
		/// <see langword="true"/> if browser validation is supported; otherwise, <see langword="false"/>.
		/// </value>
		public abstract bool SupportsBrowserValidation { get; }

		/// <summary>
		/// Applies the browser validation by setting up one or
		/// more input rules on <see cref="IBrowserValidationGenerator"/>.
		/// </summary>
		/// <param name="config">The config.</param>
		/// <param name="inputType">Type of the input.</param>
		/// <param name="generator">The generator.</param>
		/// <param name="attributes">The attributes.</param>
		/// <param name="target">The target.</param>
		public virtual void ApplyBrowserValidation(BrowserValidationConfiguration config,
		                                           InputElementType inputType, IBrowserValidationGenerator generator,
		                                           IDictionary attributes, string target)
		{
		}

		/// <summary>
		/// Implementors should perform the actual validation upon
		/// the property value
		/// </summary>
		/// <param name="instance">The target type instance</param>
		/// <returns><c>true</c> if the field is OK</returns>
		public bool IsValid(object instance)
		{
			object value;

			if (propertyAccessor != null)
			{
				value = propertyAccessor(instance);
			}
			else
			{
				value = AccessorUtil.GetPropertyValue(instance, Property);
			}

			return IsValid(instance, value);
		}

		/// <summary>
		/// Implementors should perform the actual validation upon
		/// the property value
		/// </summary>
		/// <param name="instance">The target type instance</param>
		/// <param name="fieldValue">The property/field value. It can be null.</param>
		/// <returns><c>true</c> if the value is accepted (has passed the validation test)</returns>
		public abstract bool IsValid(object instance, object fieldValue);

		/// <summary>
		/// Builds the error message.
		/// </summary>
		protected virtual string BuildErrorMessage()
		{
			if (!String.IsNullOrEmpty(ErrorMessage))
			{
				// Could be localized by AbstractValidationAttribute.ConfigureValidatorMessage
				return ErrorMessage;
			}
			return String.Format(GetString(MessageKey), Name);
		}

		/// <summary>
		/// Gets the string from resource
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		protected string GetString(string key)
		{
			return validationRegistry.GetStringFromResource(key);
		}

		/// <summary>
		/// Returns the key used to internationalize error messages
		/// </summary>
		protected virtual string MessageKey
		{
			get { return MessageConstants.GenericInvalidField; }
		}

		/// <summary>
		/// Gets the property name. The <see cref="FriendlyName"/>
		/// is returned if non-null, otherwise it will return the property name.
		/// </summary>
		public string Name
		{
			get
			{
				if (friendlyName != null)
				{
					return friendlyName;
				}

				return Property.Name;
			}
		}
	}
}