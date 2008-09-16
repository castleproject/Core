// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
	using System.Collections.Generic;

	/// <summary>
	/// Represents the sliced data and offers
	/// a few read only properties to create a pagination bar.
	/// </summary>
	[Serializable]
	public class GenericCustomPage<T> : AbstractPage<T>, IEnumerable<T>
	{
		private readonly IEnumerable<T> sourceList;

		/// <summary>
		/// Initializes a new instance of the <see cref="GenericCustomPage&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="list">The list.</param>
		/// <param name="curPage">The cur page.</param>
		/// <param name="pageSize">Size of the page.</param>
		/// <param name="total">The total.</param>
		public GenericCustomPage(IEnumerable<T> list, int curPage, int pageSize, int total)
		{
			int startIndex = (pageSize * curPage) - pageSize;
			int endIndex = Math.Min(startIndex + pageSize, total);

			sourceList = list;

			CalculatePaginationInfo(startIndex, endIndex, total, pageSize, curPage);
		}

		/// <summary>
		/// Returns a enumerator for the contents
		/// of this page only (not the whole set)
		/// </summary>
		/// <returns>Enumerator instance</returns>
		protected override IEnumerator<T> GetGenericEnumeratorImplementation()
		{
			foreach (T item in sourceList)
			{
				yield return item;
			}
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
		/// </returns>
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			foreach (T item in sourceList)
			{
				yield return item;
			}
		}


		protected override T GetGenericItemAtIndex(int itemIndex)
		{
			return PaginationSupport.GetItemAtIndex(sourceList, itemIndex - ((this as IPaginatedPage).FirstItemIndex - 1));
		}

		protected override object GetItemAtIndex(int itemIndex)
		{
			return GetGenericItemAtIndex(itemIndex);
		}
	}
}
