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
	using System.Collections;
	using System.Reflection;
	using System.Resources;
	using System.Threading;
	using Castle.Components.Validator;

	/// <summary>
	/// Abstract <see cref="IValidator"/> implementation
	/// </summary>
	[Serializable]
	public abstract class AbstractValidator : IValidator
	{
		private static ResourceManager resourceManager;

		private int executionOrder;
		private string errorMessage, friendlyName;
		private PropertyInfo property;
		private RunWhen runWhen;

		/// <summary>
		/// Initializes the <see cref="AbstractValidator"/> class.
		/// </summary>
		static AbstractValidator()
		{
			resourceManager = 
				new ResourceManager("Castle.Components.Validator.Messages", 
					typeof(AbstractValidator).Assembly);
		}

		/// <summary>
		/// Implementors should perform any initialization logic
		/// </summary>
		/// <param name="property">The target property</param>
		public void Initialize(PropertyInfo property)
		{
			this.property = property;

			if (errorMessage == null)
			{
				errorMessage = BuildErrorMessage();
			}
		}

		/// <summary>
		/// Obtains the value of a property or field on a specific instance.
		/// </summary>
		/// <param name="instance">The instance to inspect.</param>
		/// <param name="fieldOrPropertyName">The name of the field or property to inspect.</param>
		/// <returns></returns>
		public object GetFieldOrPropertyValue(object instance, string fieldOrPropertyName)
		{
			PropertyInfo pi = instance.GetType().GetProperty(fieldOrPropertyName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);

			if (pi == null)
			{
				FieldInfo fi = instance.GetType().GetField(fieldOrPropertyName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);

				if (fi != null)
				{
					return fi.GetValue(instance);
				}
			}
			else
			{
				return pi.GetValue(instance, null);
			}

			throw new ValidationException("No public instance field or property named " + fieldOrPropertyName + " for type " + instance.GetType().FullName);
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
		/// Gets a value indicating whether this validator supports web validation.
		/// </summary>
		/// <value>
		/// <see langword="true"/> if web validation is supported; otherwise, <see langword="false"/>.
		/// </value>
		public abstract bool SupportsWebValidation { get; }

		/// <summary>
		/// Applies the web validation by setting up one or
		/// more input rules on <see cref="IWebValidationGenerator"/>.
		/// </summary>
		/// <param name="config">The config.</param>
		/// <param name="inputType">Type of the input.</param>
		/// <param name="generator">The generator.</param>
		/// <param name="attributes">The attributes.</param>
		public abstract void ApplyWebValidation(WebValidationConfiguration config,
		                                        InputElementType inputType, IWebValidationGenerator generator,
		                                        IDictionary attributes);

		/// <summary>
		/// Implementors should perform the actual validation upon
		/// the property value
		/// </summary>
		/// <param name="instance">The target type instance</param>
		/// <returns><c>true</c> if the field is OK</returns>
		public bool IsValid(object instance)
		{
			return IsValid(instance, Property.GetValue(instance, null));
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
			return String.Format(GetResourceForCurrentCulture().GetString(MessageKey), Name);
		}

		/// <summary>
		/// Returns the key used to internationalize error messages
		/// </summary>
		protected virtual string MessageKey
		{
			get { return MessageConstants.GenericInvalidField; }
		}

		/// <summary>
		/// Returns the resource set instance with the validation error messages.
		/// <seealso cref="MessageConstants"/>
		/// </summary>
		/// <returns>A resource set instance</returns>
		protected static ResourceSet GetResourceForCurrentCulture()
		{
			return resourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true); 
		}

		/// <summary>
		/// Gets the property name. The <see cref="FriendlyName"/>
		/// is returned if non-null, otherwise it will return the property name.
		/// </summary>
		protected string Name
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