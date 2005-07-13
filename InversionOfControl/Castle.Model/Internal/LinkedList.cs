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
		private LinkNode _head;
		private LinkNode _tail;
		private int _count;

		public LinkedList()
		{
		}

		public object Head
		{
			get
			{
				if (_head == null) return null;
				return _head.Value;
			}
		}

		public void AddFirst(object value)
		{
			if (_head == null)
			{
				_head = new LinkNode(value);
			}
			else
			{
				_head = new LinkNode(value, _head, null);
			}

			_count++;
		}

		public int Add(object value)
		{
			if (_head == null)
			{
				_head = new LinkNode(value);
				
				_tail = _head;
			}
			else
			{
				_tail.Next = new LinkNode(value, null, _tail);
				
				_tail = _tail.Next;
			}

			return _count++;
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
			_head = _tail = null;
			_count = 0;
		}

		public bool Replace(object old, object value)
		{
			LinkNode node = _head;

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
			if (index > _count - 1)
			{
				throw new ArgumentOutOfRangeException("index");
			}

			if (index == 0)
			{
				AddFirst(value);
			}
			else if (index == _count -1)
			{
				Add(value);
			}
			else
			{
				LinkNode node = _head.Next; int indexCur = 0;

				while(node != null)
				{
					if (++indexCur == index)
					{
						node.Previous.Next = new LinkNode(value, node, node.Previous);
						_count++;
						break;
					}

					node = node.Next;
				}
			}
		}

		public void Remove(object value)
		{
			if (_head != null)
			{
				if (_head.Value.Equals( value ))
				{
					if (_head == _tail) _tail = null;
					
					_head = _head.Next;
					
					_count--;
				}
				else if (_tail.Value.Equals( value ))
				{
					_tail.Previous.Next = null;
					_tail = _tail.Previous;
					
					_count--;
				}
				else
				{
					LinkNode node = _head.Next;

					while(node != null)
					{
						if (node.Value.Equals(value))
						{
							node.Previous.Next = node.Next;
							node.Next.Previous = node.Previous;
							_count--;
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
			get { return _count; }
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
			return new LinkedListEnumerator(_head);
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
		private LinkNode _head;
		private LinkNode _current;
		private bool _isFirst;

		public LinkedListEnumerator(LinkNode node)
		{
			_head = node;
			Reset();
		}

		public void Reset()
		{
			_current = _head;
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