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

	/// <summary>
	/// <see cref="IValidatorRegistry"/> implementation that
	/// caches the reflection and custom attributes calls for better performance.
	/// </summary>
	public class CachedValidationRegistry : IValidatorRegistry
	{
		private static ResourceManager defaultResourceManager;
		private ResourceManager resourceManager;

		private readonly IDictionary propertiesPerType = Hashtable.Synchronized(new Hashtable());
		private readonly IDictionary attrsPerProperty = Hashtable.Synchronized(new Hashtable());

		/// <summary>
		/// Initializes the <see cref="CachedValidationRegistry"/> class.
		/// </summary>
		static CachedValidationRegistry()
		{
			defaultResourceManager =
				new ResourceManager("Castle.Components.Validator.Messages",
				                    typeof(CachedValidationRegistry).Assembly);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CachedValidationRegistry"/> class.
		/// </summary>
		public CachedValidationRegistry()
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CachedValidationRegistry"/> class.
		/// </summary>
		/// <param name="resourceManager">The resource manager.</param>
		public CachedValidationRegistry(ResourceManager resourceManager)
		{
			this.resourceManager = resourceManager;
		}

		#region IValidatorRegistry Members

		/// <summary>
		/// Gets all validators associated with a <see cref="Type"/>.
		/// <para>
		/// The validators returned are initialized.
		/// </para>
		/// </summary>
		/// <param name="validatorRunner">The validator runner.</param>
		/// <param name="targetType">Target type.</param>
		/// <param name="runWhen">Restrict the set returned to the phase specified</param>
		/// <returns>A Validator array</returns>
		public IValidator[] GetValidators(ValidatorRunner validatorRunner, Type targetType, RunWhen runWhen)
		{
			PropertyInfo[] properties = (PropertyInfo[]) propertiesPerType[targetType];

			if (properties == null)
			{
				properties = targetType.GetProperties();
				propertiesPerType[targetType] = properties;
			}

			ArrayList list = new ArrayList();

			foreach (PropertyInfo prop in properties)
			{
				list.AddRange(GetValidators(validatorRunner, targetType, prop, runWhen));
			}

			return (IValidator[]) list.ToArray(typeof (IValidator));
		}

		/// <summary>
		/// Gets all validators associated with a property.
		/// <para>
		/// The validators returned are initialized.
		/// </para>
		/// </summary>
		/// <param name="validatorRunner">The validator runner.</param>
		/// <param name="targetType">Target type.</param>
		/// <param name="property">The property.</param>
		/// <param name="runWhen">Restrict the set returned to the phase specified</param>
		/// <returns>A Validator array</returns>
		public IValidator[] GetValidators(
			ValidatorRunner validatorRunner, Type targetType, PropertyInfo property,
			RunWhen runWhen)
		{
			object[] builders = (object[]) attrsPerProperty[property];

			if (builders == null)
			{
				builders = property.GetCustomAttributes(typeof (IValidatorBuilder), true);
				attrsPerProperty[property] = builders;
			}

			ArrayList validators = new ArrayList();

			foreach (IValidatorBuilder builder in builders)
			{
				IValidator validator = builder.Build(validatorRunner, targetType);

				if (!IsValidatorOnPhase(validator, runWhen)) continue;

				validator.Initialize(this, property);
				validators.Add(validator);
			}

			return (IValidator[]) validators.ToArray(typeof (IValidator));
		}

		/// <summary>
		/// Gets the string from resource by key
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public string GetStringFromResource(string key)
		{
			if(resourceManager!=null)
			{
				ResourceSet resourceSet = resourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true);
				string result = resourceSet.GetString(key);
				if (result != null)
					return result;
			}
			ResourceSet defaultResourceSetForCurrentThread = defaultResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true);
			return defaultResourceSetForCurrentThread.GetString(key);
		}

		#endregion

		private static bool IsValidatorOnPhase(IValidator validator, RunWhen when)
		{
			if (validator.RunWhen == RunWhen.Everytime) return true;

			return ((validator.RunWhen & when) != 0);
		}
	}
}