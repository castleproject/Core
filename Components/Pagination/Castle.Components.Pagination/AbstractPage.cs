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
	/// Abstract implementation of <see cref="IPaginatedPage"/>
	/// which performs the standard calculations on 
	/// <see cref="CalculatePaginationInfo"/>
	/// </summary>
	[Serializable]
	public abstract class AbstractPage : IPaginatedPage
	{
		private int firstItemIndex, lastItemIndex, totalItems, pageSize, currentPageSize;
		private int previousPageIndex, nextPageIndex, lastPageIndex, currentPageIndex;
		private bool hasPreviousPage, hasNextPage;

		/// <summary>
		/// Calculate the values of all properties
		/// based on the specified parameters
		/// </summary>
		/// <param name="startIndex">Start index</param>
		/// <param name="endIndex">Last index</param>
		/// <param name="count">Total of elements</param>
		/// <param name="pageSize">Page size</param>
		/// <param name="currentPage">This page index</param>
		protected void CalculatePaginationInfo(int startIndex, int endIndex,
		                                       int count, int pageSize, int currentPage)
		{
			firstItemIndex = count != 0 ? startIndex + 1 : 0;
			lastItemIndex = endIndex;
			totalItems = count;
			currentPageIndex = currentPage;
			previousPageIndex = currentPage - 1;
			nextPageIndex = currentPage + 1;
			lastPageIndex = count == -1 ? -1 : count / pageSize;
			hasPreviousPage = currentPageIndex > 1;
			currentPageSize = count > 0 ? endIndex - startIndex : 0;
			this.pageSize = pageSize;

			if (count != -1 && count / (float) pageSize > lastPageIndex)
			{
				lastPageIndex++;
			}

			hasNextPage = count == -1 || currentPageIndex < lastPageIndex;
		}

		public int CurrentPageIndex
		{
			get { return currentPageIndex; }
		}

		public int PreviousPageIndex
		{
			get { return previousPageIndex; }
		}

		public int NextPageIndex
		{
			get { return nextPageIndex; }
		}

		public int FirstItemIndex
		{
			get { return firstItemIndex; }
		}

		public int LastItemIndex
		{
			get { return lastItemIndex; }
		}

		public int TotalItems
		{
			get { return totalItems; }
		}

		public int PageSize
		{
			get { return pageSize; }
		}

		public bool HasPreviousPage
		{
			get { return hasPreviousPage; }
		}

		public bool HasNextPage
		{
			get { return hasNextPage; }
		}

		public bool HasPage(int pageNumber)
		{
			return pageNumber <= lastPageIndex && pageNumber >= 1;
		}

		public int TotalPages
		{
			get { return lastPageIndex; }
		}

		public int CurrentPageSize
		{
			get { return currentPageSize; }
		}

		/// <summary>
		/// Returns a enumerator for the contents
		/// of this page only (not the whole set)
		/// </summary>
		/// <returns>Enumerator instance</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumeratorImplementation();
		}

		object IPaginatedPage.FirstItem
		{
			get { return GetItemAtIndex(firstItemIndex - 1); }
		}

		object IPaginatedPage.LastItem
		{
			get { return GetItemAtIndex(lastItemIndex - 1); }
		}

		/// <summary>
		/// should be overrided
		/// </summary>
		/// <returns></returns>
		protected abstract object GetItemAtIndex(int itemIndex);

		/// <summary>
		/// should be overrided
		/// </summary>
		/// <returns></returns>
		protected abstract IEnumerator GetEnumeratorImplementation();
	}

	/// <summary>
	/// Generic specialization of <see cref="AbstractPage"/>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class AbstractPage<T> : AbstractPage, IPaginatedPage<T>
	{

        T IPaginatedPage<T>.FirstItem
        {
			get { return (T) ((IPaginatedPage)this).FirstItem; }
        }


        T IPaginatedPage<T>.LastItem
        {
			get { return (T)((IPaginatedPage)this).LastItem; }
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return GetGenericEnumeratorImplementation();
		}

		protected override IEnumerator GetEnumeratorImplementation()
		{
			return this.GetGenericEnumeratorImplementation();
		}

		/// <summary>
		/// return enumerator over the current set (not the whole set)
		/// </summary>
		/// <returns></returns>
		protected abstract IEnumerator<T> GetGenericEnumeratorImplementation();

		/// <summary>
		/// return element at index
		/// </summary>
		/// <returns></returns>
		protected abstract T GetGenericItemAtIndex(int itemIndex);
	}
}