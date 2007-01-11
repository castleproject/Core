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

	public class ValidatorRunner
	{
		private static IDictionary type2Validator;

		private readonly bool inferValidators;
		private readonly IValidatorRegistry registry;

		static ValidatorRunner()
		{
			type2Validator = new Hashtable();
			type2Validator[typeof(Int16)] = typeof(IntegerValidator);
			type2Validator[typeof(Int32)] = typeof(IntegerValidator);
			type2Validator[typeof(Int64)] = typeof(IntegerValidator);
			type2Validator[typeof(decimal)] = typeof(DecimalValidator);
			type2Validator[typeof(Single)] = typeof(SingleValidator);
			type2Validator[typeof(double)] = typeof(DoubleValidator);
			type2Validator[typeof(DateTime)] = typeof(DateValidator);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidatorRunner"/> class.
		/// </summary>
		/// <param name="inferValidators">if <c>true</c>, the runner will try 
		/// to infer un-declared validators based on property types</param>
		/// <param name="registry">The registry.</param>
		public ValidatorRunner(bool inferValidators, IValidatorRegistry registry)
		{
			this.inferValidators = inferValidators;
			this.registry = registry;
		}

		/// <summary>
		/// Determines whether the specified instance is valid.
		/// </summary>
		/// <param name="objectInstance">The obj.</param>
		/// <returns>
		/// 	<see langword="true"/> if the specified obj is valid; otherwise, <see langword="false"/>.
		/// </returns>
		public bool IsValid(object objectInstance)
		{
			if (objectInstance == null) throw new ArgumentNullException("objectInstance");

			bool isValid = true;

			foreach(IValidator validator in GetValidators(objectInstance))
			{
				if (!validator.IsValid(objectInstance))
				{
					isValid = false;
				}
			}

			return isValid;
		}

		public IValidator[] GetValidators(Type parentType, PropertyInfo property)
		{
			if (parentType == null) throw new ArgumentNullException("parentType");
			if (property == null) throw new ArgumentNullException("property");

			IValidator[] validators = registry.GetValidators(parentType, property);

			if (inferValidators && validators.Length == 0)
			{
				Type defaultValidatorForType = (Type) type2Validator[property.PropertyType];

				if (defaultValidatorForType != null)
				{
					validators = new IValidator[] { (IValidator) Activator.CreateInstance(defaultValidatorForType) };

					validators[0].Initialize(property);
				}
			}

			return validators;
		}

		private IValidator[] GetValidators(object objectInstance)
		{
			if (objectInstance == null) throw new ArgumentNullException("objectInstance");

			return registry.GetValidators(objectInstance.GetType());
		}
	}
}
