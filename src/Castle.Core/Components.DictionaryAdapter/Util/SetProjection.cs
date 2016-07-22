// Copyright 2004-2012 Castle Project - http://www.castleproject.org/
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

#if DOTNET40
namespace Castle.Components.DictionaryAdapter
{
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;

	public class SetProjection<T> : ListProjection<T>, ISet<T>
	{
		private readonly HashSet<T> set;

		public SetProjection(ICollectionAdapter<T> adapter)
			: base(adapter)
		{
			set = new HashSet<T>();
			Repopulate();
		}

		public override bool Contains(T item)
		{
			return set.Contains(item);
		}

		public bool IsSubsetOf(IEnumerable<T> other)
		{
			return set.IsSubsetOf(other);
		}

		public bool IsSupersetOf(IEnumerable<T> other)
		{
			return set.IsSupersetOf(other);
		}

		public bool IsProperSubsetOf(IEnumerable<T> other)
		{
			return set.IsProperSubsetOf(other);
		}

		public bool IsProperSupersetOf(IEnumerable<T> other)
		{
			return set.IsProperSupersetOf(other);
		}

		public bool Overlaps(IEnumerable<T> other)
		{
			return set.Overlaps(other);
		}

		public bool SetEquals(IEnumerable<T> other)
		{
			return set.SetEquals(other);
		}

		private void Repopulate()
		{
			SuspendEvents();

			var count = Count;
			for (var index = 0; index < count;)
			{
				var value = this[index];

				if (!set.Add(value))
					{ RemoveAt(index); count--; }
				else
					index++;
			}

			ResumeEvents();
		}

		public override void EndNew(int index)
		{
			if (IsNew(index) && OnInserting(this[index]))
				base.EndNew(index);
			else
				CancelNew(index);
		}
		
		public override bool Add(T item)
		{
			return !set.Contains(item)
				&& base.Add(item);
		}

		protected override bool OnInserting(T value)
		{
			return set.Add(value);
		}

		protected override bool OnReplacing(T oldValue, T newValue)
		{
			if (!set.Add(newValue))
				return false;

			set.Remove(oldValue);
			return true;
		}

		public override bool Remove(T item)
		{
			return set .Remove(item)
				&& base.Remove(item);
		}

		public override void RemoveAt(int index)
		{
			set .Remove  (this[index]);
			base.RemoveAt(index);
		}

		public override void Clear()
		{
			set .Clear();
			base.Clear();
		}

		public void UnionWith(IEnumerable<T> other)
		{
			foreach (var value in other)
				Add(value);
		}

		public void ExceptWith(IEnumerable<T> other)
		{
			foreach (var value in other)
				Remove(value);
		}

		public void IntersectWith(IEnumerable<T> other)
		{
			var removals = set.Except(other).ToArray();

			ExceptWith(removals);
		}

		public void SymmetricExceptWith(IEnumerable<T> other)
		{
			var removals  = set.Intersect(other).ToArray();
			var additions = other.Except(removals);

			ExceptWith(removals);
			UnionWith(additions);
		}
	}
}
#endif
