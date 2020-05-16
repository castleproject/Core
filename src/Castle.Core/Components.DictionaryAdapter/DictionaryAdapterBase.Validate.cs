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
	using System.Linq;

	public partial class DictionaryAdapterBase : IDictionaryValidate
	{
		private ICollection<IDictionaryValidator> validators;

		public bool CanValidate { get; set; }

		public bool IsValid
		{
			get 
			{
				if (CanValidate && validators != null)
				{
					return !validators.Any(v => !v.IsValid(this));
				}
				return !CanValidate;
			}
		}

		public string Error
		{
			get
			{
				if (CanValidate && validators != null)
				{
					return string.Join(Environment.NewLine, validators.Select(
						v => v.Validate(this)).Where(e => !string.IsNullOrEmpty(e)).ToArray());
				}
				return String.Empty;
			}
		}

		public string this[String columnName]
		{
			get
			{
				if (CanValidate && validators != null)
				{
					PropertyDescriptor property;
					if (This.Properties.TryGetValue(columnName, out property))
					{
						return string.Join(Environment.NewLine, validators.Select(
							v => v.Validate(this, property)).Where(e => !string.IsNullOrEmpty(e))
							.ToArray());
					}
				}
				return String.Empty;
			}
		}

		public DictionaryValidateGroup ValidateGroups(params object[] groups)
		{
			return new DictionaryValidateGroup(groups, this);
		}

		public IEnumerable<IDictionaryValidator> Validators
		{
			get
			{
				return validators ?? Enumerable.Empty<IDictionaryValidator>();
			}
		}

		public void AddValidator(IDictionaryValidator validator)
		{
			if (validators == null)
			{
				validators = new HashSet<IDictionaryValidator>();
			}
			validators.Add(validator);
		}

		protected internal void Invalidate()
		{
			if (CanValidate)
			{
				if (validators != null) foreach (var validator in validators)
				{
					validator.Invalidate(this);
				}

				NotifyPropertyChanged("IsValid");
			}
		}
	}
}
