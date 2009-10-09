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

	#region Class PropertyModifiedEventArgs

	public class PropertyModifiedEventArgs : PropertyChangedEventArgs
	{
		public PropertyModifiedEventArgs(String propertyName, object oldPropertyValue, object newPropertyValue)
			: base(propertyName)
		{
			OldPropertyValue = oldPropertyValue;
			NewPropertyValue = newPropertyValue;
		}

		public object OldPropertyValue { get; private set; }

		public object NewPropertyValue { get; private set; }
	}

	#endregion

	#region Class PropertyChangingEventArgs

	public class PropertyChangingEventArgs : PropertyModifiedEventArgs
	{
		public PropertyChangingEventArgs(String propertyName, object oldPropertyValue, object newPropertyValue)
			: base(propertyName, oldPropertyValue, newPropertyValue)
		{
		}

		public bool Cancel { get; set; }
	}

	public delegate void PropertyChangingEventHandler(object sender, PropertyChangingEventArgs e);

	#endregion

	/// <summary>
	/// Contract for managing Dictionary adapter notifications.
	/// </summary>
	public interface IDictionaryNotify : INotifyPropertyChanged
	{
		bool CanNotify { get; }

		bool ShouldNotify { get; }

		event PropertyChangingEventHandler PropertyChanging;

		bool PropagateChildNotifications { get; set; }

		IDisposable SupressNotificationsSection();

		void SupressNotifications();

		void ResumeNotifications();

	}
}
