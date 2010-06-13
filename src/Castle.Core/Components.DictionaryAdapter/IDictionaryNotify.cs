// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

	#region Class PropertyModifyingEventArgs

	public class PropertyModifyingEventArgs : PropertyChangingEventArgs
	{
		public PropertyModifyingEventArgs(String propertyName, object oldPropertyValue, object newPropertyValue)
			: base(propertyName)
		{
			OldPropertyValue = oldPropertyValue;
			NewPropertyValue = newPropertyValue;
		}

		public object OldPropertyValue { get; private set; }

		public object NewPropertyValue { get; private set; }

		public bool Cancel { get; set; }
	}

	public delegate void PropertyModifyingEventHandler(object sender, PropertyModifyingEventArgs e);

	#endregion

	/// <summary>
	/// Contract for managing Dictionary adapter notifications.
	/// </summary>
	public interface IDictionaryNotify : 
#if !SILVERLIGHT
		INotifyPropertyChanging, 
#endif
		INotifyPropertyChanged
	{
		bool CanNotify { get; }

		bool ShouldNotify { get; }

		bool PropagateChildNotifications { get; set; }

		IDisposable SuppressNotificationsBlock();

		void SuppressNotifications();

		void ResumeNotifications();
	}
}
