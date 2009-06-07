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

namespace Castle.Facilities.WcfIntegration.Internal
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Threading;

	internal class Node<T> where T : class
	{
		public T Item;
		public Node<T> Next;
	}

	internal abstract class WcfPool<T> : IEnumerable<T>
		where T : class
	{
		private int count;
		private int maxSize;
		private Node<T> head;
		private Node<T> tail;

		public WcfPool() : this(0)
		{
		}

		public WcfPool(int maxSize) 
		{
			this.maxSize = maxSize;
			Clear();
		}

		public int Count
		{
			get { return count; }
		}

		public T Get()
		{
			do
			{
				Node<T> oldHead = head;
				Node<T> oldTail = tail;
				Node<T> oldHeadNext = oldHead.Next;

				if (oldHead == head)
				{
					if (oldHead == oldTail)
					{
						if (oldHeadNext == null)
							return Create();

						Interlocked.CompareExchange(ref tail, oldHeadNext, oldTail);
					}
					else
					{
						T result = oldHeadNext.Item;
						if (oldHead == Interlocked.CompareExchange(ref head, oldHeadNext, oldHead))
						{
							Interlocked.Decrement(ref count);
							if (Restore(result)) return result;
						}
					}
				}
			} while (true);
		}

		public bool Add(T item) 
		{
			if (!Recycle(item) || (maxSize > 0 && count >= maxSize)) 
				return false;

			Node<T> oldTail, oldTailNext;
			Node<T> node = new Node<T> { Item = item };

			do
			{
				oldTail = tail;
				oldTailNext = oldTail.Next;

				if (tail == oldTail) 
				{
					if (oldTailNext == null)
					{
						if (null == Interlocked.CompareExchange(ref tail.Next, node, null))
							break;
					}
					else 
					{
						Interlocked.CompareExchange(ref tail, oldTailNext, oldTail);
						break;
					}
				}
			} while (true);

			Interlocked.CompareExchange(ref tail, node, oldTail);
			Interlocked.Increment(ref count);
			return true;
		}

		public IEnumerator<T> GetEnumerator()
		{
			Node<T> current = head;
			while (current != null)
			{
				yield return current.Item;
				current = current.Next;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Clear()
		{
			tail = head = new Node<T>();
			count = 0;
		}

		protected virtual bool Recycle(T item)
		{
			return true;
		}

		protected virtual bool Restore(T item)
		{
			return true;
		}

		protected abstract T Create();
	}
}
