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
	using System.Collections;
	/// <summary>
	/// Represents a page of a bigger set
	/// </summary>
	/// <remarks>
	/// all indexes are one based
	/// </remarks>
	public interface IPaginatedPage : IEnumerable {

		#region *PageIndex
		/// <summary>
		/// The index this page represents
		/// </summary>
		int CurrentPageIndex { get; }

		/// <summary>
		/// The next page index (from this page)
		/// </summary>
		int NextPageIndex { get; }
		
		/// <summary>
		/// The previous index (from this page)
		/// </summary>
		int PreviousPageIndex { get; }
		#endregion

		#region *ItemIndex
		/// <summary>
		/// The first item index on current page
		/// </summary>
		int FirstItemIndex { get; }

		/// <summary>
		/// The last item index on current page
		/// </summary>
		int LastItemIndex { get; }
		#endregion

		#region *Item
		/// <summary>
		/// Retrieve the first item on current page
		/// </summary>
		object FirstItem { get; }

		/// <summary>
		/// Retrieve the last item on current page
		/// </summary>
		object LastItem { get; }
		#endregion

		#region Has*Page
		/// <summary>
		/// Returns true if current page is not first page
		/// </summary>
		bool HasPreviousPage { get; }
		
		/// <summary>
		/// Returns true if current page is not last page
		/// </summary>
		bool HasNextPage { get; }

		/// <summary>
		/// Checks whether the specified page exists.
		/// Useful for Google-like pagination.
		/// 
		/// </summary>
		/// <returns>true if pageNumber is >= FirstPageIndex</returns>
		/// <returns></returns>
		bool HasPage(int pageNumber);
		#endregion

		/// <summary>
		/// The count of all elements on the set
		/// </summary>
		int TotalItems { get; }

		/// <summary>
		/// Total page count
		/// </summary>
		int TotalPages { get; }

		/// <summary>
		/// Get the requested size of pages
		/// </summary>
		int PageSize { get; }

		/// <summary>
		/// Get the size of current page
		/// </summary>
		int CurrentPageSize { get; }
	}

	/// <summary>
	/// Generic specialization of <see cref="IPaginatedPage"/>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IPaginatedPage<T> : IPaginatedPage, System.Collections.Generic.IEnumerable<T> {

		/// <summary>
		/// Retrieve the first item on current page
		/// </summary>
		new T FirstItem { get; }

		/// <summary>
		/// Retrieve the last item on current page
		/// </summary>
		new T LastItem { get; }
	}
}