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

namespace Castle.Components.Validator
{
	using System;
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
		private string friendlyName;
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

			if (errorMessage != null)
			{
				validator.ErrorMessage = errorMessage;
			}
			if (friendlyName != null)
			{
				validator.FriendlyName = friendlyName;
			}
		}
	}
}
