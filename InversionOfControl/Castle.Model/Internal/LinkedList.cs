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

	public class LinkedList : IEnumerable
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

		public void Add(object value)
		{
			if (_head == null)
			{
				_head = new LinkNode(value);
			}
			else
			{
				if (_tail == null) _tail = _head;

				_tail.Next = new LinkNode(value);
				_tail = _tail.Next;
			}

			_count++;
		}

		public void AddFirst(object value)
		{
			if (_head == null)
			{
				_head = new LinkNode(value);
			}
			else
			{
				_head = new LinkNode(value, _head);
			}

			_count++;
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
	}

	class LinkNode
	{
		private object _value;
		private LinkNode _next;

		public LinkNode(object value) : this(value, null)
		{
		}

		public LinkNode(object value, LinkNode next)
		{
			_value = value;
			_next = next;
		}

		public object Value
		{
			get { return _value; }
		}

		public LinkNode Next
		{
			get { return _next; }
			set { _next = value; }
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

		#region IEnumerator Members

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

		#endregion
	}
}