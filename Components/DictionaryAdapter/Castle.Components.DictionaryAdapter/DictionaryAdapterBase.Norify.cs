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
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;

	public abstract partial class DictionaryAdapterBase
	{
		private int supressNotificationCount = 0;
		private bool propagateChildNotifications = true;
		private HashSet<object> composedChildNotifications;

		[ThreadStatic]
		private static TrackPropertyChangeScope ReadonlyTrackingScope;

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

		public bool SupportsNotification { get; set; }

		public bool ShouldNotify
		{
			get { return SupportsNotification && supressNotificationCount == 0; }
		}

		public bool PropagateChildNotifications
		{
			get { return propagateChildNotifications; }
			set { propagateChildNotifications = value; }
		}
        
		public IDisposable SupressNotificationsSection()
		{
			return new SupressNotificationsScope(this);
		}

		public void SupressNotifications()
		{
			++supressNotificationCount;
		}

		public void ResumeNotifications()
		{
			--supressNotificationCount;
		}
        
		protected bool NotifyPropertyChanging(PropertyDescriptor property, object oldValue, object newValue)
		{
			if (!property.SuppressNotifications)
			{
				var propertyChanging = PropertyChanging;

				if (propertyChanging != null)
				{
					var eventArgs = new PropertyChangingEventArgs(property.PropertyName, oldValue, newValue);
					propertyChanging(this, eventArgs);
					return !eventArgs.Cancel;
				}
			}
			return true;
		}

		protected void NotifyPropertyChanged(PropertyDescriptor property, object oldValue, object newValue)
		{
			if (!property.SuppressNotifications)
			{
				var propertyChanged = PropertyChanged;

				ComposeChildNotifications(oldValue, newValue);

				if (propertyChanged != null)
				{
					propertyChanged(this, new PropertyModifiedEventArgs(property.PropertyName, oldValue, newValue));
				}
			}
		}

		protected TrackPropertyChangeScope TrackPropertyChange(PropertyDescriptor property, 
															   object oldValue, object newValue)
		{
			if (ShouldNotify && !property.SuppressNotifications)
			{
				return new TrackPropertyChangeScope(this, property, oldValue);
			}
			return null;
		}

		protected TrackPropertyChangeScope TrackReadonlyPropertyChanges()
		{
			if (ShouldNotify && ReadonlyTrackingScope == null)
			{
				var scope = new TrackPropertyChangeScope(this);
				ReadonlyTrackingScope = scope;
				return scope;
			}
			return null;
		}

		private void ComposeChildNotifications(object oldValue, object newValue)
		{
			if (composedChildNotifications == null)
				composedChildNotifications = new HashSet<object>();

			if (oldValue is INotifyPropertyChanged && composedChildNotifications.Remove(oldValue))
			{
				((INotifyPropertyChanged)oldValue).PropertyChanged -= Child_PropertyChanged;

				if (oldValue is IDictionaryNotify)
				{
					((IDictionaryNotify)oldValue).PropertyChanging -= Child_PropertyChanging;
				}
			}

			if (newValue is INotifyPropertyChanged && composedChildNotifications.Add(newValue))
			{
				((INotifyPropertyChanged)newValue).PropertyChanged += Child_PropertyChanged;

				if (newValue is IDictionaryNotify)
				{
					((IDictionaryNotify)newValue).PropertyChanging += Child_PropertyChanging;
				}
			}
		}

		private void Child_PropertyChanging(object sender, PropertyChangingEventArgs e)
		{
			if (propagateChildNotifications)
			{
				var propertyChanging = PropertyChanging;

				if (propertyChanging != null)
				{
					propertyChanging(sender, e);
				}
			}
		}

		private void Child_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (propagateChildNotifications)
			{
				var propertyChanged = PropertyChanged;

				if (propertyChanged != null)
				{
					propertyChanged(sender, e);
				}
			}
		}

		#region Nested Class: SupressNotificationsScope

		class SupressNotificationsScope : IDisposable
		{
			private readonly DictionaryAdapterBase adapter;

			public SupressNotificationsScope(DictionaryAdapterBase adapter)
			{
				this.adapter = adapter;
				this.adapter.SupressNotifications();
			}

			public void Dispose()
			{
				this.adapter.ResumeNotifications();
			}
		}

		#endregion

		#region Nested Class: TrackPropertyChangeScope

		public class TrackPropertyChangeScope : IDisposable
		{
			private readonly DictionaryAdapterBase adapter;
			private readonly PropertyDescriptor property;
			private readonly object existingValue;
			private IDictionary<PropertyDescriptor, object> readonlyProperties;

			public TrackPropertyChangeScope(DictionaryAdapterBase adapter)
			{
				this.adapter = adapter;
				readonlyProperties = adapter.Properties.Where(pd => !pd.Value.Property.CanWrite)
					.ToDictionary(pd => pd.Value, pd => adapter.GetProperty(pd.Key));
			}

			public TrackPropertyChangeScope(DictionaryAdapterBase adapter, PropertyDescriptor property,
											object existingValue)
				: this(adapter)
			{
				this.property = property;
				this.existingValue = existingValue;
				existingValue = adapter.GetProperty(property.PropertyName);
			}

			public bool Notify()
			{
				if (ReadonlyTrackingScope == this)
				{
					ReadonlyTrackingScope = null;
					return NotifyReadonly();
				}

				var newValue = adapter.GetProperty(property.PropertyName);
				if (NotifyIfChanged(property, existingValue, newValue))
				{
					if (ReadonlyTrackingScope == null)
						NotifyReadonly();
					return true;
				}

				return false;
			}

			private bool NotifyReadonly()
			{
				bool changed = false;
				foreach (var readonlyProperty in readonlyProperties)
				{
					var descriptor = readonlyProperty.Key;
					var currentValue = adapter.GetProperty(descriptor.PropertyName);
					changed |= NotifyIfChanged(descriptor, readonlyProperty.Value, currentValue);
				}
				return changed;
			}

			private bool NotifyIfChanged(PropertyDescriptor descriptor, object oldValue, object newValue)
			{
				if (!Object.Equals(oldValue, newValue))
				{
					adapter.NotifyPropertyChanged(descriptor, oldValue, newValue);
					return true;
				}
				return false;
			}

			public void Dispose()
			{
				Notify();
			}
		}

		#endregion
	}
}
