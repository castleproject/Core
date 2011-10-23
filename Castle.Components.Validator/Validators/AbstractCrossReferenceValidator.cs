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

	/// <summary>
	/// Abstract <see cref="IValidator"/> implementation
	/// </summary>
	[Serializable]
	public abstract class AbstractCrossReferenceValidator : AbstractValidator, IReferenceAccessAware
	{
		private Accessor referenceAccessor;
		private readonly string propertyToCompare;

		/// <summary>
		/// Initializes a new instance of the AbstractCrossReferenceValidator class.
		/// </summary>
		/// <param name="propertyToCompare">The property to compare.</param>
		public AbstractCrossReferenceValidator(string propertyToCompare)
		{
			this.propertyToCompare = propertyToCompare;
		}

		/// <summary>
		/// Target Property to compare
		/// </summary>
		public string PropertyToCompare
		{
			get { return propertyToCompare; }
		}

		/// <summary>
		/// Sets the reference accessor.
		/// </summary>
		public Accessor ReferenceAccessor
		{
			set { referenceAccessor = value; }
		}

		/// <summary>
		/// Obtains the value of the reference on a specific instance.
		/// </summary>
		/// <param name="instance">The instance to inspect.</param>
		/// <returns></returns>
		public object GetReferenceValue(object instance)
		{
			if (referenceAccessor != null)
			{
				return referenceAccessor(instance);
			}

			return AccessorUtil.GetPathValue(instance, propertyToCompare);
		}
	}
}