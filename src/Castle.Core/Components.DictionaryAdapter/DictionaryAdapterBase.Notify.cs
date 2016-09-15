// Copyright 2004-2012 Castle Project - http://www.castleproject.org/
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
		[ThreadStatic]
		private static TrackPropertyChangeScope readOnlyTrackingScope;

		private int suppressNotificationCount = 0;

		public event PropertyChangingEventHandler PropertyChanging;
		public event PropertyChangedEventHandler  PropertyChanged;

		public bool CanNotify { get; set; }

		public bool ShouldNotify
		{
			get { return CanNotify && suppressNotificationCount == 0; }
		}

		public IDisposable SuppressNotificationsBlock()
		{
			return new NotificationSuppressionScope(this);
		}

		public void SuppressNotifications()
		{
			++suppressNotificationCount;
		}

		public void ResumeNotifications()
		{
			--suppressNotificationCount;
		}

		protected bool NotifyPropertyChanging(PropertyDescriptor property, object oldValue, object newValue)
		{
			if (property.SuppressNotifications)
				return true;

			var propertyChanging = PropertyChanging;
			if (propertyChanging == null)
				return true;

			var args = new PropertyChangingEventArgsEx(property.PropertyName, oldValue, newValue);
			propertyChanging(this, args);
			return !args.Cancel;
		}

		protected void NotifyPropertyChanged(PropertyDescriptor property, object oldValue, object newValue)
		{
			if (property.SuppressNotifications)
				return;

			var propertyChanged = PropertyChanged;
			if (propertyChanged == null)
				return;

			propertyChanged(this, new PropertyChangedEventArgsEx(property.PropertyName, oldValue, newValue));
		}

		protected void NotifyPropertyChanged(string propertyName)
		{
			if (!ShouldNotify)
				return;

			var propertyChanged = PropertyChanged;
			if (propertyChanged == null)
				return;

			propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		protected TrackPropertyChangeScope TrackPropertyChange(PropertyDescriptor property, 
															   object oldValue, object newValue)
		{
			if (!ShouldNotify || property.SuppressNotifications)
				return null;

			return new TrackPropertyChangeScope(this, property, oldValue);
		}

		protected TrackPropertyChangeScope TrackReadonlyPropertyChanges()
		{
			if (!ShouldNotify || readOnlyTrackingScope != null)
				return null;

			return readOnlyTrackingScope = new TrackPropertyChangeScope(this);
		}

		private class NotificationSuppressionScope : IDisposable
		{
			private readonly DictionaryAdapterBase adapter;

			public NotificationSuppressionScope(DictionaryAdapterBase adapter)
			{
				this.adapter = adapter;
				this.adapter.SuppressNotifications();
			}

			public void Dispose()
			{
				this.adapter.ResumeNotifications();
			}
		}

		public class TrackPropertyChangeScope : IDisposable
		{
			private readonly DictionaryAdapterBase adapter;
			private readonly PropertyDescriptor property;
			private readonly object existingValue;
			private Dictionary<PropertyDescriptor, object> readOnlyProperties;

			public TrackPropertyChangeScope(DictionaryAdapterBase adapter)
			{
				this.adapter            = adapter;
				this.readOnlyProperties = adapter.This.Properties.Values
					.Where(
						pd => !pd.Property.CanWrite || pd.IsDynamicProperty
					)
					.ToDictionary(
						pd => pd,
						pd => GetEffectivePropertyValue(pd)
					);
			}

			public TrackPropertyChangeScope(DictionaryAdapterBase adapter, PropertyDescriptor property, object existingValue)
				: this(adapter)
			{
				this.property      = property;
				this.existingValue = existingValue;
				existingValue      = adapter.GetProperty(property.PropertyName, true); // TODO: This looks unnecessary
			}

			public void Dispose()
			{
				Notify();
			}

			public bool Notify()
			{
				if (readOnlyTrackingScope == this)
				{
					readOnlyTrackingScope = null;
					return NotifyReadonly();
				}

				var newValue = GetEffectivePropertyValue(property);

				if (!NotifyIfChanged(property, existingValue, newValue))
					return false;

				if (readOnlyTrackingScope == null)
					NotifyReadonly();

				return true;
			}

			private bool NotifyReadonly()
			{
				var changed = false;

				foreach (var readOnlyProperty in readOnlyProperties)
				{
					var descriptor   = readOnlyProperty.Key;
					var currentValue = GetEffectivePropertyValue(descriptor);
					changed |= NotifyIfChanged(descriptor, readOnlyProperty.Value, currentValue);
				}

				adapter.Invalidate();
				return changed;
			}

			private bool NotifyIfChanged(PropertyDescriptor descriptor, object oldValue, object newValue)
			{
				if (Equals(oldValue, newValue))
					return false;

				adapter.NotifyPropertyChanged(descriptor, oldValue, newValue);
				return true;
			}

			private object GetEffectivePropertyValue(PropertyDescriptor property)
			{
				var value = adapter.GetProperty(property.PropertyName, true);
				if (value == null || !property.IsDynamicProperty)
					return value;

				var dynamicValue = value as IDynamicValue;
				if (dynamicValue == null)
					return value;

				return dynamicValue.GetValue();
			}
		}
	}
}
