// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord.Generator.Components
{
	using System;
	using System.Collections;

	using Castle.ActiveRecord.Generator.Components.Database;


	public class BuildContext
	{
		private IDictionary _key2Desc = new Hashtable();
		private IList _newlyCreated = new ArrayList();

		public BuildContext()
		{
		}

		public IList NewlyCreatedDescriptors
		{
			get { return _newlyCreated; }
		}

		public void AddPendentDescriptor(object key, ActiveRecordDescriptor descriptor)
		{
			if (!_key2Desc.Contains(key))
			{
				_key2Desc[key] = descriptor;
			}
		}

		public void IgnorePendent(object key)
		{
			_key2Desc.Remove(key);
		}

		public void RemovePendent(ActiveRecordDescriptor descriptor)
		{
			foreach(DictionaryEntry entry in _key2Desc)
			{
				if (entry.Value == descriptor)
				{
					_key2Desc.Remove(entry.Key);
					break;
				}
			}

			AddToNewlyCreated(descriptor);
		}

		private void AddToNewlyCreated(ActiveRecordDescriptor descriptor)
		{
			if (!_newlyCreated.Contains(descriptor))
			{
				_newlyCreated.Add(descriptor);
			}
		}

		public bool HasPendents
		{
			get { return (_key2Desc.Count != 0); }
		}

		public ActiveRecordDescriptor GetNextPendent()
		{
			if (HasPendents)
			{
				foreach(DictionaryEntry entry in _key2Desc)
				{
					return entry.Value as ActiveRecordDescriptor;
				}
			}

			return null;
		}
	}
}
