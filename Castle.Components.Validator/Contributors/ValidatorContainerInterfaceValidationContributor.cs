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

namespace Castle.Components.Validator.Contibutors
{
	using System;
	using System.Collections.Generic;
	using Castle.Components.Validator.Attributes;

	/// <summary>
	/// Will apply validation on members defined in derived interface IValidatorContainerInterface from the object instance
	/// </summary>
	public class ValidatorContainerInterfaceValidationContributor 
		: AbstractValidationContributor
		  , IHasValidationPerformerDependency
		  , IHasValidatorRegistryDependency
		  , IHasValidatorRunnerDependency
	{
		private static Dictionary<Type, IEnumerable<Type>> typeToValidatorContainerInterfacesMap = new Dictionary<Type, IEnumerable<Type>>();

		private IValidatorRegistry validatorRegistry;
		private IValidationPerformer validationPerformer;
		private IValidatorRunner validatorRunner;

		IValidationPerformer IHasValidationPerformerDependency.ValidationPerformer
		{
			set { validationPerformer = value; }
		}


		IValidatorRegistry IHasValidatorRegistryDependency.ValidatorRegistry
		{
			set { validatorRegistry = value; }
		}


		IValidatorRunner IHasValidatorRunnerDependency.ValidatorRunner {
			set { validatorRunner = value; }
		}


		/// <summary>
		/// Allows for custom initialization based on type.  This will only be called once
		/// for each type passed to the contributor.
		/// </summary>
		/// <param name="type">The type.</param>
		protected override void Initialize(Type type)
		{
			if(!typeToValidatorContainerInterfacesMap.ContainsKey(type))
			{
				IList<Type> cachedInterfaceTypes = new List<Type>();
				Type[] interfaceTypes = type.GetInterfaces();
				foreach(Type interfaceType in interfaceTypes)
				{
					if(IsInterfaceFlagged(interfaceType))
						cachedInterfaceTypes.Add(interfaceType);
				}
				typeToValidatorContainerInterfacesMap[type] = cachedInterfaceTypes;
			}
		}

		private bool IsInterfaceFlagged(Type interfaceType)
		{
			object[] attributes = interfaceType.GetCustomAttributes(typeof(ValidatorContainerInterfaceFlagAttribute), false);
			return (attributes.Length != 0);
		}


		/// <summary>
		/// Determines whether the specified instance is valid.  Returns an
		/// <see cref="ErrorSummary"/> that will be appended to the existing
		/// error summary for an object.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <param name="when">The when.</param>
		/// <returns></returns>
		protected override ErrorSummary IsValidInternal(object instance, RunWhen when)
		{
			ErrorSummary summary = new ErrorSummary();

			// perform validation on each validator container interfaces
			foreach (Type validatorContainerType in GetValidatorContainerInterfacesForType(instance.GetType()))
			{
				// don't run any other contributors
				IValidationContributor[] contributors = null;

				validationPerformer.PerformValidation(
					instance,
					RequestValidatorsToRegistry(validatorContainerType, when),
					contributors,
					when,
					summary
					);
			}
			return summary;
		}

		private IEnumerable<Type> GetValidatorContainerInterfacesForType(Type type)
		{
			return typeToValidatorContainerInterfacesMap[type];
		}

		private IValidator[] RequestValidatorsToRegistry(Type validatorContainerType, RunWhen when)
		{
			return validatorRegistry.GetValidators(
				validatorRunner, 
				validatorContainerType,
				when);
		}
	}
}
