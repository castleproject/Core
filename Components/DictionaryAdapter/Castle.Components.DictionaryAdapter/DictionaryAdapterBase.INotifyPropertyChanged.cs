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

	public abstract partial class DictionaryAdapterBase : INotifyPropertyChanged
	{
		[ThreadStatic]
		private static TrackPropertyChangeScope ReadonlyTrackingScope;

		public bool WantsPropertyChangeNotification { get; private set; }

		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add { _propertyChanged += value; }
			remove { _propertyChanged -= value; }
		}
		private PropertyChangedEventHandler _propertyChanged;

		protected void NotifyPropertyChanged(string propertyName)
		{
			var propertyChanged = _propertyChanged;

			if (propertyChanged != null)
			{
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		protected TrackPropertyChangeScope TrackPropertyChange(string propertyName)
		{
			if (WantsPropertyChangeNotification)
			{
				return new TrackPropertyChangeScope(this, propertyName);
			}
			return null;
		}

		protected TrackPropertyChangeScope TrackReadonlyPropertyChanges()
		{
			if (WantsPropertyChangeNotification && ReadonlyTrackingScope == null)
			{
				var scope = new TrackPropertyChangeScope(this);
				ReadonlyTrackingScope = scope;
				return scope;
			}
			return null;
		}

		#region Nested Class: TrackPropertyChangeScope

		public class TrackPropertyChangeScope : IDisposable
		{
			private readonly string propertyName;
			private readonly object existingValue;
			private readonly DictionaryAdapterBase adapter;
			private IDictionary<string, object> readonlyProperties;

			public TrackPropertyChangeScope(DictionaryAdapterBase adapter)
			{
				this.adapter = adapter;
				readonlyProperties = adapter.Properties.Where(pd => !pd.Value.Property.CanWrite)
					.ToDictionary(pd => pd.Key, pd => adapter.GetProperty(pd.Key));
			}

			public TrackPropertyChangeScope(DictionaryAdapterBase adapter, string propertyName)
				: this(adapter)
			{
				this.propertyName = propertyName;
				existingValue = adapter.GetProperty(propertyName);
			}

			public bool Notify()
			{
				if (ReadonlyTrackingScope == this)
				{
					ReadonlyTrackingScope = null;
					return NotifyReadonly();
				}

				var newValue = adapter.GetProperty(propertyName);
				if (NotifyIfChanged(propertyName, existingValue, newValue))
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
					var propertyName = readonlyProperty.Key;
					var currentValue = adapter.GetProperty(propertyName);
					changed |= NotifyIfChanged(propertyName, readonlyProperty.Value, currentValue);
				}
				return changed;
			}

			private bool NotifyIfChanged(string propertyName, object oldValue, object newValue)
			{
				if (!Object.Equals(oldValue, newValue))
				{
					adapter.NotifyPropertyChanged(propertyName);
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
