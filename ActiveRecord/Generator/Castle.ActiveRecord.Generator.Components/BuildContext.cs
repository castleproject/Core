// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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
		private IList _pendents = new ArrayList();
		private IList _newlyCreated = new ArrayList();

		public BuildContext()
		{
		}

		public IList NewlyCreatedDescriptors
		{
			get { return _newlyCreated; }
		}

		public void AddPendentDescriptor(ActiveRecordDescriptor descriptor)
		{
			if (!_pendents.Contains(descriptor))
			{
				_pendents.Add(descriptor);
			}
		}

		public void RemovePendent(ActiveRecordDescriptor descriptor)
		{
			_pendents.Remove(descriptor);

			_newlyCreated.Add(descriptor);
		}

		public bool HasPendents
		{
			get { return (_pendents.Count != 0); }
		}

		public ActiveRecordDescriptor GetNextPendent()
		{
			if (HasPendents)
			{
				return _pendents[0] as ActiveRecordDescriptor;
			}

			return null;
		}
	}
}
