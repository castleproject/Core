// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.f
// See the License for the specific language governing permissions and
// limitations under the License.

#if !SILVERLIGHT
namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;

	public class SingleIterator<T> : Iterator<T>
	{
		private readonly T item;
		private int position;

		public SingleIterator(T item)
		{
			this.item     = item;
			this.position = -1;
		}

		public override bool HasCurrent
		{
			get { return 0 == position; }
		}

		public override T Current
		{
			get { return HasCurrent ? item : OnNoCurrent(); }
		}

		public override bool MoveNext()
		{
			return 0 == ++position;
		}
	}
}
#endif
