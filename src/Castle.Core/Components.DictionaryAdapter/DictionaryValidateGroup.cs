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

	public class DictionaryValidateGroup : IDictionaryValidate, INotifyPropertyChanged, IDisposable
	{
		private readonly object[] groups;
		private readonly IDictionaryAdapter adapter;
		private readonly string[] propertyNames;
		private readonly PropertyChangedEventHandler propertyChanged;

		public DictionaryValidateGroup(object[] groups, IDictionaryAdapter adapter)
		{
			this.groups = groups;
			this.adapter = adapter;

			propertyNames = (from property in this.adapter.This.Properties.Values
					  from groupings in property.Annotations.OfType<GroupAttribute>()
					  where this.groups.Intersect(groupings.Group).Any() 
					  select property.PropertyName).Distinct().ToArray();

			if (propertyNames.Length > 0 && adapter.CanNotify)
			{
				propertyChanged += (sender, args) =>
				{
					if (PropertyChanged != null)
						PropertyChanged(this, args);
				};
				this.adapter.PropertyChanged += propertyChanged;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public bool CanValidate
		{
			get { return adapter.CanValidate; }
			set { adapter.CanValidate = value; }
		}

		public bool IsValid
		{
			get { return string.IsNullOrEmpty(Error); }
		}

		public string Error
		{
			get
			{
				return string.Join(Environment.NewLine,
					propertyNames.Select(propertyName => adapter[propertyName])
					.Where(errors => !string.IsNullOrEmpty(errors)).ToArray());
			}
		}

		public string this[string columnName]
		{
			get
			{
				if (Array.IndexOf(propertyNames, columnName) >= 0)
				{
					return adapter[columnName];
				}
				return string.Empty;
			}
		}

		public DictionaryValidateGroup ValidateGroups(params object[] groups)
		{
			groups = this.groups.Union(groups).ToArray();
			return new DictionaryValidateGroup(groups, adapter);
		}

		public IEnumerable<IDictionaryValidator> Validators
		{
			get { return adapter.Validators; }
		}

		public void AddValidator(IDictionaryValidator validator)
		{
			throw new NotSupportedException();
		}

		public void Dispose()
		{
			adapter.PropertyChanged -= propertyChanged;
		}
	}
}
