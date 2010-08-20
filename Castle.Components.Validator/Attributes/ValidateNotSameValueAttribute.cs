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
	/// Validates that the content has not been set to the specified value
	/// </summary>
	public class ValidateNotSameValueAttribute : AbstractValidationAttribute
	{
		private readonly object value;

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidateSameAsAttribute"/> class.
		/// </summary>
		public ValidateNotSameValueAttribute(object mustNotBeThisValue)
		{
			value = mustNotBeThisValue;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidateNotSameValueAttribute"/> class.
		/// </summary>
		/// <param name="valueType">Type of the value.</param>
		/// <param name="mustNotBeThisValue">The must not be this value.</param>
		public ValidateNotSameValueAttribute(Type valueType, object mustNotBeThisValue)
			: this(valueType, mustNotBeThisValue, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidateSameAsAttribute"/> class.
		/// </summary>
		public ValidateNotSameValueAttribute(Type valueType, object mustNotBeThisValue, string errorMessage)
			: base(errorMessage)
		{
			if (mustNotBeThisValue == null)
			{
				throw new ArgumentException(
					"You cannot use ValidateNotSameValue validator for null values, use ValidateNotEmpty for this");
			}
			if (typeof(IConvertible).IsAssignableFrom(valueType))
				value = Convert.ChangeType(mustNotBeThisValue, valueType);
			else
			{
				MethodBase createInstance = valueType.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static);
				if (createInstance != null)
				{
					value = createInstance.Invoke(null, new object[] {mustNotBeThisValue});
				}
				ConstructorInfo ctor = valueType.GetConstructor(new Type[] {mustNotBeThisValue.GetType()});
				if (ctor == null)
				{
					throw new ArgumentException(
						"valueType must be a type that implements IConvertible, or have a Parse method, or have a constructor that accept a " +
						mustNotBeThisValue.GetType()
						);
				}
				value = Activator.CreateInstance(valueType, mustNotBeThisValue);
			}
		}

		/// <summary>
		/// Constructs and configures an <see cref="IValidator"/>
		/// instance based on the properties set on the attribute instance.
		/// </summary>
		/// <returns></returns>
		public override IValidator Build()
		{
			IValidator validator = new NotSameValueValidator(value);
			ConfigureValidatorMessage(validator);
			return validator;
		}
	}
}