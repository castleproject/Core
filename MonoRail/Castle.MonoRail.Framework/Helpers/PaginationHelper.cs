// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Helpers
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.Specialized;

	/// <summary>
	/// Used as callback handler to obtain the items 
	/// to be displayed. 
	/// </summary>
	public delegate IList DataObtentionDelegate();

	/// <summary>
	/// This helper allows you to easily paginate through a data source 
	/// -- anything that implements <see cref="IList"/>.
	/// </summary>
	/// 
	/// <remarks>
	/// With the pagination you expose a <see cref="Page"/> instance to your view, 
	/// that can be used to created a very detailed page navigator.
	/// 
	/// <para>
	/// You can use up to three approaches to pagination:
	/// </para>
	/// 
	/// <list type="bullet">
	/// <item>
	///		<term>CreatePagination</term>
	///		<description>Uses a whole data set and creates a <see cref="Page"/> with a slice of it. </description>
	/// </item>
	/// <item>
	///		<term>CreateCachedPagination</term>
	///		<description>Caches the dataset and creates a <see cref="Page"/> with a slice. 
	///		As the cache is shared, you must be very careful on creating a cache key that uniquely represents the 
	///		cached dataset.
	/// </description>
	/// </item>
	/// <item>
	///		<term>CreateCustomPage</term>
	///		<description>
	///		In this case, you are handling the slicing. The <see cref="Page"/> is created with your 
	///		actual dataset and information about total. It calculates the other information based on that.
	///		</description>
	/// </item>
	/// </list>
	/// 
	/// <para>
	/// Performance wise, the best choice is the <see cref="CreateCustomPage(IList,int,int,int)"/>
	/// </para>
	/// </remarks>
	public class PaginationHelper : AbstractHelper
	{
		/// <summary>
		/// The parameter key that the helper looks for on the request
		/// </summary>
		public const string PageParameterName = "page";

		#region CreatePageLink

		/// <summary>
		/// Creates a link to navigate to a specific page
		/// </summary>
		/// <param name="page">Page index</param>
		/// <param name="text">Link text</param>
		/// <returns>An anchor tag</returns>
		public String CreatePageLink(int page, String text)
		{
			return CreatePageLink(page, text, null, null);
		}

		/// <summary>
		/// Creates a link to navigate to a specific page
		/// </summary>
		/// <param name="page">Page index</param>
		/// <param name="text">Link text</param>
		/// <param name="htmlAttributes">Attributes for the anchor tag</param>
		/// <returns>An anchor tag</returns>
		public String CreatePageLink(int page, String text, IDictionary htmlAttributes)
		{
			return CreatePageLink(page, text, htmlAttributes, null);
		}

		/// <summary>
		/// Creates a link to navigate to a specific page
		/// </summary>
		/// <param name="page">Page index</param>
		/// <param name="text">Link text</param>
		/// <param name="htmlAttributes">Attributes for the anchor tag</param>
		/// <param name="queryStringParams">Query string entries for the link</param>
		/// <returns>An anchor tag</returns>
		public String CreatePageLink(int page, String text, IDictionary htmlAttributes, IDictionary queryStringParams)
		{
			String filePath = CurrentContext.Request.FilePath;

			if (queryStringParams == null)
			{
				queryStringParams = new Hashtable();
			}
			else if (queryStringParams.IsReadOnly || queryStringParams.IsFixedSize)
			{
				queryStringParams = new Hashtable(queryStringParams);
			}

			queryStringParams[PageParameterName] = page.ToString();

			return String.Format("<a href=\"{0}?{1}\" {2}>{3}</a>",
			                     filePath, BuildQueryString(queryStringParams), GetAttributes(htmlAttributes), text);
		}

		/// <summary>
		/// Creates a link to navigate to a specific page
		/// </summary>
		/// <param name="page">Page index</param>
		/// <param name="text">Link text</param>
		/// <param name="htmlAttributes">Attributes for the anchor tag</param>
		/// <returns>An anchor tag</returns>
		public String CreatePageLinkWithCurrentQueryString(int page, String text, IDictionary htmlAttributes)
		{
			NameValueCollection queryStringParams = Context.Request.QueryString;
			IDictionary dictionary = null;
			if (queryStringParams != null && queryStringParams.Count > 0)
			{
				dictionary = new Hashtable(queryStringParams.Count);
				foreach(string key in queryStringParams.Keys)
				{
					if (key != null)
					{
						dictionary[key] = queryStringParams.GetValues(key);
					}
				}
			}
			return CreatePageLink(page, text, htmlAttributes, dictionary);
		}

		#endregion

		#region CreatePagination

		/// <summary>
		/// Creates a <see cref="Page"/> which is a sliced view of
		/// the data source
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="datasource">Data source to be used as target of the pagination</param>
		/// <param name="pageSize">Page size</param>
		/// <returns>A <see cref="Page"/> instance</returns>
		public static IPaginatedPage CreatePagination(IEngineContext engineContext, IList datasource, int pageSize)
		{
			return CreatePagination(datasource, pageSize, GetCurrentPageFromRequest(engineContext));
		}

		/// <summary>
		/// Creates a <see cref="Page"/> which is a sliced view of
		/// the data source
		/// </summary>
		/// <param name="datasource">Data source to be used as target of the pagination</param>
		/// <param name="pageSize">Page size</param>
		/// <param name="currentPage">current page index (1 based)</param>
		/// <returns>A <see cref="Page"/> instance</returns>
		public static IPaginatedPage CreatePagination(IList datasource, int pageSize, int currentPage)
		{
			if (currentPage <= 0) currentPage = 1;

			return new Page(datasource, currentPage, pageSize);
		}

		#endregion

		#region CreatePagination<T>

		/// <summary>
		/// Creates a <see cref="Page"/> which is a sliced view of
		/// the data source
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="datasource">Data source to be used as target of the pagination</param>
		/// <param name="pageSize">Page size</param>
		/// <returns>A <see cref="Page"/> instance</returns>
		public static IPaginatedPage CreatePagination<T>(IEngineContext engineContext,
		                                                 ICollection<T> datasource, int pageSize)
		{
			return CreatePagination<T>(datasource, pageSize, GetCurrentPageFromRequest(engineContext));
		}

		/// <summary>
		/// Creates a <see cref="Page"/> which is a sliced view of
		/// the data source
		/// </summary>
		/// <param name="datasource">Data source to be used as target of the pagination</param>
		/// <param name="pageSize">Page size</param>
		/// <param name="currentPage">current page index (1 based)</param>
		/// <returns>A <see cref="Page"/> instance</returns>
		public static IPaginatedPage CreatePagination<T>(ICollection<T> datasource,
		                                                 int pageSize, int currentPage)
		{
			if (currentPage <= 0) currentPage = 1;

			return new GenericPage<T>(datasource, currentPage, pageSize);
		}

		#endregion

		#region CreateCachedPagination

		/// <summary>
		/// Creates a <see cref="Page"/> which is a sliced view of
		/// the data source. This method first looks for the datasource
		/// in the <see cref="System.Web.Caching.Cache"/> and if not found,
		/// it invokes the <c>dataObtentionCallback</c> and caches the result
		/// using the specifed <c>cacheKey</c>
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="cacheKey">Cache key used to query/store the datasource</param>
		/// <param name="pageSize">Page size</param>
		/// <param name="dataObtentionCallback">Callback to be used to populate the cache</param>
		/// <returns>A <see cref="Page"/> instance</returns>
		/// <remarks>
		/// CreateCachedPagination is quite dangerous exactly because the cache is
		/// shared. If the results vary per logged user, then the programmer must
		/// rather pay a lot of attention when generating the cache key.
		/// It is preferable to have caching happen at a lower level of the stack, for example the NHibernate query cache.
		/// </remarks>
		public static IPaginatedPage CreateCachedPagination(IEngineContext engineContext, String cacheKey, int pageSize,
		                                                    DataObtentionDelegate dataObtentionCallback)
		{
			ICacheProvider cacheProvider = engineContext.Services.CacheProvider;
			IList datasource = (IList) cacheProvider.Get(cacheKey);

			if (datasource == null)
			{
				datasource = dataObtentionCallback();

				cacheProvider.Store(cacheKey, datasource);
			}

			return CreatePagination(engineContext, datasource, pageSize);
		}

		#endregion

		#region CreateCustomPage

		/// <summary>
		/// Creates a <see cref="Page"/> which is a sliced view of
		/// the data source
		/// <para>
		/// Assumes that the slicing is managed by the caller.
		/// </para>
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="datasource">Data source to be used as target of the pagination</param>
		/// <param name="pageSize">Page size</param>
		/// <param name="total">The total of items in the datasource</param>
		/// <returns>A <see cref="Page"/> instance</returns>
		public static IPaginatedPage CreateCustomPage(IEngineContext engineContext, IList datasource, int pageSize, int total)
		{
			return CreateCustomPage(datasource, pageSize, GetCurrentPageFromRequest(engineContext), total);
		}

		/// <summary>
		/// Creates a <see cref="Page"/> which is a sliced view of
		/// the data source
		/// <para>
		/// Assumes that the slicing is managed by the caller.
		/// </para>
		/// </summary>
		/// <param name="datasource">Data source to be used as target of the pagination</param>
		/// <param name="pageSize">Page size</param>
		/// <param name="total">The total of items in the datasource</param>
		/// <param name="currentPage">The current page index (1 based).</param>
		/// <returns></returns>
		public static IPaginatedPage CreateCustomPage(IList datasource, int pageSize, int currentPage, int total)
		{
			if (currentPage <= 0) currentPage = 1;

			return new Page(datasource, currentPage, pageSize, total);
		}

		/// <summary>
		/// Creates a <see cref="Page"/> which is a sliced view of
		/// the data source
		/// <para>
		/// Assumes that the slicing is managed by the caller.
		/// </para>
		/// </summary>
		/// <param name="datasource">Data source to be used as target of the pagination</param>
		/// <param name="pageSize">Page size</param>
		/// <param name="total">The total of items in the datasource</param>
		/// <param name="currentPage">The current page index (1 based).</param>
		/// <returns></returns>
		public static IPaginatedPage CreateCustomPage<T>(IEnumerable<T> datasource, int pageSize, int currentPage, int total)
		{
			if (currentPage <= 0) currentPage = 1;

			return new GenericCustomPage<T>(datasource, currentPage, pageSize, total);
		}

		#endregion

		private static int GetCurrentPageFromRequest(IEngineContext engineContext)
		{
			String currentPage = GetParameter(engineContext, PageParameterName);

			int curPage = 1;

			if (currentPage != null && currentPage != String.Empty)
			{
				curPage = Int32.Parse(currentPage);
			}

			return curPage <= 0 ? 1 : curPage;
		}

		private static string GetParameter(IEngineContext engineContext, string parameterName)
		{
			return engineContext.Request.Params[parameterName];
		}
	}

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
		/// <param name="curPage">The desired page index</param>
		/// <param name="pageSize">The desired page size</param>
		/// <param name="total">The total of items in the data source.</param>
		protected Page(int curPage, int pageSize, int total)
		{
			startIndex = (pageSize * curPage) - pageSize;
			endIndex = Math.Min(startIndex + pageSize, total);

			CalculatePaginationInfo(startIndex, endIndex, total, pageSize, curPage);
		}

		/// <summary>
		/// Constructs a Page using the specified parameters
		/// </summary>
		/// <param name="list">The whole set</param>
		/// <param name="curPage">The desired page index</param>
		/// <param name="pageSize">The desired page size</param>
		public Page(IList list, int curPage, int pageSize) : this(curPage, pageSize, list.Count)
		{
			CreateSlicedCollection(startIndex, endIndex, list);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Page"/> class.
		/// </summary>
		/// <param name="slice">The sliced list.</param>
		/// <param name="curPage">The desired page index</param>
		/// <param name="pageSize">The desired page size</param>
		/// <param name="total">The total of items (not in the list, but on the original source).</param>
		public Page(IList slice, int curPage, int pageSize, int total) : this(curPage, pageSize, total)
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

		/// <summary>
		/// Creates an enumerator for the 
		/// sliced set
		/// </summary>
		/// <returns>An enumerator instance</returns>
		public override IEnumerator GetEnumerator()
		{
			return slice.GetEnumerator();
		}
	}

	/// <summary>
	/// Represents the sliced data and offers
	/// a few read only properties to create a pagination bar.
	/// </summary>
	[Serializable]
	public class GenericPage<T> : AbstractPage
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
		public override IEnumerator GetEnumerator()
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
					yield return list[i];
				}
			}
			else
			{
				IEnumerator en = sourceList.GetEnumerator();
				for(int i = 0; i < sliceEnd; i++)
				{
					if (!en.MoveNext())
						yield break;

					if (i < sliceStart)
						continue;

					yield return en.Current;
				}
			}
		}
	}

	/// <summary>
	/// Represents the sliced data and offers
	/// a few read only properties to create a pagination bar.
	/// </summary>
	[Serializable]
	public class GenericCustomPage<T> : AbstractPage, IEnumerable<T>
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
		public override IEnumerator GetEnumerator()
		{
			foreach(T item in sourceList)
			{
				yield return item;
			}
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
		/// </returns>
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			foreach(T item in sourceList)
			{
				yield return item;
			}
		}
	}

	/// <summary>
	/// Abstract implementation of <see cref="IPaginatedPage"/>
	/// which performs the standard calculations on 
	/// <see cref="CalculatePaginationInfo"/>
	/// </summary>
	[Serializable]
	public abstract class AbstractPage : IPaginatedPage
	{
		private int firstItem, lastItem, totalItems, pageSize;
		private int previousIndex, nextIndex, lastIndex, curIndex;
		private bool hasPrev, hasNext, hasFirst, hasLast;

		/// <summary>
		/// Calculate the values of all properties
		/// based on the specified parameters
		/// </summary>
		/// <param name="startIndex">Start index</param>
		/// <param name="endIndex">Last index</param>
		/// <param name="count">Total of elements</param>
		/// <param name="pageSize">Page size</param>
		/// <param name="curPage">This page index</param>
		protected void CalculatePaginationInfo(int startIndex, int endIndex,
		                                       int count, int pageSize, int curPage)
		{
			firstItem = count != 0 ? startIndex + 1 : 0;
			lastItem = endIndex;
			totalItems = count;

			hasPrev = startIndex != 0;
			hasNext = count == -1 || (startIndex + pageSize) < count;
			hasFirst = curPage != 1;
			hasLast = count > curPage * pageSize;

			curIndex = curPage;
			previousIndex = curPage - 1;
			nextIndex = curPage + 1;
			lastIndex = count == -1 ? -1 : count / pageSize;

			this.pageSize = pageSize;

			if (count != -1 && count / (float) pageSize > lastIndex)
			{
				lastIndex++;
			}
		}

		/// <summary>
		/// The first index
		/// </summary>
		public int FirstIndex
		{
			get { return 1; }
		}

		/// <summary>
		/// The index this page represents
		/// </summary>
		public int CurrentIndex
		{
			get { return curIndex; }
		}

		/// <summary>
		/// The last index available on the set
		/// </summary>
		public int LastIndex
		{
			get { return lastIndex; }
		}

		/// <summary>
		/// The previous index (from this page)
		/// </summary>
		public int PreviousIndex
		{
			get { return previousIndex; }
		}

		/// <summary>
		/// The next index (from this page)
		/// </summary>
		public int NextIndex
		{
			get { return nextIndex; }
		}

		/// <summary>
		/// The first element (index + 1)
		/// </summary>
		public int FirstItem
		{
			get { return firstItem; }
		}

		/// <summary>
		/// The last element in the page (count)
		/// </summary>
		public int LastItem
		{
			get { return lastItem; }
		}

		/// <summary>
		/// The count of all elements on the set
		/// </summary>
		public int TotalItems
		{
			get { return totalItems; }
		}

		/// <summary>
		/// Gets the size of the page.
		/// </summary>
		/// <value>The size of the page.</value>
		public int PageSize
		{
			get { return pageSize; }
		}

		/// <summary>
		/// Returns true if a previous page 
		/// is accessible from this page
		/// </summary>
		public bool HasPrevious
		{
			get { return hasPrev; }
		}

		/// <summary>
		/// Returns true if a next page is
		/// accessible from this page
		/// </summary>
		public bool HasNext
		{
			get { return hasNext; }
		}

		/// <summary>
		/// Returns true if a first page 
		/// exists
		/// </summary>
		public bool HasFirst
		{
			get { return hasFirst; }
		}

		/// <summary>
		/// Returns true if a last page 
		/// exists
		/// </summary>
		public bool HasLast
		{
			get { return hasLast; }
		}

		/// <summary>
		/// Checks whether the specified page exists.
		/// Useful for Google-like pagination.
		/// </summary>
		/// <param name="pageNumber">The page number</param>
		public bool HasPage(int pageNumber)
		{
			return pageNumber <= LastIndex;
		}

		/// <summary>
		/// Returns a enumerator for the contents
		/// of this page only (not the whole set)
		/// </summary>
		/// <returns>Enumerator instance</returns>
		public abstract IEnumerator GetEnumerator();
	}
}