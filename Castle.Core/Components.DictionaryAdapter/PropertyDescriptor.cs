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
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Reflection;
	using System.Diagnostics;

	using Castle.Core.Internal;

	/// <summary>
	/// Describes a dictionary property.
	/// </summary>
	[DebuggerDisplay("{Property.DeclaringType.FullName,nq}.{PropertyName,nq}")]
	public class PropertyDescriptor : IDictionaryKeyBuilder, IDictionaryPropertyGetter, IDictionaryPropertySetter
	{
		private IDictionary state;
		private IDictionary extendedProperties;
		private List<IDictionaryPropertyGetter> getters;
		private List<IDictionaryPropertySetter> setters;
		private List<IDictionaryKeyBuilder> keyBuilders;

		private static readonly object[] NoBehaviors = new object[0];
		private static readonly ICollection<IDictionaryKeyBuilder> NoKeysBuilders = new IDictionaryKeyBuilder[0];
		private static readonly ICollection<IDictionaryPropertyGetter> NoHGetters = new IDictionaryPropertyGetter[0];
		private static readonly ICollection<IDictionaryPropertySetter> NoSetters = new IDictionaryPropertySetter[0];

		/// <summary>
		/// Initializes an empty <see cref="PropertyDescriptor"/> class.
		/// </summary>
		public PropertyDescriptor()
		{
			Behaviors = NoBehaviors;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyDescriptor"/> class.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <param name="behaviors">The property behaviors.</param>
		public PropertyDescriptor(PropertyInfo property, object[] behaviors) : this()
		{
			Property = property;
			Behaviors = behaviors ?? NoBehaviors;
			IsDynamicProperty = typeof(IDynamicValue).IsAssignableFrom(property.PropertyType);
			ObtainTypeConverter();
		}

		/// <summary>
		/// Initializes a new instance <see cref="PropertyDescriptor"/> class.
		/// </summary>
		protected PropertyDescriptor(object[] behaviors)
		{
			Behaviors = behaviors ?? NoBehaviors;
		}

		/// <summary>
		///  Copies an existinginstance of the <see cref="PropertyDescriptor"/> class.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="copyBehaviors"></param>
		public PropertyDescriptor(PropertyDescriptor source, bool copyBehaviors)
		{
			Property = source.Property;
			Behaviors = source.Behaviors;
			IsDynamicProperty = source.IsDynamicProperty;
			TypeConverter = source.TypeConverter;
			SuppressNotifications = source.SuppressNotifications;
			state = source.state;
			IfExists = source.IfExists;
			Fetch = source.Fetch;

			if (copyBehaviors)
			{
				keyBuilders = source.keyBuilders;
				getters = source.getters;
				setters = source.setters;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public int ExecutionOrder
		{
			get { return 0; }
		}

		/// <summary>
		/// Gets the property name.
		/// </summary>
		public string PropertyName
		{
			get { return (Property != null) ? Property.Name : null; }
		}

		/// <summary>
		/// Gets the property type.
		/// </summary>
		public Type PropertyType
		{
			get { return (Property != null) ? Property.PropertyType : null; }
		}

		/// <summary>
		/// Gets the property.
		/// </summary>
		/// <value>The property.</value>
		public PropertyInfo Property { get; private set; }

		/// <summary>
		/// Returns true if the property is dynamic.
		/// </summary>
		public bool IsDynamicProperty { get; private set; }

		/// <summary>
		/// Gets additional state.
		/// </summary>
		public IDictionary State
		{
			get
			{
				if (state == null)
				{
					state = new Dictionary<object, object>();
				}
				return state;
			}
		}

		/// <summary>
		/// Determines if property should be fetched.
		/// </summary>
		public bool Fetch { get; set; }

		/// <summary>
		/// Determines if property must exist first.
		/// </summary>
		public bool IfExists { get; set; }

		/// <summary>
		/// Determines if notifications should occur.
		/// </summary>
		public bool SuppressNotifications { get; set; }

		/// <summary>
		/// Gets the property behaviors.
		/// </summary>
		public object[] Behaviors { get; private set; }

		/// <summary>
		/// Gets the type converter.
		/// </summary>
		/// <value>The type converter.</value>
		public TypeConverter TypeConverter { get; private set; }

		/// <summary>
		/// Gets the extended properties.
		/// </summary>
		public IDictionary ExtendedProperties
		{
			get
			{
				if (extendedProperties == null)
				{
					extendedProperties = new Dictionary<object, object>();
				}
				return extendedProperties;
			}
		}

		/// <summary>
		/// Gets the key builders.
		/// </summary>
		/// <value>The key builders.</value>
		public ICollection<IDictionaryKeyBuilder> KeyBuilders
		{
			get { return keyBuilders ?? NoKeysBuilders; }
		}

		/// <summary>
		/// Gets the setter.
		/// </summary>
		/// <value>The setter.</value>
		public ICollection<IDictionaryPropertySetter> Setters
		{
			get { return setters ?? NoSetters; }
		}

		/// <summary>
		/// Gets the getter.
		/// </summary>
		/// <value>The getter.</value>
		public ICollection<IDictionaryPropertyGetter> Getters
		{
			get { return getters ?? NoHGetters; }
		}

		#region IDictionaryKeyBuilder Members

		/// <summary>
		/// Gets the key.
		/// </summary>
		/// <param name="dictionaryAdapter">The dictionary adapter.</param>
		/// <param name="key">The key.</param>
		/// <param name="descriptor">The descriptor.</param>
		/// <returns></returns>
		public string GetKey(IDictionaryAdapter dictionaryAdapter, String key, PropertyDescriptor descriptor)
		{
			if (keyBuilders != null)
			{
				foreach (var builder in keyBuilders)
				{
					key = builder.GetKey(dictionaryAdapter, key, this);
				}
			}

			return key;
		}

		/// <summary>
		/// Adds the key builder.
		/// </summary>
		/// <param name="builders">The builder.</param>
		public PropertyDescriptor AddKeyBuilder(params IDictionaryKeyBuilder[] builders)
		{
			return AddKeyBuilders((IEnumerable<IDictionaryKeyBuilder>)builders);
		}

		/// <summary>
		/// Adds the key builders.
		/// </summary>
		/// <param name="builders">The builders.</param>
		public PropertyDescriptor AddKeyBuilders(IEnumerable<IDictionaryKeyBuilder> builders)
		{
			if (builders != null)
			{
				if (keyBuilders == null)
				{
					keyBuilders = new List<IDictionaryKeyBuilder>(builders);
				}
				else
				{
					keyBuilders.AddRange(builders);
				}
			}
			return this;
		}

		/// <summary>
		/// Copies the key builders to the other <see cref="PropertyDescriptor"/>
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public PropertyDescriptor CopyKeyBuilders(PropertyDescriptor other)
		{
			if (keyBuilders != null)
			{
				other.AddKeyBuilders(keyBuilders.Select(builder => builder.Copy()).OfType<IDictionaryKeyBuilder>());
			}
			return this;
		}

		#endregion

		#region IDictionaryPropertyGetter Members

		/// <summary>
		/// Gets the property value.
		/// </summary>
		/// <param name="dictionaryAdapter">The dictionary adapter.</param>
		/// <param name="key">The key.</param>
		/// <param name="storedValue">The stored value.</param>
		/// <param name="descriptor">The descriptor.</param>
		/// <param name="ifExists">true if return only existing.</param>
		/// <returns></returns>
		public object GetPropertyValue(IDictionaryAdapter dictionaryAdapter,
			string key, object storedValue, PropertyDescriptor descriptor, bool ifExists)
		{
			key = GetKey(dictionaryAdapter, key, descriptor);
			storedValue = storedValue ?? dictionaryAdapter.ReadProperty(key);

			if (getters != null)
			{
				foreach (var getter in getters)
				{
					storedValue = getter.GetPropertyValue(dictionaryAdapter, key, storedValue, this, IfExists || ifExists);
				}
			}

			return storedValue;
		}

		/// <summary>
		/// Adds the dictionary getter.
		/// </summary>
		/// <param name="getters">The getter.</param>
		public PropertyDescriptor AddGetter(params IDictionaryPropertyGetter[] getters)
		{
			return AddGetters((IEnumerable<IDictionaryPropertyGetter>)getters);
		}

		/// <summary>
		/// Adds the dictionary getters.
		/// </summary>
		/// <param name="gets">The getters.</param>
		public PropertyDescriptor AddGetters(IEnumerable<IDictionaryPropertyGetter> gets)
		{
			if (gets != null)
			{
				if (getters == null)
				{
					getters = new List<IDictionaryPropertyGetter>(gets);
				}
				else
				{
					getters.AddRange(gets);
				}
			}
			return this;
		}

		/// <summary>
		/// Copies the property getters to the other <see cref="PropertyDescriptor"/>
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public PropertyDescriptor CopyGetters(PropertyDescriptor other)
		{
			if (getters != null)
			{
				other.AddGetters(getters.Select(getter => getter.Copy()).OfType<IDictionaryPropertyGetter>());
			}
			return this;
		}

		#endregion

		#region IDictionaryPropertySetter Members

		/// <summary>
		/// Sets the property value.
		/// </summary>
		/// <param name="dictionaryAdapter">The dictionary adapter.</param>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <param name="descriptor">The descriptor.</param>
		/// <returns></returns>
		public bool SetPropertyValue(IDictionaryAdapter dictionaryAdapter,
			string key, ref object value, PropertyDescriptor descriptor)
		{
			key = GetKey(dictionaryAdapter, key, descriptor);

			if (setters != null)
			{
				foreach(var setter in setters)
				{
					if (setter.SetPropertyValue(dictionaryAdapter, key, ref value, this) == false)
					{
						return false;
					}
				}
			}

			dictionaryAdapter.StoreProperty(this, key, value);
			return true;
		}

		/// <summary>
		/// Adds the dictionary setter.
		/// </summary>
		/// <param name="setters">The setter.</param>
		public PropertyDescriptor AddSetter(params IDictionaryPropertySetter[] setters)
		{
			return AddSetters((IEnumerable<IDictionaryPropertySetter>)setters);
		}

		/// <summary>
		/// Adds the dictionary setters.
		/// </summary>
		/// <param name="sets">The setters.</param>
		public PropertyDescriptor AddSetters(IEnumerable<IDictionaryPropertySetter> sets)
		{
			if (sets != null)
			{
				if (setters == null)
				{
					setters = new List<IDictionaryPropertySetter>(sets);
				}
				else
				{
					setters.AddRange(sets);
				}
			}
			return this;
		}

		/// <summary>
		/// Copies the property setters to the other <see cref="PropertyDescriptor"/>
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public PropertyDescriptor CopySetters(PropertyDescriptor other)
		{
			if (setters != null)
			{
				other.AddSetters(setters.Select(setter => setter.Copy()).OfType<IDictionaryPropertySetter>());
			}
			return this;
		}

		#endregion

		/// <summary>
		/// Adds the behaviors.
		/// </summary>
		/// <param name="behaviors"></param>
		/// <returns></returns>
		public PropertyDescriptor AddBehavior(params IDictionaryBehavior[] behaviors)
		{
			return AddBehaviors((IEnumerable<IDictionaryBehavior>)behaviors);
		}

		/// <summary>
		/// Adds the behaviors.
		/// </summary>
		/// <param name="behaviors"></param>
		/// <returns></returns>
		public PropertyDescriptor AddBehaviors(IEnumerable<IDictionaryBehavior> behaviors)
		{
			if (behaviors != null)
			{
				foreach (var behavior in behaviors)
				{
					InternalAddBehavior(behavior);
				}
			}
			return this;
		}

			/// <summary>
		/// Adds the behaviors from the builders.
		/// </summary>
		/// <param name="builders"></param>
		/// <returns></returns>
		public PropertyDescriptor AddBehaviors(params IDictionaryBehaviorBuilder[] builders)
		{
			AddBehaviors(builders.SelectMany(builder => builder.BuildBehaviors().OfType<IDictionaryBehavior>()));
			return this;
		}

		/// <summary>
		/// Copies the behaviors to the other <see cref="PropertyDescriptor"/>
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public virtual PropertyDescriptor CopyBehaviors(PropertyDescriptor other)
		{
			return CopyKeyBuilders(other).CopyGetters(other).CopySetters(other);
		}

		/// <summary>
		/// Copies the <see cref="PropertyDescriptor"/>
		/// </summary>
		/// <returns></returns>
		public IDictionaryBehavior Copy()
		{
			return new PropertyDescriptor(this, true);
		}

		protected virtual void InternalAddBehavior(IDictionaryBehavior behavior)
		{
			if (behavior is IDictionaryKeyBuilder)
			{
				AddKeyBuilder((IDictionaryKeyBuilder)behavior);
			}

			if (behavior is IDictionaryPropertyGetter)
			{
				AddGetter((IDictionaryPropertyGetter)behavior);
			}

			if (behavior is IDictionaryPropertySetter)
			{
				AddSetter((IDictionaryPropertySetter)behavior);
			}
		}

		private void ObtainTypeConverter()
		{
			var converterType = AttributesUtil.GetTypeConverter(Property);

			if (converterType != null)
			{
				TypeConverter = (TypeConverter) Activator.CreateInstance(converterType);
			}
			else
			{
				TypeConverter = TypeDescriptor.GetConverter(PropertyType);
			}
		}
	}
}