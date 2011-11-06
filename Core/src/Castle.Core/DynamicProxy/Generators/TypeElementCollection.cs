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
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.DynamicProxy.Generators
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	public class TypeElementCollection<TElement> : ICollection<TElement>
		where TElement : MetaTypeElement, IEquatable<TElement>
	{
		private readonly ICollection<TElement> items = new List<TElement>();

		public int Count
		{
			get { return items.Count; }
		}

		bool ICollection<TElement>.IsReadOnly
		{
			get { return false; }
		}

		public void Add(TElement item)
		{
			if (item.CanBeImplementedExplicitly == false)
			{
				items.Add(item);
				return;
			}
			if (Contains(item))
			{
				item.SwitchToExplicitImplementation();
				if (Contains(item))
				{
					// there is something reaaaly wrong going on here
					throw new ProxyGenerationException("Duplicate element: " + item);
				}
			}
			items.Add(item);
		}

		public bool Contains(TElement item)
		{
			foreach (var element in items)
			{
				if (element.Equals(item))
				{
					return true;
				}
			}

			return false;
		}

		public IEnumerator<TElement> GetEnumerator()
		{
			return items.GetEnumerator();
		}

		void ICollection<TElement>.Clear()
		{
			throw new NotSupportedException();
		}

		void ICollection<TElement>.CopyTo(TElement[] array, int arrayIndex)
		{
			throw new NotSupportedException();
		}

		bool ICollection<TElement>.Remove(TElement item)
		{
			throw new NotSupportedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}