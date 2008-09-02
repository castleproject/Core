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

namespace Castle.MonoRail.Framework.Helpers
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using Castle.Components.Pagination;

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
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="PaginationHelper"/> class.
		/// </summary>
		public PaginationHelper() { }
		/// <summary>
		/// Initializes a new instance of the <see cref="PaginationHelper"/> class.
		/// setting the Controller, Context and ControllerContext.
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		public PaginationHelper(IEngineContext engineContext) : base(engineContext) { }
		#endregion

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
		public static IPaginatedPage<T> CreatePagination<T>(IEngineContext engineContext,
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
		public static IPaginatedPage<T> CreatePagination<T>(ICollection<T> datasource,
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
		public static IPaginatedPage<T> CreateCustomPage<T>(IEnumerable<T> datasource, int pageSize, int currentPage, int total)
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


}
