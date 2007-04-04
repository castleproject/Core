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

	/// <summary>
	/// Coordinates the gathering and execution of validators.
	/// <seealso cref="IValidatorRegistry"/>
	/// </summary>
	/// <example>
	/// ValidatorRunner runner = new ValidatorRunner(new CachedValidationRegistry());
	/// 
	/// if (!runner.IsValid(customer))
	/// {
	///		// do something as the Customer instance is not valid
	/// }
	/// </example>
	public class ValidatorRunner
	{
		private readonly static IDictionary type2Validator;
	    private readonly IDictionary extendedProperties = new Hashtable();
		private readonly bool inferValidators;
		private readonly IDictionary errorPerInstance;
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
			type2Validator[typeof(DateTime)] = typeof(DateTimeValidator);
		}

	    public IDictionary ExtendedProperties
	    {
	        get { return extendedProperties; }
	    }

	    /// <summary>
		/// Initializes a new instance of the <see cref="ValidatorRunner"/> class.
		/// </summary>
		/// <param name="registry">The instance registry.</param>
		public ValidatorRunner(IValidatorRegistry registry) : this(true, registry)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidatorRunner"/> class.
		/// </summary>
		/// <param name="inferValidators">if <c>true</c>, the runner will try 
		/// to infer un-declared validators based on property types</param>
		/// <param name="registry">The registry.</param>
		public ValidatorRunner(bool inferValidators, IValidatorRegistry registry)
		{
			if (registry == null) throw new ArgumentNullException("registry");

			errorPerInstance = new Hashtable();

			this.inferValidators = inferValidators;
			this.registry = registry;
		}

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
		public bool IsValid(object objectInstance)
		{
			return IsValid(objectInstance, RunWhen.Everytime);
		}

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
		public bool IsValid(object objectInstance, RunWhen runWhen)
		{
			if (objectInstance == null) throw new ArgumentNullException("objectInstance");

			bool isValid = true;

			ErrorSummary summary = new ErrorSummary();

			foreach(IValidator validator in GetValidators(objectInstance, runWhen))
			{
				if (!validator.IsValid(objectInstance))
				{
					summary.RegisterErrorMessage(validator.Name, validator.ErrorMessage);

					isValid = false;
				}
			}

			errorPerInstance[objectInstance] = summary;

			return isValid;
		}

		/// <summary>
		/// Gets the registered validators.
		/// </summary>
		/// <param name="parentType">Type of the parent.</param>
		/// <param name="property">The property.</param>
		/// <returns></returns>
		public IValidator[] GetValidators(Type parentType, PropertyInfo property)
		{
			return GetValidators(parentType, property, RunWhen.Everytime);
		}

		/// <summary>
		/// Gets the registered validators.
		/// </summary>
		/// <param name="parentType">Type of the parent.</param>
		/// <param name="property">The property.</param>
		/// <param name="runWhenPhase">The run when phase.</param>
		/// <returns></returns>
		public IValidator[] GetValidators(Type parentType, PropertyInfo property, RunWhen runWhenPhase)
		{
			if (parentType == null) throw new ArgumentNullException("parentType");
			if (property == null) throw new ArgumentNullException("property");

			IValidator[] validators = registry.GetValidators(this, parentType, property, runWhenPhase);

			if (inferValidators && validators.Length == 0)
			{
				Type defaultValidatorForType = (Type) type2Validator[property.PropertyType];

				if (defaultValidatorForType != null)
				{
					validators = new IValidator[] { (IValidator) Activator.CreateInstance(defaultValidatorForType) };

					validators[0].Initialize(property);
				}
			}

			Array.Sort(validators, ValidatorComparer.Instance);

			return validators;
		}

		/// <summary>
		/// Gets the error list per instance.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <returns></returns>
		public bool HasErrors(object instance)
		{
			ErrorSummary summary = (ErrorSummary) errorPerInstance[instance];

			if (summary == null) return false;

			return summary.ErrorsCount != 0;
		}

		/// <summary>
		/// Gets the error list per instance.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <returns></returns>
		public ErrorSummary GetErrorSummary(object instance)
		{
			return (ErrorSummary) errorPerInstance[instance];
		}

		private IValidator[] GetValidators(object objectInstance, RunWhen runWhen)
		{
			if (objectInstance == null) throw new ArgumentNullException("objectInstance");

			IValidator[] validators = registry.GetValidators(this, objectInstance.GetType(), runWhen);

			Array.Sort(validators, ValidatorComparer.Instance);

			return validators;
		}

		class ValidatorComparer : IComparer
		{
			private readonly static ValidatorComparer instance = new ValidatorComparer();

			public int Compare(object x, object y)
			{
				IValidator left = (IValidator) x;
				IValidator right = (IValidator) y;

				return left.ExecutionOrder - right.ExecutionOrder;
			}

			public static ValidatorComparer Instance
			{
				get { return instance; }
			}
		}
	}
}
