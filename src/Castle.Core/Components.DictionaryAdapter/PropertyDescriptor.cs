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
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Linq;
	using System.Reflection;
	using Castle.Core.Internal;

	/// <summary>
	/// Describes a dictionary property.
	/// </summary>
	[DebuggerDisplay("{Property.DeclaringType.FullName,nq}.{PropertyName,nq}")]
	public class PropertyDescriptor : IDictionaryKeyBuilder, IDictionaryPropertyGetter, IDictionaryPropertySetter
	{
		private IDictionary state;
		private IDictionary extendedProperties;
		protected SortedSet<IDictionaryBehavior> dictionaryBehaviors;

		private static readonly object[] NoBehaviors = new object[0];

		/// <summary>
		/// Initializes an empty <see cref="PropertyDescriptor"/> class.
		/// </summary>
		public PropertyDescriptor()
		{
			PropertyBehaviors = NoBehaviors;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyDescriptor"/> class.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <param name="behaviors">The property behaviors.</param>
		public PropertyDescriptor(PropertyInfo property, object[] behaviors) : this()
		{
			Property = property;
			PropertyBehaviors = behaviors ?? NoBehaviors;
			IsDynamicProperty = typeof(IDynamicValue).IsAssignableFrom(property.PropertyType);
			ObtainTypeConverter();
		}

		/// <summary>
		/// Initializes a new instance <see cref="PropertyDescriptor"/> class.
		/// </summary>
		public PropertyDescriptor(object[] behaviors)
		{
			PropertyBehaviors = behaviors ?? NoBehaviors;
		}

		/// <summary>
		///  Copies an existinginstance of the <see cref="PropertyDescriptor"/> class.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="copyBehaviors"></param>
		public PropertyDescriptor(PropertyDescriptor source, bool copyBehaviors)
		{
			Property = source.Property;
			PropertyBehaviors = source.PropertyBehaviors;
			IsDynamicProperty = source.IsDynamicProperty;
			TypeConverter = source.TypeConverter;
			SuppressNotifications = source.SuppressNotifications;
			state = source.state;
			IfExists = source.IfExists;
			Fetch = source.Fetch;

			if (copyBehaviors && source.dictionaryBehaviors != null)
				dictionaryBehaviors = new SortedSet<IDictionaryBehavior>(source.dictionaryBehaviors, DictionaryBehaviorComparer.Instance);
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
		public object[] PropertyBehaviors { get; private set; }

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
		public IEnumerable<IDictionaryKeyBuilder> KeyBuilders
		{
			get 
			{
				return (dictionaryBehaviors != null)
					? dictionaryBehaviors.OfType<IDictionaryKeyBuilder>() 
					: Enumerable.Empty<IDictionaryKeyBuilder>();
			}
		}

		/// <summary>
		/// Gets the setter.
		/// </summary>
		/// <value>The setter.</value>
		public IEnumerable<IDictionaryBehavior> Behaviors
		{
			get
			{
				return dictionaryBehaviors ?? Enumerable.Empty<IDictionaryBehavior>();
			}
		}

		/// <summary>
		/// Gets the setter.
		/// </summary>
		/// <value>The setter.</value>
		public IEnumerable<IDictionaryPropertySetter> Setters
		{
			get
			{
				return (dictionaryBehaviors != null)
					? dictionaryBehaviors.OfType<IDictionaryPropertySetter>()
					: Enumerable.Empty<IDictionaryPropertySetter>();
			}
		}

		/// <summary>
		/// Gets the getter.
		/// </summary>
		/// <value>The getter.</value>
		public IEnumerable<IDictionaryPropertyGetter> Getters
		{
			get
			{
				return (dictionaryBehaviors != null)
					? dictionaryBehaviors.OfType<IDictionaryPropertyGetter>()
					: Enumerable.Empty<IDictionaryPropertyGetter>();
			}
		}

		/// <summary>
		/// Gets the initializers.
		/// </summary>
		/// <value>The initializers.</value>
		public IEnumerable<IDictionaryInitializer> Initializers
		{
			get
			{
				return (dictionaryBehaviors != null)
					? dictionaryBehaviors.OfType<IDictionaryInitializer>()
					: Enumerable.Empty<IDictionaryInitializer>();
			}
		}

		/// <summary>
		/// Gets the meta-data initializers.
		/// </summary>
		/// <value>The meta-data initializers.</value>
		public IEnumerable<IDictionaryMetaInitializer> MetaInitializers
		{
			get
			{
				return (dictionaryBehaviors != null)
					? dictionaryBehaviors.OfType<IDictionaryMetaInitializer>()
					: Enumerable.Empty<IDictionaryMetaInitializer>();
			}
		}

		/// <summary>
		/// Gets the key.
		/// </summary>
		/// <param name="dictionaryAdapter">The dictionary adapter.</param>
		/// <param name="key">The key.</param>
		/// <param name="descriptor">The descriptor.</param>
		/// <returns></returns>
		public string GetKey(IDictionaryAdapter dictionaryAdapter, String key, PropertyDescriptor descriptor)
		{
			if (dictionaryBehaviors != null)
			{
				foreach (var builder in KeyBuilders)
				{
					key = builder.GetKey(dictionaryAdapter, key, this);
				}
			}
			return key;
		}

		/// <summary>
		/// Gets the property value.
		/// </summary>
		/// <param name="dictionaryAdapter">The dictionary adapter.</param>
		/// <param name="key">The key.</param>
		/// <param name="storedValue">The stored value.</param>
		/// <param name="descriptor">The descriptor.</param>
		/// <param name="ifExists">true if return only existing.</param>
		/// <returns></returns>
		public object GetPropertyValue(IDictionaryAdapter dictionaryAdapter, string key, object storedValue, PropertyDescriptor descriptor, bool ifExists)
		{
			key = GetKey(dictionaryAdapter, key, descriptor);
			storedValue = storedValue ?? dictionaryAdapter.ReadProperty(key);

			if (dictionaryBehaviors != null)
			{
				foreach (var getter in Getters)
				{
					storedValue = getter.GetPropertyValue(dictionaryAdapter, key, storedValue, this, IfExists || ifExists);
				}
			}

			return storedValue;
		}

		/// <summary>
		/// Sets the property value.
		/// </summary>
		/// <param name="dictionaryAdapter">The dictionary adapter.</param>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <param name="descriptor">The descriptor.</param>
		/// <returns></returns>
		public bool SetPropertyValue(IDictionaryAdapter dictionaryAdapter, string key, ref object value, PropertyDescriptor descriptor)
		{
			key = GetKey(dictionaryAdapter, key, descriptor);

			if (dictionaryBehaviors != null)
			{
				foreach (var setter in Setters)
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
		/// Adds the behaviors.
		/// </summary>
		/// <param name="behaviors">The behaviors.</param>
		public PropertyDescriptor AddBehaviors(params IDictionaryBehavior[] behaviors)
		{
			return AddBehaviors((IEnumerable<IDictionaryBehavior>)behaviors);
		}

		/// <summary>
		/// Adds the behaviors.
		/// </summary>
		/// <param name="behaviors">The behaviors.</param>
		public PropertyDescriptor AddBehaviors(IEnumerable<IDictionaryBehavior> behaviors)
		{
			if (behaviors != null)
			{
				if (dictionaryBehaviors == null)
				{
					dictionaryBehaviors = new SortedSet<IDictionaryBehavior>(behaviors, DictionaryBehaviorComparer.Instance);
				}
				else
				{
					foreach (var behavior in behaviors)
					{
						dictionaryBehaviors.Add(behavior);
					}
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
		public PropertyDescriptor CopyBehaviors(PropertyDescriptor other)
		{
			if (dictionaryBehaviors != null)
			{
				other.AddBehaviors(dictionaryBehaviors.Select(b => b.Copy()).Where(b => b != null));
			}
			return this;
		}

		/// <summary>
		/// Copies the <see cref="PropertyDescriptor"/>
		/// </summary>
		/// <returns></returns>
		public IDictionaryBehavior Copy()
		{
			return new PropertyDescriptor(this, true);
		}

		private void ObtainTypeConverter()
		{
			var converterType = AttributesUtil.GetTypeConverter(Property);

			TypeConverter = (converterType != null)
				? (TypeConverter) Activator.CreateInstance(converterType)
				: TypeDescriptor.GetConverter(PropertyType);
		}
	}
}