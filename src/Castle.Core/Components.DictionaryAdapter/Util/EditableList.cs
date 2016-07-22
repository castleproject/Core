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
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;

	public class EditableList<T> : List<T>, IEditableObject, IRevertibleChangeTracking
	{
		private bool isEditing;
		private List<T> snapshot;

		public EditableList()
		{
		}

		public EditableList(IEnumerable<T> collection) 
			: base(collection)
		{
		}

		public void BeginEdit()
		{
			if (isEditing == false)
			{
				snapshot = new List<T>(this);
				isEditing = true;
			}
		}

		public bool IsChanged
		{
			get
			{
				if (snapshot == null || snapshot.Count != Count)
					return false;

				var items = GetEnumerator();
				var snapshotItems = snapshot.GetEnumerator();

				while (items.MoveNext() && snapshotItems.MoveNext())
				{
					if (ReferenceEquals(items.Current, snapshotItems.Current) == false)
						return false;

					var tracked = items.Current as IChangeTracking;
					if (tracked != null && tracked.IsChanged)
						return true;
				}

				return false;
			}
		}

		public void EndEdit()
		{
			isEditing = false;
			snapshot = null;
		}

		public void CancelEdit()
		{
			if (isEditing)
			{
				Clear();
				AddRange(snapshot);
				snapshot = null;
				isEditing = false;
			}
		}

		public void AcceptChanges()
		{
			BeginEdit();
		}

		public void RejectChanges()
		{
			CancelEdit();
		}
	}

	public class EditableList : EditableList<object>, IList
	{
		public EditableList()
		{
		}

		public EditableList(IEnumerable<object> collection)
			: base(collection)
		{
		}
	}
}
