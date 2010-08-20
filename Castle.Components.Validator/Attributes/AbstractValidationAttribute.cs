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
	using System.Globalization;
	using System.Reflection;

	/// <summary>
	/// Represents "phases" in which you can group 
	/// different validations and run then accordingly
	/// </summary>
	[Flags]
	public enum RunWhen
	{
		/// <summary>
		/// Run all validations
		/// </summary>
		Everytime = 0x1,
		/// <summary>
		/// Only during an insertion phase
		/// </summary>
		Insert = 0x2,
		/// <summary>
		/// Only during an update phase
		/// </summary>
		Update = 0x4,
		/// <summary>
		/// Defines a custom phase
		/// </summary>
		Custom = 0x8,
	}

	/// <summary>
	/// The base class for all the validation attributes.
	/// This class define a <seealso cref="Validator"/> property that is used to retrieve the validtor that is used to 
	/// validate the value of the property.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple=true), Serializable]
	public abstract class AbstractValidationAttribute : Attribute, IValidatorBuilder
	{
		private readonly string errorMessage;
		private string friendlyName, friendlyNameKey, errorMessageKey;
		private Type resourceType;
		private int executionOrder = 0;
		private RunWhen runWhen = RunWhen.Everytime;
		private Accessor propertyAccessor;

		/// <summary>
		/// Initializes a new instance of the <see cref="AbstractValidationAttribute"/> class.
		/// </summary>
		protected AbstractValidationAttribute()
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AbstractValidationAttribute"/> class.
		/// </summary>
		/// <param name="errorMessage">The error message.</param>
		protected AbstractValidationAttribute(string errorMessage)
		{
			this.errorMessage = errorMessage;
		}

		/// <summary>
		/// Implementors should perform any initialization logic
		/// </summary>
		/// <param name="validationRegistry">The validation registry.</param>
		/// <param name="property">The target property</param>
		public virtual void Initialize(IValidatorRegistry validationRegistry, PropertyInfo property)
		{
			propertyAccessor = validationRegistry.GetPropertyAccessor(property);
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
		/// Gets or sets the a friendly name for the target property
		/// </summary>
		/// <value>The name.</value>
		public string FriendlyName
		{
			get { return friendlyName; }
			set { friendlyName = value; }
		}

		/// <summary>
		/// Gets the error message.
		/// </summary>
		/// <value>The error message.</value>
		protected string ErrorMessage
		{
			get { return errorMessage; }
		}

		///<summary>
		/// Must be set when using FriendlyNameKey or ErrorMessageKey with default resource localization support.
		///</summary>
		/// <value>the ressource type (generated type from .resx)</value>
		public Type ResourceType {
			get { return resourceType; }
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				resourceType = value;
			}
		}

		/// <summary>
		/// Gets or sets the resource name of the friendly name for the target property.
		/// </summary>
		public string FriendlyNameKey {
			get { return friendlyNameKey; }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentException("Value cannot be null or empty.", "value");
				}
				friendlyNameKey = value;
			}
		}

		/// <summary>
		/// Gets or sets the error message resource name to use as lookup for the <see cref="ResourceType"/> if a validation fails. 
		/// </summary>
		public string ErrorMessageKey {
			get { return errorMessageKey; }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentException("Value cannot be null or empty.", "value");
				}
				errorMessageKey = value;
			}
		}

		/// <summary>
		/// Gets the property accessor;
		/// </summary>
		protected Accessor PropertyAccessor
		{
			get { return propertyAccessor; }
		}

		/// <summary>
		/// Constructs and configures an <see cref="IValidator"/>
		/// instance based on the properties set on the attribute instance.
		/// </summary>
		/// <returns></returns>
		public abstract IValidator Build();

		/// <summary>
		/// Constructs and configures an <see cref="IValidator"/>
		/// instance based on the properties set on the attribute instance.
		/// </summary>
		public virtual IValidator Build(IValidatorRunner validatorRunner, Type type)
		{
			IValidator validator = Build();
			
			if (validator is IPropertyAccessAware)
			{
				((IPropertyAccessAware)validator).PropertyAccessor = propertyAccessor;
			}

			return validator;
		}

		/// <summary>
		/// Applies the common configuration defined on the attribute.
		/// </summary>
		/// <param name="validator">The validator instance.</param>
		protected void ConfigureValidatorMessage(IValidator validator)
		{
			validator.RunWhen = runWhen;
			validator.ExecutionOrder = executionOrder;

			string solvedErrorMessage = errorMessage;
			string solvedFriendlyName = friendlyName;

			if (ResourceType != null)
			{
				if (!String.IsNullOrEmpty(ErrorMessageKey))
				{
					solvedErrorMessage = GetLocalizedMessageByKey(ErrorMessageKey);
				}
				if (!String.IsNullOrEmpty(FriendlyNameKey))
				{
					solvedFriendlyName = GetLocalizedMessageByKey(FriendlyNameKey);
				}
			}
			else if (ResourceType == null && (!String.IsNullOrEmpty(ErrorMessageKey) || !String.IsNullOrEmpty(FriendlyNameKey)))
			{
				throw new ArgumentException(
					"You have set ErrorMessageKey and/or FriendlyNameKey but have not specified the ResourceType to use for lookup.");
			}

			// default message resolution: use validator attribute properties
			if (solvedErrorMessage != null)
			{
				validator.ErrorMessage = solvedErrorMessage;
			}
			if (solvedFriendlyName != null)
			{
				validator.FriendlyName = solvedFriendlyName;
			}
		}

		// this may be refactored with something more abstract when supporting other localization schemes
		string GetLocalizedMessageByKey(string key)
		{
			PropertyInfo property = ResourceType.GetProperty(key, BindingFlags.Public | BindingFlags.Static);
			if (property == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "The resource type '{0}' does not have a publicly visible static property named '{1}'. You probably marked the resources as internal, to fix this change the 'Access modifier' dropdown to 'Public' in the VS resources editor.", new object[] { ResourceType.FullName, this.ErrorMessageKey}));
			}
			if (property.PropertyType != typeof(string))
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "The property '{0}' on resource type '{1}' is not a string type.", new object[] { property.Name, ResourceType.FullName }));
			}

			return (string) property.GetValue(null, null);
		}
	}
}
