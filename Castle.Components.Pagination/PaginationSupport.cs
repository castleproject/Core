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

namespace Castle.Components.Pagination
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	/// Common pagination support methods
	/// </summary>
	public static class PaginationSupport
	{
		/// <summary>
		/// Subsititute for <see cref="Enumerable.ElementAt{TSource}"/>.
		/// </summary>
		/// <param name="enumerable"></param>
		/// <param name="itemIndex"></param>
		/// <returns></returns>
		public static object GetItemAtIndex(IEnumerable enumerable, int itemIndex)
		{
			EnsureItemIndexIsInRange(itemIndex);

			object item;

			IList list = enumerable as IList;

			if (list != null)
			{
				item = list[itemIndex];
			}
			else
			{
				item = enumerable.Cast<object>().Skip(itemIndex).Take(1).Single();
			}

			return item;
		}

		private static void EnsureItemIndexIsInRange(int itemIndex)
		{
			if (itemIndex < 0)
			{
				throw new ArgumentOutOfRangeException("itemIndex");
			}
		}

		/// <summary>
		/// Subsititute for Subsititute for <see cref="Enumerable.ElementAt{TSource}"/>.
		/// </summary>
		/// <param name="enumerable"></param>
		/// <param name="itemIndex"></param>
		/// <returns></returns>
		public static T GetItemAtIndex<T>(IEnumerable<T> enumerable, int itemIndex)
		{
			EnsureItemIndexIsInRange(itemIndex);

			T item;

			IList<T> list = enumerable as IList<T>;

			if (list != null)
			{
				item = list[itemIndex];
			}
			else
			{
				item = enumerable.Skip(itemIndex).Take(1).Single();
			}

			return item;
		}
	}
}