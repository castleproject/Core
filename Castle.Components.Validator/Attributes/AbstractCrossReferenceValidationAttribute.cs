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
	/// The base class for all the validation attributes that reference a property or field.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple=true), Serializable]
	public abstract class AbstractCrossReferenceValidationAttributre : AbstractValidationAttribute
	{
		private Accessor referenceAccessor;
		private readonly string propertyToCompare;

		/// <summary>
		/// Initializes a new instance of the <see cref="AbstractCrossReferenceValidationAttributre"/> class.
		/// </summary>
		/// <param name="propertyToCompare">Target property to compare</param>
		protected AbstractCrossReferenceValidationAttributre(string propertyToCompare)
		{
			this.propertyToCompare = propertyToCompare;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AbstractCrossReferenceValidationAttributre"/> class.
		/// </summary>
		/// <param name="propertyToCompare">Target property to compare</param>
		/// <param name="errorMessage">The error message.</param>
		protected AbstractCrossReferenceValidationAttributre(string propertyToCompare, string errorMessage)
			: base(errorMessage)
		{
			this.propertyToCompare = propertyToCompare;
		}

		/// <summary>
		/// Gets the property to compare.
		/// </summary>
		public string PropertyToCompare
		{
			get { return propertyToCompare; }
		}

		/// <summary>
		/// Gets the reference accessor.
		/// </summary>
		protected Accessor ReferenceAccessor
		{
			get { return referenceAccessor; }
		}

		/// <summary>
		/// Implementors should perform any initialization logic
		/// </summary>
		/// <param name="validationRegistry">The validation registry.</param>
		/// <param name="property">The target property</param>
		public override void Initialize(IValidatorRegistry validationRegistry, PropertyInfo property)
		{
			base.Initialize(validationRegistry, property);

			referenceAccessor = validationRegistry.GetFieldOrPropertyAccessor(property.DeclaringType, propertyToCompare);
		}

		/// <summary>
		/// Constructs and configures an <see cref="IValidator"/>
		/// instance based on the properties set on the attribute instance.
		/// </summary>
		public override IValidator Build(IValidatorRunner validatorRunner, Type type)
		{
			IValidator validator = base.Build(validatorRunner, type);

			if (validator is IReferenceAccessAware)
			{
				((IReferenceAccessAware)validator).ReferenceAccessor = referenceAccessor;
			}

			return validator;
		}
	}
}
