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

namespace Castle.Rook.Compiler.AST.Util
{
	using System;
	using System.Collections;

	using Foley.Utilities.Collections;


	public abstract class LinkedListBase : IList
	{
		protected LinkedList InnerList = new LinkedList();

		public LinkedListBase()
		{
		}

		protected abstract void PrepareNode(object value);

		int IList.Add(object value)
		{
			PrepareNode(value);

			return InnerList.Add(value);
		}

		public bool Contains(object value)
		{
			return InnerList.Contains(value);
		}

		public void Clear()
		{
			InnerList.Clear();
		}

		public int IndexOf(object value)
		{
			return InnerList.IndexOf(value);
		}

		public void Insert(int index, object value)
		{
			PrepareNode(value);

			InnerList.Insert(index, value);
		}

		public void Remove(object value)
		{
			InnerList.Remove(value);
		}

		public void RemoveAt(int index)
		{
			InnerList.RemoveAt(index);
		}

		public bool IsReadOnly
		{
			get { return InnerList.IsReadOnly; }
		}

		public bool IsFixedSize
		{
			get { return InnerList.IsFixedSize; }
		}

		object IList.this[int index]
		{
			get { return InnerList[index]; }
			set { InnerList[index] = value; }
		}

		public void CopyTo(Array array, int index)
		{
			InnerList.CopyTo(array, index);
		}

		public int Count
		{
			get { return InnerList.Count; }
		}

		public object SyncRoot
		{
			get { return InnerList.SyncRoot; }
		}

		public bool IsSynchronized
		{
			get { return InnerList.IsSynchronized; }
		}

		public IEnumerator GetEnumerator()
		{
			return InnerList.GetEnumerator();
		}

		public override string ToString()
		{
			String content = "";

			foreach(object node in this)
			{
				content += " " + node.GetType();
			}

			return content;
		}

		public void Replace(IASTNode originalNode, IASTNode replace)
		{
			if (!InnerList.Contains(originalNode))
			{
				throw new ArgumentException("Tried to replace inexistent node " + originalNode.ToString());
			}

			int index = InnerList.IndexOf(originalNode);
			
			InnerList.RemoveAt(index);

			if (replace != null)
			{
				PrepareNode(replace);
				InnerList.Insert(index, replace);
			}
		}
	}
}
