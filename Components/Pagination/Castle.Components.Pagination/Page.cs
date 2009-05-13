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

	/// <summary>
	/// Represents the sliced data and offers
	/// a few read only properties to create a pagination bar.
	/// </summary>
	[Serializable]
	public class Page : AbstractPage
	{
		private readonly IList slice = new ArrayList();
		private readonly int startIndex;
		private readonly int endIndex;

		/// <summary>
		/// Initializes a new instance of the <see cref="Page"/> class.
		/// </summary>
		/// <param name="currentPageIndex">The desired page index</param>
		/// <param name="pageSize">The desired page size</param>
		/// <param name="total">The total of items in the data source.</param>
		protected Page(int currentPageIndex, int pageSize, int total)
		{
			startIndex = (pageSize * currentPageIndex) - pageSize;
			endIndex = Math.Min(startIndex + pageSize, total);

			CalculatePaginationInfo(startIndex, endIndex, total, pageSize, currentPageIndex);
		}

		/// <summary>
		/// Constructs a Page using the specified parameters
		/// </summary>
		/// <param name="list">The whole set</param>
		/// <param name="currentPageIndex">The desired page index</param>
		/// <param name="pageSize">The desired page size</param>
		public Page(IList list, int currentPageIndex, int pageSize)
			: this(currentPageIndex, pageSize, list.Count)
		{
			CreateSlicedCollection(startIndex, endIndex, list);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Page"/> class.
		/// </summary>
		/// <param name="slice">The sliced list.</param>
		/// <param name="currentPageIndex">The desired page index</param>
		/// <param name="pageSize">The desired page size</param>
		/// <param name="total">The total of items (not in the list, but on the original source).</param>
		public Page(IList slice, int currentPageIndex, int pageSize, int total)
			: this(currentPageIndex, pageSize, total)
		{
			this.slice = slice;
		}

		/// <summary>
		/// Populates the sliced view of the whole set
		/// </summary>
		/// <param name="startIndex">Index to start to</param>
		/// <param name="endIndex">Last index</param>
		/// <param name="list">Source set</param>
		private void CreateSlicedCollection(int startIndex, int endIndex, IList list)
		{
			for(int index = startIndex; index < endIndex; index++)
			{
				slice.Add(list[index]);
			}
		}

		protected override object GetItemAtIndex(int itemIndex)
		{
			return slice[itemIndex - startIndex];
		}

		/// <summary>
		/// Creates an enumerator for the 
		/// sliced set
		/// </summary>
		/// <returns>An enumerator instance</returns>
		protected override IEnumerator GetEnumeratorImplementation()
		{
			return slice.GetEnumerator();
		}
	}
}