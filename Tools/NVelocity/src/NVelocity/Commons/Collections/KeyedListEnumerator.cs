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

namespace Commons.Collections
{
	using System;
	using System.Collections;

	public class KeyedListEnumerator : IDictionaryEnumerator
	{
		private int index = -1;
		private ArrayList objs;

		internal KeyedListEnumerator(ArrayList list)
		{
			objs = list;
		}

		public bool MoveNext()
		{
			index++;

			if (index >= objs.Count)
				return false;

			return true;
		}

		public void Reset()
		{
			index = -1;
		}

		public object Current
		{
			get
			{
				if (index < 0 || index >= objs.Count)
					throw new InvalidOperationException();

				return objs[index];
			}
		}

		public DictionaryEntry Entry
		{
			get { return (DictionaryEntry) Current; }
		}

		public object Key
		{
			get { return Entry.Key; }
		}

		public object Value
		{
			get { return Entry.Value; }
		}
	}
}