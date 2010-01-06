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

namespace Castle.Core.Internal
{
	using System;
	using System.Collections;

#if !SILVERLIGHT
	[Serializable]
#endif
	public class LinkedList : IList
	{
		private LinkNode internalhead;
		private LinkNode internaltail;
		private int internalcount;

		public object Head
		{
			get
			{
				if (internalhead == null) return null;

				return internalhead.Value;
			}
		}

		public virtual void AddFirst(object value)
		{
			if (internalhead == null)
			{
				internalhead = new LinkNode(value);
			}
			else
			{
				var newHead = new LinkNode(value, internalhead, null);
				internalhead.Previous = newHead;
				internalhead = newHead;
			}

			internalcount++;
		}

		public virtual int AddLast(object value)
		{
			if (internalhead == null)
			{
				internalhead = new LinkNode(value);
			}
			else
			{
				LinkNode p;
				for (p = internalhead; p.Next != null; p = p.Next) ;
				p.Next = new LinkNode(value, null, p);
			}

			return internalcount++;
		}

		public virtual int Add(object value)
		{
			if (internalhead == null)
			{
				internalhead = new LinkNode(value);

				internaltail = internalhead;
			}
			else
			{
				// Added this code because internaltail seems to be null in come cases
				if (internaltail == null)
				{
					internaltail = internalhead;

					while(internaltail.Next != null)
						internaltail = internaltail.Next;
				}

				internaltail.Next = new LinkNode(value, null, internaltail);

				internaltail = internaltail.Next;
			}

			return internalcount++;
		}

		public bool Contains(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}

			foreach(var item in this)
			{
				if (value.Equals(item))
				{
					return true;
				}
			}

			return false;
		}

		public void Clear()
		{
			internalhead = internaltail = null;
			internalcount = 0;
		}

		public virtual bool Replace(object old, object value)
		{
			var node = internalhead;

			while(node != null)
			{
				if (node.Value.Equals(old))
				{
					node.Value = value;
					return true;
				}

				node = node.Next;
			}

			return false;
		}

		public int IndexOf(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}

			var index = -1;

			foreach(var item in this)
			{
				index++;

				if (value.Equals(item))
				{
					return index;
				}
			}

			return -1;
		}

		public virtual void Insert(int index, object value)
		{
			if (index == 0)
			{
				AddFirst(value);
			}
			else if (index == internalcount)
			{
				AddLast(value);
			}
			else
			{
				var insert = GetNode(index);
				var node = new LinkNode(value, insert, insert.Previous);
				insert.Previous.Next = node;
				insert.Previous = node;
				internalcount++;
			}
		}

		/// <summary>
		/// Returns the node at the specified index.
		/// </summary>
		/// <param name="index">The lookup index.</param>
		/// <returns>The node at the specified index.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// If the specified <paramref name="index"/> is greater than the
		/// number of objects within the list.
		/// </exception>
		private LinkNode GetNode(int index)
		{
			ValidateIndex(index);
			var node = internalhead;
			for(int i = 0; i < index; i++)
			{
				node = node.Next;
			}
			return node;
		}

		/// <summary>
		/// Validates the specified index.
		/// </summary>
		/// <param name="index">The lookup index.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// If the index is invalid.
		/// </exception>
		private void ValidateIndex(int index)
		{
			if (index < 0 || index >= internalcount)
			{
				throw new ArgumentOutOfRangeException("index");
			}
		}

		public virtual void Remove(object value)
		{
			if (internalhead != null)
			{
				if (internalhead.Value.Equals(value))
				{
					if (internalhead == internaltail) internaltail = null;

					internalhead = internalhead.Next;

					internalcount--;
				}
				else if (internaltail.Value.Equals(value))
				{
					internaltail.Previous.Next = null;
					internaltail = internaltail.Previous;

					internalcount--;
				}
				else
				{
					var node = internalhead.Next;
					while(node != null)
					{
						if (node.Value.Equals(value))
						{
							node.Previous.Next = node.Next;
							node.Next.Previous = node.Previous;
							internalcount--;
							break;
						}

						node = node.Next;
					}
				}
			}
		}

		public virtual void RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool IsFixedSize
		{
			get { return false; }
		}

		public object this[int index]
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public int Count
		{
			get { return internalcount; }
		}

		public Array ToArray(Type type)
		{
			Array array = Array.CreateInstance(type, Count);

			int index = 0;

			foreach(var value in this)
			{
				array.SetValue(value, index++);
			}

			return array;
		}

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return new LinkedListEnumerator(internalhead);
		}

		#endregion

		public void CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		public object SyncRoot
		{
			get { return this; }
		}

		public bool IsSynchronized
		{
			get { return false; }
		}
	}

#if !SILVERLIGHT
	[Serializable]
#endif
	internal class LinkNode
	{
		private object value;
		private LinkNode next;
		private LinkNode previous;

		public LinkNode(object value) : this(value, null, null)
		{
		}

		public LinkNode(object value, LinkNode next, LinkNode previous)
		{
			this.value = value;
			this.next = next;
			this.previous = previous;
		}

		public LinkNode Next
		{
			get { return next; }
			set { next = value; }
		}

		public LinkNode Previous
		{
			get { return previous; }
			set { previous = value; }
		}

		public object Value
		{
			get { return value; }
			set { this.value = value; }
		}
	}

	internal class LinkedListEnumerator : IEnumerator
	{
		private readonly LinkNode internalhead;
		private LinkNode current;
		private bool isFirst;

		public LinkedListEnumerator(LinkNode node)
		{
			internalhead = node;
			Reset();
		}

		public void Reset()
		{
			current = internalhead;
			isFirst = true;
		}

		public object Current
		{
			get { return current.Value; }
		}

		public bool MoveNext()
		{
			if (current == null) return false;

			if (!isFirst)
			{
				current = current.Next;
			}
			else
			{
				isFirst = false;
			}

			return current != null;
		}
	}
}