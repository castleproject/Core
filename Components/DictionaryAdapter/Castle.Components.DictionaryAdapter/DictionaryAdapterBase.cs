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
	using System.Collections.Specialized;
	using System.ComponentModel;

	public abstract partial class DictionaryAdapterBase : IDictionaryAdapter
	{
		private HybridDictionary state;

		public DictionaryAdapterBase(Type type, IDictionaryAdapterFactory factory,
									 IDictionary dictionary, PropertyDescriptor descriptor)
		{
			Type = type;
			Factory = factory;
			Dictionary = dictionary;
			Descriptor = descriptor;

			CanEdit = typeof(IEditableObject).IsAssignableFrom(type);
			CanNotify = typeof(INotifyPropertyChanged).IsAssignableFrom(type);
			CanValidate = typeof(IDataErrorInfo).IsAssignableFrom(type);

			Initialize();
		}

		public Type Type { get; private set; }

		public IDictionary State
		{
			get
			{
				if (state == null)
					state = new HybridDictionary();
				return state;
			}
		}

		public IDictionary Dictionary { get; private set;  }

		public PropertyDescriptor Descriptor { get; private set; }

		public IDictionaryAdapterFactory Factory { get; private set; }

		public abstract object[] Behaviors { get; }

		public abstract IDictionaryInitializer[] Initializers { get; }

		public abstract IDictionary<string, PropertyDescriptor> Properties { get; }

		public virtual object GetProperty(string propertyName)
		{
			PropertyDescriptor descriptor;
			if (Properties.TryGetValue(propertyName, out descriptor))
			{
				object propertyValue;
				if (GetEditedProperty(propertyName, out propertyValue))
				{
					return propertyValue;
				}

				propertyValue = descriptor.GetPropertyValue(this, propertyName, null, Descriptor);
				if (propertyValue is IEditableObject)
				{
					AddEditDependency((IEditableObject)propertyValue);
				}
				ComposeChildNotifications(descriptor, null, propertyValue);
				return propertyValue;
			}

			return null;
		}

		public T GetPropertyOfType<T>(string propertyName)
		{
			return (T)GetProperty(propertyName);
		}

		public virtual bool SetProperty(string propertyName, ref object value)
		{
			bool stored = false;

			PropertyDescriptor descriptor;
			if (Properties.TryGetValue(propertyName, out descriptor))
			{
				if (EditProperty(propertyName, value))
				{
					return true;
				}

				if (!ShouldNotify)
				{
					return descriptor.SetPropertyValue(this, propertyName, ref value, Descriptor);
				}

				var existingValue = GetProperty(propertyName);
				if (!NotifyPropertyChanging(descriptor, existingValue, value))
				{
					return false;
				}

				var trackPropertyChange = TrackPropertyChange(descriptor, existingValue, value);

				stored = descriptor.SetPropertyValue(this, propertyName, ref value, Descriptor);

				if (stored && trackPropertyChange != null)
				{
					trackPropertyChange.Notify();
				}
			}

			return stored;
		}

		public void FetchProperties()
		{
			foreach (var propertyName in Properties.Keys)
			{
				GetProperty(propertyName);
			}
		}

		protected void Initialize()
		{
			foreach (var initializer in Initializers)
			{
				initializer.Initialize(this, Behaviors);
			}
		}
	}
}
