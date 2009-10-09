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
	using System.Linq;
	using System.Collections.Generic;
	using System.ComponentModel;

	public abstract partial class DictionaryAdapterBase
	{
		private Dictionary<string, object> updates;
		private HashSet<IEditableObject> editDependencies;

		public bool CanEdit { get; private set; }

		public bool IsEditing { get; private set; }

		public void BeginEdit()
		{
			IsEditing = CanEdit;
		}

		public void CancelEdit()
		{
			if (IsEditing)
			{
				if (updates != null)
				{
					updates.Clear();
				}

				if (editDependencies != null)
				{
					foreach (var editDependency in editDependencies)
					{
						editDependency.CancelEdit();
					}
					editDependencies.Clear();
				}

				IsEditing = false;
			}
		}

		public void EndEdit()
		{
			if (IsEditing)
			{
				IsEditing = false;

				if (updates != null && updates.Count > 0)
				{
					using (TrackReadonlyPropertyChanges())
					{
						foreach (var update in updates.ToArray())
						{
							object value = update.Value;
							SetProperty(update.Key, ref value);
						}
					}
				}

				if (editDependencies != null)
				{
					foreach (var editDependency in editDependencies)
					{
						editDependency.EndEdit();
					}
					editDependencies.Clear();
				}
			}
		}

		protected bool GetEditedProperty(string propertyName, out object propertyValue)
		{
			propertyValue = null;
			return IsEditing &&  updates != null &&
				updates.TryGetValue(propertyName, out propertyValue);
		}

		protected bool EditProperty(string propertyName, object propertyValue)
		{
			if (IsEditing)
			{
				updates = updates ?? new Dictionary<string, object>();
				updates[propertyName] = propertyValue;
				return true;
			}
			return false;
		}

		protected void AddEditDependency(IEditableObject editDependency)
		{
			if (IsEditing)
			{
				editDependencies = editDependencies ?? new HashSet<IEditableObject>();

				if (editDependencies.Add(editDependency))
				{
					editDependency.BeginEdit();
				}
			}
		}
	}
}
