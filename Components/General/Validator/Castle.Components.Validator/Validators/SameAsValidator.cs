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
	using System.Collections;
	using System.Reflection;

	public class SameAsValidator : AbstractValidator
	{
		private readonly string propertyToCompare;

		public SameAsValidator(string propertyToCompare)
		{
			this.propertyToCompare = propertyToCompare;
		}

		public override bool SupportWebValidation
		{
			get { return true; }
		}

		public override void ApplyWebValidation(WebValidationConfiguration config, InputElementType inputType,
		                                        IWebValidationGenerator generator, IDictionary attributes)
		{
			// TODO: Add same as to IWebValidationGenerator
		}

		public override bool IsValid(object instance, object fieldValue)
		{
			PropertyInfo property = instance.GetType().GetProperty(propertyToCompare);

			if (property == null)
			{
				throw new ValidationInternalError("Could not find property " + propertyToCompare + " on type " + instance.GetType());
			}

			object referenceValue = property.GetValue(instance, null);

			if (fieldValue == null && referenceValue == null)
			{
				return true;
			}
			else if (fieldValue != null)
			{
				return fieldValue.Equals(referenceValue);
			}
			else
			{
				return referenceValue.Equals(fieldValue);
			}
		}

		protected override string MessageKey
		{
			get { return MessageConstants.SameAsMessage; }
		}
	}
}
