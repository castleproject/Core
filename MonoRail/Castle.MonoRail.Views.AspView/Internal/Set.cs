// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Views.AspView.Internal
{
	using System.Collections.Generic;

	public class StringSet : ICollection<string>
	{
		readonly List<string> internalList = new List<string>();

		#region ICollection<string> Members

		public void Add(string item)
		{
			if (internalList.Contains(item))
				return;
			internalList.Add(item);
		}

		public void Clear()
		{
			internalList.Clear();
		}

		public bool Contains(string item)
		{
			return internalList.Contains(item);
		}

		public void CopyTo(string[] array, int arrayIndex)
		{
			internalList.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return internalList.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(string item)
		{
			return internalList.Remove(item);
		}

		#endregion

		#region IEnumerable<string> Members

		public IEnumerator<string> GetEnumerator()
		{
			return internalList.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return internalList.GetEnumerator();
		}

		#endregion
	}
}
