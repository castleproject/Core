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

	/// <summary>
	/// Represents the sliced data and offers
	/// a few read only properties to create a pagination bar.
	/// </summary>
	[Serializable]
	public class GenericPage<T> : AbstractPage<T>
	{
		private readonly int sliceStart, sliceEnd;
		private readonly ICollection<T> sourceList;

		/// <summary>
		/// Initializes a new instance of the <see cref="GenericPage&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="list">The list.</param>
		/// <param name="curPage">The cur page.</param>
		/// <param name="pageSize">Size of the page.</param>
		public GenericPage(ICollection<T> list, int curPage, int pageSize)
		{
			// Calculate slice indexes
			int startIndex = sliceStart = (pageSize * curPage) - pageSize;
			int endIndex = sliceEnd = Math.Min(startIndex + pageSize, list.Count);

			sourceList = list;

			CalculatePaginationInfo(startIndex, endIndex, list.Count, pageSize, curPage);
		}

		/// <summary>
		/// Returns a enumerator for the contents
		/// of this page only (not the whole set)
		/// </summary>
		/// <returns>Enumerator instance</returns>
		protected override IEnumerator<T> GetGenericEnumeratorImplementation()
		{
			if (sourceList is IList<T>)
			{
				IList<T> list = (IList<T>) sourceList;
				for(int i = sliceStart; i < sliceEnd; i++)
				{
					yield return list[i];
				}
			}
			else if (sourceList is IList)
			{
				IList list = (IList) sourceList;
				for(int i = sliceStart; i < sliceEnd; i++)
				{
					yield return (T) list[i];
				}
			}
			else
			{
				IEnumerator<T> en = sourceList.GetEnumerator();
				for(int i = 0; i < sliceEnd; i++)
				{
					if (!en.MoveNext())
					{
						yield break;
					}

					if (i < sliceStart)
					{
						continue;
					}

					yield return en.Current;
				}
			}
		}

		protected override T GetGenericItemAtIndex(int itemIndex)
		{
			return new List<T>(sourceList)[itemIndex];
		}


		protected override object GetItemAtIndex(int itemIndex)
		{
			return GetGenericItemAtIndex(itemIndex);
		}
	}
}