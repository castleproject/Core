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
	using System.Collections;
	using System.ComponentModel;
	using System.Linq;
	using System.Reflection;

	public abstract partial class DictionaryAdapterBase : IDictionaryAdapter
	{
		public DictionaryAdapterBase(DictionaryAdapterInstance instance)
		{
			This = instance;

			CanEdit = typeof(IEditableObject).IsAssignableFrom(Meta.Type);
			CanNotify = typeof(INotifyPropertyChanged).IsAssignableFrom(Meta.Type);
#if FEATURE_IDATAERRORINFO
			CanValidate = typeof(IDataErrorInfo).IsAssignableFrom(Meta.Type);
#else
			CanValidate = false;
#endif

			Initialize();
		}

		public abstract DictionaryAdapterMeta Meta { get; }

		public DictionaryAdapterInstance This { get; private set; }

		public string GetKey(string propertyName)
		{
			PropertyDescriptor descriptor;
			if (This.Properties.TryGetValue(propertyName, out descriptor))
			{
				return descriptor.GetKey(this, propertyName, This.Descriptor);
			}
			return null;
		}

		public virtual object GetProperty(string propertyName, bool ifExists)
		{
			PropertyDescriptor descriptor;
			if (This.Properties.TryGetValue(propertyName, out descriptor))
			{
				var propertyValue = descriptor.GetPropertyValue(this, propertyName, null, This.Descriptor, ifExists);
				if (propertyValue is IEditableObject)
				{
					AddEditDependency((IEditableObject)propertyValue);
				}
				return propertyValue;
			}
			return null;
		}

		public T GetPropertyOfType<T>(string propertyName)
		{
			var propertyValue = GetProperty(propertyName, false);
			return propertyValue != null ? (T)propertyValue : default(T);
		}

		public object ReadProperty(string key)
		{
			object propertyValue = null;
			if (GetEditedProperty(key, out propertyValue) == false)
			{
				var dictionary = GetDictionary(This.Dictionary, ref key);
				if (dictionary != null) propertyValue = dictionary[key];
			}
			return propertyValue;
		}

		public virtual bool SetProperty(string propertyName, ref object value)
		{
			bool stored = false;

			PropertyDescriptor descriptor;
			if (This.Properties.TryGetValue(propertyName, out descriptor))
			{
				if (ShouldNotify == false)
				{
					stored = descriptor.SetPropertyValue(this, propertyName, ref value, This.Descriptor);
					Invalidate();
					return stored;
				}

				var existingValue = GetProperty(propertyName, true);
				if (NotifyPropertyChanging(descriptor, existingValue, value) == false)
				{
					return false;
				}

				var trackPropertyChange = TrackPropertyChange(descriptor, existingValue, value);

				stored = descriptor.SetPropertyValue(this, propertyName, ref value, This.Descriptor);

				if (stored && trackPropertyChange != null)
				{
					trackPropertyChange.Notify();
				}
			}

			return stored;
		}

		public void StoreProperty(PropertyDescriptor property, string key, object value)
		{
			if (property == null || EditProperty(property, key, value) == false)
			{
				var dictionary = GetDictionary(This.Dictionary, ref key);
				if (dictionary != null)	dictionary[key] = value;
			}
		}

		public void ClearProperty(PropertyDescriptor property, string key)
		{
			key = key ?? GetKey(property.PropertyName);
			if (property == null || ClearEditProperty(property, key) == false)
			{
				var dictionary = GetDictionary(This.Dictionary, ref key);
				if (dictionary != null) dictionary.Remove(key);
			}	
		}

		public bool ShouldClearProperty(PropertyDescriptor property, object value)
		{
			return property == null ||
				property.Setters.OfType<RemoveIfAttribute>().Where(remove => remove.ShouldRemove(value)).Any();
		}

		public override bool Equals(object obj)
		{
			var other = obj as IDictionaryAdapter;

			if (other == null)
			{
				return false;
			}

			if (ReferenceEquals(this, obj))
			{
				return true;
			}	

			if (Meta.Type != other.Meta.Type)
			{
				return false;
			}

			if (This.EqualityHashCodeStrategy != null)
			{
				return This.EqualityHashCodeStrategy.Equals(this, other);
			}

			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			if (This.OldHashCode.HasValue)
			{
				return This.OldHashCode.Value;
			}

			int hashCode;
			if (This.EqualityHashCodeStrategy == null ||
				This.EqualityHashCodeStrategy.GetHashCode(this, out hashCode) == false)
			{
				hashCode = base.GetHashCode();
			}

			This.OldHashCode = hashCode;
			return hashCode;
		}

		protected void Initialize()
		{
			var metaBehaviors = Meta.Behaviors;
			var initializers  = This.Initializers;

			foreach (var initializer in initializers)
			{
				initializer.Initialize(this, metaBehaviors);
			}

			foreach (var property in This.Properties.Values)
			{
				if (property.Fetch)
					GetProperty(property.PropertyName, false);
			}
		}

		private static IDictionary GetDictionary(IDictionary dictionary, ref string key)
		{
			if (key.StartsWith("!") == false)
			{
				var parts = key.Split(',');
				for (var i = 0; i < parts.Length - 1; ++i)
				{
					dictionary = dictionary[parts[i]] as IDictionary;
					if (dictionary == null) return null;
				}
				key = parts[parts.Length - 1];
			}
			return dictionary;
		}
	}
}
