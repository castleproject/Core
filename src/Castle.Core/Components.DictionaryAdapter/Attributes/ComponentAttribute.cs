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
	public class ComponentAttribute : DictionaryBehaviorAttribute, IDictionaryKeyBuilder,
									  IDictionaryPropertyGetter, IDictionaryPropertySetter
	{
		/// <summary>
		/// Applies no prefix.
		/// </summary>
		public bool NoPrefix
		{
			get { return Prefix == ""; }
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
		public string Prefix { get; set; }

		#region IDictionaryKeyBuilder Members

		string IDictionaryKeyBuilder.GetKey(IDictionaryAdapter dictionaryAdapter, string key,
		                                    PropertyDescriptor property)
		{
			return Prefix ?? key + "_";
		}

		#endregion

		#region IDictionaryPropertyGetter

		object IDictionaryPropertyGetter.GetPropertyValue(IDictionaryAdapter dictionaryAdapter,
			string key, object storedValue, PropertyDescriptor property, bool ifExists)
		{
			if (storedValue == null)
			{
				var component = dictionaryAdapter.This.ExtendedProperties[property.PropertyName];

				if (component == null)
				{
					var descriptor = new PropertyDescriptor(property.Property, null);
					descriptor.AddBehavior(new KeyPrefixAttribute(key));
					component = dictionaryAdapter.This.Factory.GetAdapter(
						property.Property.PropertyType, dictionaryAdapter.This.Dictionary, descriptor);
					dictionaryAdapter.This.ExtendedProperties[property.PropertyName] = component;
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
			dictionaryAdapter.This.ExtendedProperties.Remove(property.PropertyName);
			return false;
		}

		#endregion
	}
}