// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.Components.DictionaryAdapter
{
	using System;
	using System.Linq;
	using System.Collections;
	using System.Reflection;

	/// <summary>
	/// Removes a property if matches value.
	/// </summary>
	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Property, AllowMultiple = true)]
	public class RemoveIfAttribute : DictionaryBehaviorAttribute, IDictionaryPropertySetter
	{
		private ICondition condition;

		public RemoveIfAttribute()
		{
			ExecutionOrder += 10;
		}

		public RemoveIfAttribute(params object[] values) : this()
		{
			values = values ?? new object[] { null };
			condition = new ValueCondition(values, null);
		}

		public RemoveIfAttribute(object[] values, Type comparerType) : this()
		{
			var comparer = Construct<IEqualityComparer>(comparerType, "comparerType");
			condition = new ValueCondition(values, comparer);
		}

		protected RemoveIfAttribute(ICondition condition) : this()
		{
			this.condition = condition;
		}

		public Type Condition
		{
			set { condition = Construct<ICondition>(value, "value"); }
		}

		bool IDictionaryPropertySetter.SetPropertyValue(IDictionaryAdapter dictionaryAdapter,
			string key, ref object value, PropertyDescriptor property)
		{
			if (ShouldRemove(value))
			{
				dictionaryAdapter.ClearProperty(property, key);
				return false;
			}
			return true;
		}

		internal bool ShouldRemove(object value)
		{
			return condition != null && condition.SatisfiedBy(value);
		}

		private static TBase Construct<TBase>(Type type, string paramName)
			where TBase : class
		{
			if (type == null)
			{
				throw new ArgumentNullException(paramName);
			}

			if (type.GetTypeInfo().IsAbstract == false && typeof(TBase).IsAssignableFrom(type))
			{
				var constructor = type.GetConstructor(Type.EmptyTypes);
				if (constructor != null)
				{
					return (TBase)constructor.Invoke(new object[0]);
				}
			}

			throw new ArgumentException(string.Format(
				"{0} is not a concrete type implementing {1} with a default constructor",
				type.FullName, typeof(TBase).FullName));
		}

		#region Nested Class: ValueCondition

		class ValueCondition : ICondition
		{
			private readonly object[] values;
			private readonly IEqualityComparer comparer;

			public ValueCondition(object[] values, IEqualityComparer comparer)
			{
				this.values = values;
				this.comparer = comparer;
			}

			public bool SatisfiedBy(object value)
			{
				return values.Any(valueToMatch =>
				{
					if (comparer == null)
					{
						return Equals(value, valueToMatch);
					}
					return comparer.Equals(value, valueToMatch);
				});
			}
		}

		#endregion
	}
}