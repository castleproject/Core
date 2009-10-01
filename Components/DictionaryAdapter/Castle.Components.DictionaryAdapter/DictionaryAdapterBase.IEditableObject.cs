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
	using System.Collections.Generic;
	using System.ComponentModel;

	public abstract partial class DictionaryAdapterBase : IEditableObject
	{
		private readonly IDictionary<string, object> updates;

		public bool IsEditable { get; private set; }

		public bool IsEditing { get; private set; }

		void IEditableObject.BeginEdit()
		{
			IsEditing = true;
		}

		void IEditableObject.CancelEdit()
		{
			updates.Clear();
			IsEditing = false;
		}

		void IEditableObject.EndEdit()
		{
			if (IsEditing)
			{
				IsEditing = false;

				using (TrackReadonlyPropertyChanges())
                {
					foreach (var update in updates)
					{
						object value = update.Value;
						SetProperty(update.Key, ref value);
					}                	
                }
			}
		}

		protected bool GetEditedProperty(string propertyName, out object propertyValue)
		{
			propertyValue = null;
			return IsEditing && updates.TryGetValue(propertyName, out propertyValue);
		}

		protected bool EditProperty(string propertyName, object propertyValue)
		{
			if (IsEditing)
			{
				updates[propertyName] = propertyValue;
				return true;
			}
			return false;
		}
	}
}
