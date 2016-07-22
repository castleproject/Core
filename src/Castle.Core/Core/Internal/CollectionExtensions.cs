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

namespace Castle.Core.Internal
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class CollectionExtensions
	{
		public static T Find<T>(this T[] items, Predicate<T> predicate)
		{
			return Array.Find(items, predicate);
		}

		public static T[] FindAll<T>(this T[] items, Predicate<T> predicate)
		{
			return Array.FindAll(items, predicate);
		}

		/// <summary>
		///   Checks whether or not collection is null or empty. Assumes colleciton can be safely enumerated multiple times.
		/// </summary>
		/// <param name = "this"></param>
		/// <returns></returns>
		public static bool IsNullOrEmpty(this IEnumerable @this)
		{
			return @this == null || @this.GetEnumerator().MoveNext() == false;
		}

		/// <summary>
		///   Generates a HashCode for the contents for the list. Order of items does not matter.
		/// </summary>
		/// <typeparam name="T">The type of object contained within the list.</typeparam>
		/// <param name="list">The list.</param>
		/// <returns>The generated HashCode.</returns>
		public static int GetContentsHashCode<T>(IList<T> list)
		{
			if (list == null)
			{
				return 0;
			}

			var result = 0;
			for (var i = 0; i < list.Count; i++)
			{
				if (list[i] != null)
				{
					// simply add since order does not matter
					result += list[i].GetHashCode();
				}
			}

			return result;
		}

		/// <summary>
		///   Determines if two lists are equivalent. Equivalent lists have the same number of items and each item is found within the other regardless of respective position within each.
		/// </summary>
		/// <typeparam name="T">The type of object contained within the list.</typeparam>
		/// <param name="listA">The first list.</param>
		/// <param name="listB">The second list.</param>
		/// <returns><c>True</c> if the two lists are equivalent.</returns>
		public static bool AreEquivalent<T>(IList<T> listA, IList<T> listB)
		{
			if (listA == null && listB == null)
			{
				return true;
			}

			if (listA == null || listB == null)
			{
				return false;
			}

			if (listA.Count != listB.Count)
			{
				return false;
			}

			// copy contents to another list so that contents can be removed as they are found,
			// in order to consider duplicates
			var listBAvailableContents = listB.ToList();

			// order is not important, just make sure that each entry in A is also found in B
			for (var i = 0; i < listA.Count; i++)
			{
				var found = false;

				for (var j = 0; j < listBAvailableContents.Count; j++)
				{
					if (Equals(listA[i], listBAvailableContents[j]))
					{
						found = true;
						listBAvailableContents.RemoveAt(j);
						break;
					}
				}

				if (!found)
				{
					return false;
				}
			}

			return true;
		}
	}
}
