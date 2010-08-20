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

	/// <summary>
	/// Base class for allowing custom validation of an instance beyond
	/// the <see cref="IValidator">IValidator</see> instances registered for
	/// the object.
	/// </summary>
	public abstract class AbstractValidationContributor : IValidationContributor
	{
		private static readonly IDictionary initializedTypesByContributorType = Hashtable.Synchronized(new Hashtable());

		/// <summary>
		/// Determines whether the specified instance is valid.  Returns an
		/// <see cref="ErrorSummary"/> that will be appended to the existing
		/// error summary for an object.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <param name="runWhen">The run when.</param>
		/// <returns></returns>
		public ErrorSummary IsValid(object instance, RunWhen runWhen)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");
			
			Type contributorType = this.GetType();
			Type instanceType = instance.GetType();

			if(!initializedTypesByContributorType.Contains(contributorType))
			{
				// Double checked lock so we don't get two threads adding the same type at the same time
				lock (initializedTypesByContributorType.SyncRoot)
				{
					if (!initializedTypesByContributorType.Contains(contributorType))
					{
						initializedTypesByContributorType[contributorType] = ArrayList.Synchronized(new ArrayList());
					}
				}
			}
			
			ArrayList initialized = initializedTypesByContributorType[contributorType] as ArrayList;
			if (!initialized.Contains(instanceType))
			{
				lock (initialized.SyncRoot)
				{
					if (!initialized.Contains(instanceType))
					{
						Initialize(instanceType);
						initialized.Add(instanceType);
					}
				}
			}

			return IsValidInternal(instance, runWhen);
		}

		/// <summary>
		/// Allows for custom initialization based on type.  This will only be called once
		/// for each type passed to the contributor.
		/// </summary>
		/// <param name="type">The type.</param>
		protected abstract void Initialize(Type type);

		/// <summary>
		/// Determines whether the specified instance is valid.  Returns an
		/// <see cref="ErrorSummary"/> that will be appended to the existing
		/// error summary for an object.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <param name="when">The when.</param>
		/// <returns></returns>
		protected abstract ErrorSummary IsValidInternal(object instance, RunWhen when);
	}
}
