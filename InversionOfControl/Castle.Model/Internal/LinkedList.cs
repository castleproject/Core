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

namespace Castle.Model.Internal
{
	using System;
	using System.Collections;

	public class LinkedList : IList
	{
		private LinkNode internalhead;
		private LinkNode internaltail;
		private int internalcount;

		public LinkedList()
		{
		}

		public object Head
		{
			get
			{
				if (internalhead == null) return null;
				return internalhead.Value;
			}
		}

		public void AddFirst(object value)
		{
			if (internalhead == null)
			{
				internalhead = new LinkNode(value);
			}
			else
			{
				internalhead = new LinkNode(value, internalhead, null);
			}

			internalcount++;
		}

		public int Add(object value)
		{
			if (internalhead == null)
			{
				internalhead = new LinkNode(value);
				
				internaltail = internalhead;
			}
			else
			{
				internaltail.Next = new LinkNode(value, null, internaltail);
				
				internaltail = internaltail.Next;
			}

			return internalcount++;
		}

		public bool Contains(object value)
		{
			if (value == null) throw new ArgumentNullException("value");

			foreach(object item in this)
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

		public bool Replace(object old, object value)
		{
			LinkNode node = internalhead;

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
			if (value == null) throw new ArgumentNullException("value");

			int index = -1;

			foreach(object item in this)
			{
				index++;

				if (value.Equals(item))
				{
					return index;
				}
			}

			return -1;
		}

		public void Insert(int index, object value)
		{
			if (index > internalcount - 1)
			{
				throw new ArgumentOutOfRangeException("index");
			}

			if (index == 0)
			{
				AddFirst(value);
			}
			else if (index == internalcount -1)
			{
				Add(value);
			}
			else
			{
				LinkNode node = internalhead.Next; int indexCur = 0;

				while(node != null)
				{
					if (++indexCur == index)
					{
						node.Previous.Next = new LinkNode(value, node, node.Previous);
						internalcount++;
						break;
					}

					node = node.Next;
				}
			}
		}

		public void Remove(object value)
		{
			if (internalhead != null)
			{
				if (internalhead.Value.Equals( value ))
				{
					if (internalhead == internaltail) internaltail = null;
					
					internalhead = internalhead.Next;
					
					internalcount--;
				}
				else if (internaltail.Value.Equals( value ))
				{
					internaltail.Previous.Next = null;
					internaltail = internaltail.Previous;
					
					internalcount--;
				}
				else
				{
					LinkNode node = internalhead.Next;

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

		public void RemoveAt(int index)
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
			Array array = Array.CreateInstance( type, Count );

			int index = 0;

			foreach(Object value in this)
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

	class LinkNode
	{
		private object _value;
		private LinkNode _next;
		private LinkNode _prev;

		public LinkNode(object value) : this(value, null, null)
		{
		}

		public LinkNode(object value, LinkNode next, LinkNode prev)
		{
			_value = value;
			_next = next;
			_prev = prev;
		}

		public LinkNode Next
		{
			get { return _next; }
			set { _next = value; }
		}

		public LinkNode Previous
		{
			get { return _prev; }
			set { _prev = value; }
		}

		public object Value
		{
			get { return _value; }
			set { _value = value; }
		}
	}

	class LinkedListEnumerator : IEnumerator
	{
		private LinkNode internalhead;
		private LinkNode _current;
		private bool _isFirst;

		public LinkedListEnumerator(LinkNode node)
		{
			internalhead = node;
			Reset();
		}

		public void Reset()
		{
			_current = internalhead;
			_isFirst = true;
		}

		public object Current
		{
			get { return _current.Value; }
		}

		public bool MoveNext()
		{
			if (_current == null) return false;

			if (!_isFirst)
			{
				_current = _current.Next;
			}
			else
			{
				_isFirst = false;
			}

			return _current != null;
		}
	}
}
