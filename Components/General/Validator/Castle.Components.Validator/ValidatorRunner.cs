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
	using System.Collections.Specialized;
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
		private readonly IDictionary extendedProperties = new Hashtable();
		private readonly IDictionary errorPerInstance;
		private readonly IValidatorRegistry registry;

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidatorRunner"/> class.
		/// </summary>
		/// <param name="registry">The registry.</param>
		public ValidatorRunner(IValidatorRegistry registry)
		{
			if (registry == null) throw new ArgumentNullException("registry");

			errorPerInstance = new HybridDictionary();

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

		/// <summary>
		/// Gets the extended properties, which allows <see cref="IValidator"/> 
		/// implementation to store additional information to track state.
		/// </summary>
		/// <value>The extended properties.</value>
		public IDictionary ExtendedProperties
		{
			get { return extendedProperties; }
		}

		private IValidator[] GetValidators(object objectInstance, RunWhen runWhen)
		{
			if (objectInstance == null) throw new ArgumentNullException("objectInstance");

			IValidator[] validators = registry.GetValidators(this, objectInstance.GetType(), runWhen);

			Array.Sort(validators, ValidatorComparer.Instance);

			return validators;
		}

		private class ValidatorComparer : IComparer
		{
			private static readonly ValidatorComparer instance = new ValidatorComparer();

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