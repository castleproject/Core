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
	using System.ComponentModel;

	public abstract partial class DictionaryAdapterBase : INotifyPropertyChanged
	{        
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

		protected IDisposable TrackPropertyChange(string propertyName)
		{
			if (WantsPropertyChangeNotification)
			{
				return new TrackPropertyChangeScope(this, propertyName);
			}
			return null;
		}

		#region Nested Class: NotifyPropertyScope

		class TrackPropertyChangeScope : IDisposable
		{
			private readonly string propertyName;
			private readonly object existingValue;
			private readonly DictionaryAdapterBase adapter;

			public TrackPropertyChangeScope(DictionaryAdapterBase adapter, string propertyName)
			{
				this.adapter = adapter;
				this.propertyName = propertyName;
				existingValue = adapter.GetProperty(propertyName);
			}

			public void Dispose()
			{
				var newValue = adapter.GetProperty(propertyName);

				if (!Object.Equals(existingValue, newValue))
				{
					adapter.NotifyPropertyChanged(propertyName);
				}
			}
		}

		#endregion
	}
}
