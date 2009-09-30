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

	/// <summary>
	/// Identifies a property should be represented as a nested component.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class DictionaryComponentAttribute : DictionaryBehaviorAttribute, 
		IDictionaryKeyBuilder, IDictionaryPropertyGetter, IDictionaryPropertySetter
	{
		private String prefix;

		/// <summary>
		/// Applies no prefix.
		/// </summary>
		public bool NoPrefix
		{
			get { return prefix == ""; }
			set
			{
				if (value)
				{
					Prefix = "";
				}
			}
		}

		/// <summary>
		/// Gets or sets the prefix.
		/// </summary>
		/// <value>The prefix.</value>
		public string Prefix
		{
			get { return prefix; }
			set { prefix = value; }
		}

		#region IDictionaryKeyBuilder Members

		string IDictionaryKeyBuilder.GetKey(IDictionaryAdapter dictionaryAdapter, string key,
		                                    PropertyDescriptor property)
		{
			return prefix ?? key + "_";
		}

		#endregion

		#region IDictionaryPropertyGetter

		object IDictionaryPropertyGetter.GetPropertyValue(IDictionaryAdapter dictionaryAdapter,
			string key, object storedValue, PropertyDescriptor property)
		{
			if (storedValue == null)
			{
				var component = dictionaryAdapter.State[key];

				if (component == null)
				{
					var descriptor = new PropertyDescriptor(property.Property);
					descriptor.AddKeyBuilder(new DictionaryKeyPrefixAttribute(key));
					component = dictionaryAdapter.Factory.GetAdapter(property.Property.PropertyType, 
						dictionaryAdapter.Dictionary, descriptor);
					dictionaryAdapter.State[key] = component;
				}

				return component;
			}

			return storedValue;
		}

		#endregion

		#region IDictionaryPropertySetter Members

		public bool SetPropertyValue(IDictionaryAdapter dictionaryAdapter,
			string key, ref object value, PropertyDescriptor property)
		{
			dictionaryAdapter.State.Remove(key);
			return true;
		}

		#endregion
	}
}