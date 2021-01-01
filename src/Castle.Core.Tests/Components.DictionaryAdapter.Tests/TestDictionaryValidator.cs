// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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

namespace Castle.Components.DictionaryAdapter.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Castle.Core.Internal;

	public interface IValidationRule
	{
		void Apply(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property, 
				   object propertyValue, IList<string> errors);
	}

	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Property, AllowMultiple = true)]
	public abstract class ValidationRuleAttribute : Attribute, IValidationRule
	{
		public abstract void Apply(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property,
								   object propertyValue, IList<string> errors);
	}

	public class TestDictionaryValidator : DictionaryBehaviorAttribute, IDictionaryValidator, IDictionaryInitializer
	{
		void IDictionaryInitializer.Initialize(IDictionaryAdapter dictionaryAdapter, object[] behaviors)
		{
			dictionaryAdapter.AddValidator(this);
		}

		public bool IsValid(IDictionaryAdapter dictionaryAdapter)
		{
			return string.IsNullOrEmpty(Validate(dictionaryAdapter));
		}

		public string Validate(IDictionaryAdapter dictionaryAdapter)
		{
			List<string> errors = new List<string>();
			var globalRules = AttributesUtil.GetTypeAttributes<ValidationRuleAttribute>(dictionaryAdapter.Meta.Type);

			foreach (var property in dictionaryAdapter.This.Properties.Values)
			{
				var propertyRules = AttributesUtil.GetAttributes<ValidationRuleAttribute>(property.Property).Select(x => (IValidationRule)x);
				var propertyValue = dictionaryAdapter.GetProperty(property.PropertyName, true);
				ApplyValidationRules(dictionaryAdapter, propertyRules, property, propertyValue, errors);
				ApplyValidationRules(dictionaryAdapter, globalRules, property, propertyValue, errors);
			}

			return string.Join(Environment.NewLine, errors.ToArray());
		}

		public string Validate(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property)
		{
			List<string> errors = new List<string>();
			var globalRules = AttributesUtil.GetTypeAttributes<ValidationRuleAttribute>(dictionaryAdapter.Meta.Type);

			var propertyRules = AttributesUtil.GetAttributes<ValidationRuleAttribute>(property.Property).Select(x => (IValidationRule)x);
			var propertyValue = dictionaryAdapter.GetProperty(property.PropertyName, true);
			ApplyValidationRules(dictionaryAdapter, propertyRules, property, propertyValue, errors);
			ApplyValidationRules(dictionaryAdapter, globalRules, property, propertyValue, errors);

			return string.Join(Environment.NewLine, errors.ToArray());
		}

		public void Invalidate(IDictionaryAdapter dictionaryAdapter)
		{
		}

		private void ApplyValidationRules(IDictionaryAdapter dictionaryAdapter, IEnumerable<IValidationRule> rules,
										  PropertyDescriptor property, object propertyValue, IList<string> errors)
		{
			if (rules != null)
			{
				foreach (var rule in rules)
				{
					rule.Apply(dictionaryAdapter, property, propertyValue, errors);
				}
			}
		}
	}

	public class ValidateStringLengthAtLeast : ValidationRuleAttribute
	{
		public ValidateStringLengthAtLeast(int minLength)
		{
			MinLength = minLength;
		}

		public int MinLength { get; private set; }

		public override void Apply(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property,
								   object propertyValue, IList<string> errors)
		{
			if (propertyValue == null || (propertyValue is string && ((string)propertyValue).Length < 10))
			{
				errors.Add(string.Format("Property {0} must be at least {1} characters long",
						   property.PropertyName, MinLength));
			}
		}
	}
}
