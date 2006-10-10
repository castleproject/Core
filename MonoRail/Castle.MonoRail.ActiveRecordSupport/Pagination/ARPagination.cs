 // Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.ActiveRecordSupport.Pagination
{
	using System;
	using System.Collections;
	using System.Text.RegularExpressions;
	
	using Castle.ActiveRecord.Queries;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Helpers;
	
	using NHibernate.Expression;

	/// <summary>
	/// Pendent
	/// </summary>
	public class ARPaginationHelper : AbstractHelper
	{
		/// <summary>
		/// Paginates using an <see cref="ARPaginableSimpleQuery"/>
		/// </summary>
		public static IPaginatedPage CreatePagination(int pageSize, Type targetType, string hql, params object[] parameters)
		{
			IARPaginableDataSource criteria = new ARPaginableSimpleQuery(targetType, hql, parameters);
			return CreatePagination(pageSize, criteria);
		}

		/// <summary>
		/// Paginates using an <see cref="ARPaginableCriteria"/>
		/// </summary>
		public static IPaginatedPage CreatePagination(int pageSize, Type targetType, params Order[] orders)
		{
			return CreatePagination(pageSize, targetType, orders, null);
		}

		/// <summary>
		/// Paginates using an <see cref="ARPaginableCriteria"/>
		/// </summary>
		public static IPaginatedPage CreatePagination(int pageSize, Type targetType, Order[] orders, params ICriterion[] criterions)
		{
			IARPaginableDataSource criteria = new ARPaginableCriteria(targetType, orders, criterions);
			return CreatePagination(pageSize, criteria);
		}

		/// <summary>
		/// Paginates using the specified <see cref="IARPaginableDataSource"/>.
		/// </summary>
		public static IPaginatedPage CreatePagination(int pageSize, IARPaginableDataSource criteria)
		{
			return new ARPager(pageSize, criteria, ObtainCurrentPage());
		}

#if DOTNET2
		/// <summary>
		/// Paginates using the specified <see cref="SimpleQuery{T}"/>.
		/// </summary>
		/// <param name="pageSize">The page size</param>
		/// <param name="query">The query</param>
		/// <param name="useEnumerate">If true, calls <see cref="SimpleQuery{T}.Enumerate()"/> instead of <see cref="SimpleQuery{T}.Execute()"/>.</param>
		public IPaginatedPage CreatePagination<T>(int pageSize, SimpleQuery<T> query, bool useEnumerate)
		{
			return CreatePagination(pageSize, new SimpleQueryPaginableWrapper<T>(query, useEnumerate));
		}

		private class SimpleQueryPaginableWrapper<T> : IARPaginableDataSource
		{
			private readonly SimpleQuery<T> query;
			private readonly bool useEnumerate;

			public SimpleQueryPaginableWrapper(SimpleQuery<T> query, bool useEnumerate)
			{
				this.query = query;
				this.useEnumerate = useEnumerate;
			}

			public int ObtainCount()
			{
				return query.Count();
			}

			public IEnumerable ListAll()
			{
				return query.Execute();
			}

			public IEnumerable Paginate(int pageSize, int currentPage)
			{
				SimpleQuery<T> queryClone = (SimpleQuery<T>) query.Clone();
				queryClone.SetQueryRange((currentPage - 1) * pageSize, pageSize);
				if (useEnumerate)
					return queryClone.Enumerate();
				else
					return queryClone.Execute();
			}
		}
#endif

		private static int ObtainCurrentPage()
		{
			String page = MonoRailHttpHandler.CurrentContext.Request.Params["page"];
			return page == null || Regex.IsMatch(page, "\\D")
				? 1 : Convert.ToInt32(page);
		}
	}

	/// <summary>
	/// Pendent
	/// </summary>
	public class ARPager : AbstractPage
	{
		private IEnumerable enumerable;

		public ARPager(int pageSize, IARPaginableDataSource source, int currentPage)
		{
			int count = source.ObtainCount();
			int startIndex = (pageSize * currentPage) - pageSize;
			int endIndex = Math.Min(startIndex + pageSize, count);

			enumerable = source.Paginate(pageSize, currentPage);

			CalculatePaginationInfo(startIndex, endIndex, count, pageSize, currentPage);
		}

		public override IEnumerator GetEnumerator()
		{
			return enumerable.GetEnumerator();
		}
	}
}