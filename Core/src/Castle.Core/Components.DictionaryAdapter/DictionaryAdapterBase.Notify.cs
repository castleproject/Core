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
		private int suppressNotificationCount = 0;
		private bool propagateChildNotifications = true;
		private Dictionary<object, object> composedChildNotifications;

		[ThreadStatic]
		private static TrackPropertyChangeScope readonlyTrackingScope;

		public event PropertyChangingEventHandler PropertyChanging;
		public event PropertyChangedEventHandler PropertyChanged;


		public bool CanNotify { get; set; }

		public bool ShouldNotify
		{
			get { return CanNotify && suppressNotificationCount == 0; }
		}

		public bool PropagateChildNotifications
		{
			get { return propagateChildNotifications; }
			set { propagateChildNotifications = value; }
		}

		public IDisposable SuppressNotificationsBlock()
		{
			return new SuppressNotificationsScope(this);
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
			if (property.SuppressNotifications == false)
			{
				var propertyChanging = PropertyChanging;

				if (propertyChanging != null)
				{
					var eventArgs = new PropertyModifyingEventArgs(property.PropertyName, oldValue, newValue);
					propertyChanging(this, eventArgs);
					return !eventArgs.Cancel;
				}
			}
			return true;
		}

		protected void NotifyPropertyChanged(PropertyDescriptor property, object oldValue, object newValue)
		{
			if (property.SuppressNotifications == false)
			{
				var propertyChanged = PropertyChanged;

				ComposeChildNotifications(property, oldValue, newValue);

				if (propertyChanged != null)
				{
					propertyChanged(this, new PropertyModifiedEventArgs(property.PropertyName, oldValue, newValue));
				}
			}
		}

		protected void NotifyPropertyChanged(string propertyName)
		{
			if (ShouldNotify)
			{
				var propertyChanged = PropertyChanged;

				if (propertyChanged != null)
				{
					propertyChanged(this, new PropertyChangedEventArgs(propertyName));
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
			if (ShouldNotify && readonlyTrackingScope == null)
			{
				var scope = new TrackPropertyChangeScope(this);
				readonlyTrackingScope = scope;
				return scope;
			}
			return null;
		}

		private void ComposeChildNotifications(PropertyDescriptor property, object oldValue, object newValue)
		{
			if (composedChildNotifications == null)
				composedChildNotifications = new Dictionary<object, object>();

			if (oldValue != null)
			{
				object handler;
				if (composedChildNotifications.TryGetValue(oldValue, out handler))
				{
					composedChildNotifications.Remove(oldValue);

					if (oldValue is INotifyPropertyChanged)
					{
						((INotifyPropertyChanged)oldValue).PropertyChanged -= Child_PropertyChanged;
#if !SILVERLIGHT
						if (oldValue is INotifyPropertyChanging)
						{
							((INotifyPropertyChanging)oldValue).PropertyChanging -= Child_PropertyChanging;
						}
					}
					else if (oldValue is IBindingList)
					{
						((IBindingList)oldValue).ListChanged -= (ListChangedEventHandler)handler;
#endif
					}
				}
			}

			if (newValue != null && !composedChildNotifications.ContainsKey(newValue))
			{
				if (newValue is INotifyPropertyChanged)
				{
					((INotifyPropertyChanged)newValue).PropertyChanged += Child_PropertyChanged;
					
#if !SILVERLIGHT
					if (newValue is INotifyPropertyChanging)
					{
						((INotifyPropertyChanging)newValue).PropertyChanging += Child_PropertyChanging;
					}

					composedChildNotifications.Add(newValue, null);
				}
				else if (newValue is IBindingList)
				{
					ListChangedEventHandler handler = (sender, args) =>
					{
						if (propagateChildNotifications)
						{
							var propertyChanged = PropertyChanged;

							if (propertyChanged != null)
							{
								if (args.PropertyDescriptor != null)
								{
									var propertyName = args.PropertyDescriptor.Name;
									propertyChanged(sender, new PropertyChangedEventArgs(propertyName));
								}
								propertyChanged(this, new PropertyChangedEventArgs(property.PropertyName));
							}
						}
					};
					((IBindingList)newValue).ListChanged += handler;

					composedChildNotifications.Add(newValue, handler);
#endif
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

		#region Nested Class: SuppressNotificationsScope

		class SuppressNotificationsScope : IDisposable
		{
			private readonly DictionaryAdapterBase adapter;

			public SuppressNotificationsScope(DictionaryAdapterBase adapter)
			{
				this.adapter = adapter;
				this.adapter.SuppressNotifications();
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
				readonlyProperties = adapter.This.Properties.Values.Where(
					pd => !pd.Property.CanWrite || pd.IsDynamicProperty)
					.ToDictionary(pd => pd, pd => GetEffectivePropertyValue(pd));
			}

			public TrackPropertyChangeScope(DictionaryAdapterBase adapter, PropertyDescriptor property,
											object existingValue)
				: this(adapter)
			{
				this.property = property;
				this.existingValue = existingValue;
				existingValue = adapter.GetProperty(property.PropertyName, true);
			}

			public bool Notify()
			{
				if (readonlyTrackingScope == this)
				{
					readonlyTrackingScope = null;
					return NotifyReadonly();
				}

				var newValue = GetEffectivePropertyValue(property);
				if (NotifyIfChanged(property, existingValue, newValue))
				{
					if (readonlyTrackingScope == null)
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
					var currentValue = GetEffectivePropertyValue(descriptor);
					changed |= NotifyIfChanged(descriptor, readonlyProperty.Value, currentValue);
				}
				adapter.Invalidate();
				return changed;
			}

			private bool NotifyIfChanged(PropertyDescriptor descriptor, object oldValue, object newValue)
			{
				if (Object.Equals(oldValue, newValue) == false)
				{
					adapter.NotifyPropertyChanged(descriptor, oldValue, newValue);
					return true;
				}
				return false;
			}

			private object GetEffectivePropertyValue(PropertyDescriptor property)
			{
				var value = adapter.GetProperty(property.PropertyName, true);
				if (value != null & property.IsDynamicProperty)
				{
					value = ((IDynamicValue)value).GetValue();
				}
				return value;
			}

			public void Dispose()
			{
				Notify();
			}
		}

		#endregion
	}
}
