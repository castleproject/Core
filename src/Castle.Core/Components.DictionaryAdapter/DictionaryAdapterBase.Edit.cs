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
	using System.Reflection;

	public abstract partial class DictionaryAdapterBase
	{
		private int suppressEditingCount = 0;
		private Stack<Dictionary<string, Edit>> updates;
		private HashSet<IEditableObject> editDependencies;
		
		struct Edit
		{
			public Edit(PropertyDescriptor property, object propertyValue)
			{
				Property = property;
				PropertyValue = propertyValue;
			}
			public readonly PropertyDescriptor Property;
			public object PropertyValue;
		}

		public bool CanEdit
		{
			get { return suppressEditingCount == 0 && updates != null; }
			set { updates = value ? new Stack<Dictionary<string, Edit>>() : null; }
		}

		public bool IsEditing
		{
			get { return CanEdit && updates != null && updates.Count > 0; }
		}

		public bool SupportsMultiLevelEdit { get; set; }

		public bool IsChanged
		{
			get
			{
				if (IsEditing && updates.Any(level => level.Count > 0))
					return true;

				return This.Properties.Values
					.Where(prop => typeof(IChangeTracking).IsAssignableFrom(prop.PropertyType))
					.Select(prop => GetProperty(prop.PropertyName, true))
					.Cast<IChangeTracking>().Any(track => track != null && track.IsChanged);
			}
		}

		public void BeginEdit()
		{
			if (CanEdit && (IsEditing == false || SupportsMultiLevelEdit))
			{
				updates.Push(new Dictionary<string, Edit>());
			}
		}

		public void CancelEdit()
		{
			if (IsEditing)
			{
				if (editDependencies != null)
				{
					foreach (var editDependency in editDependencies.ToArray())
					{
						editDependency.CancelEdit();
					}
					editDependencies.Clear();
				}

				using (SuppressEditingBlock())
				{
					using (TrackReadonlyPropertyChanges())
					{
						var top = updates.Peek();

						if (top.Count > 0)
						{
							foreach (var update in top.Values)
							{
								var existing = update;
								existing.PropertyValue = GetProperty(existing.Property.PropertyName, true);
							}

							updates.Pop();

							foreach (var update in top.Values.ToArray())
							{
								var oldValue = update.PropertyValue;
								var newValue = GetProperty(update.Property.PropertyName, true);
								
								if (!Object.Equals(oldValue, newValue))
								{

									NotifyPropertyChanging(update.Property, oldValue, newValue);
									NotifyPropertyChanged(update.Property, oldValue, newValue);

								}
							}
						}
					}
				}
			}
		}

		public void EndEdit()
		{
			if (IsEditing)
			{
				using (SuppressEditingBlock())
				{
					var top = updates.Pop();

					if (top.Count > 0) foreach (var update in top.ToArray())
					{
						StoreProperty(null, update.Key, update.Value.PropertyValue);
					}
				}
				
				if (editDependencies != null)
				{
					foreach (var editDependency in editDependencies.ToArray())
					{
						editDependency.EndEdit();
					}
					editDependencies.Clear();
				}
			}
		}

		public void RejectChanges()
		{
			CancelEdit();
		}

		public void AcceptChanges()
		{
			EndEdit();
		}

		public IDisposable SuppressEditingBlock()
		{
			return new SuppressEditingScope(this);
		}

		public void SuppressEditing()
		{
			++suppressEditingCount;
		}

		public void ResumeEditing()
		{
			--suppressEditingCount;
		}

		protected bool GetEditedProperty(string propertyName, out object propertyValue)
		{
			if (updates != null) foreach (var level in updates.ToArray())
			{
				Edit edit;
				if (level.TryGetValue(propertyName, out edit))
				{
					propertyValue = edit.PropertyValue;
					return true;
				}
			}
			propertyValue = null;
			return false;
		}

		protected bool EditProperty(PropertyDescriptor property, string key, object propertyValue)
		{
			if (IsEditing)
			{
				updates.Peek()[key] = new Edit(property, propertyValue);
				return true;
			}
			return false;
		}

		protected bool ClearEditProperty(PropertyDescriptor property, string key)
		{
			if (IsEditing)
			{
				updates.Peek().Remove(key);
				return true;
			}
			return false;
		}

		protected void AddEditDependency(IEditableObject editDependency)
		{
			if (IsEditing)
			{
				if (editDependencies == null)
				{
					editDependencies = new HashSet<IEditableObject>();
				}

				if (editDependencies.Add(editDependency))
				{
					editDependency.BeginEdit();
				}
			}
		}

		#region Nested Class: SuppressEditingScope

		class SuppressEditingScope : IDisposable
		{
			private readonly DictionaryAdapterBase adapter;

			public SuppressEditingScope(DictionaryAdapterBase adapter)
			{
				this.adapter = adapter;
				this.adapter.SuppressEditing();
			}

			public void Dispose()
			{
				adapter.ResumeEditing();
			}
		}

		#endregion
	}
}
