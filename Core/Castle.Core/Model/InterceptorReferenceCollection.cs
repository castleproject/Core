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

namespace Castle.Core
{
	using System;
	using System.Collections;

	using Castle.Core.Internal;

	/// <summary>
	/// Collection of <see cref="InterceptorReference"/>
	/// </summary>
	[Serializable]
	public class InterceptorReferenceCollection : ICollection
	{
		private LinkedList list = new LinkedList();

		public void Add(InterceptorReference interceptor)
		{
			list.Add( interceptor );
		}

		public void AddFirst(InterceptorReference interceptor)
		{
			list.AddFirst(interceptor);
		}

		public void AddLast(InterceptorReference interceptor)
		{
			list.AddLast(interceptor);
		}

		public void Insert(int index, InterceptorReference interceptor)
		{
			list.Insert( index, interceptor );
		}

		public bool HasInterceptors
		{
			get { return Count != 0; }
		}

		public void CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		public int Count
		{
			get { return list.Count; }
		}

		public object SyncRoot
		{
			get { return list; }
		}

		public bool IsSynchronized
		{
			get { return false; }
		}

		public IEnumerator GetEnumerator()
		{
			return list.GetEnumerator();
		}
	}
}
