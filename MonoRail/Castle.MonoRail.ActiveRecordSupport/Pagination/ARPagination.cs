// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Helpers;

	using NHibernate.Expression;

	public class ARPager : IPager
	{
		int pageSize, currentPage, lastPage = -1;
		IARPaginable source;

		public ARPager(int pageSize, IARPaginable source)
		{
			this.source = source;
			this.pageSize = pageSize;
			this.currentPage = currentPage;

			UpdateUIData();
		}

		protected virtual void UpdateUIData()
		{
			string page = ProcessEngine.CurrentContext.Request.Params["page"];
			currentPage = 
				page == null || Regex.IsMatch(page, "\\D")
				? 1
				: Convert.ToInt32(page);
		}

		#region Pagination Data
		public int CurrentIndex { get { return currentPage; } }
		public int FirstIndex { get { return 1; } }
		public int LastIndex { get { return lastPage; } }
		public int NextIndex { get { return currentPage + 1; } }
		public int PreviousIndex { get { return currentPage - 1; } }
		public bool HasPrevious { get { return currentPage > 1; } }
		public bool HasNext { get { return lastPage == -1 || currentPage < lastPage; } }
		#endregion

		public IEnumerator GetEnumerator()
		{
			IEnumerable en = source.Paginate(pageSize, currentPage);
			if (en == null)
				return null;

			return en.GetEnumerator();
		}

		#region Static Shortcut Methods
		/// <summary>
		/// Paginates using an <see cref="ARPaginableSimpleQuery"/>
		/// </summary>
		public static IPager Paginate(int pageSize, Type targetType, string hql, params object[] parameters)
		{
			IARPaginable q = new ARPaginableSimpleQuery(targetType, hql, parameters);
			return Paginate(pageSize, q);
		}
		
		/// <summary>
		/// Paginates using an <see cref="ARPaginableCriteria"/>
		/// </summary>
		public static IPager Paginate(int pageSize, Type targetType, params Order[] orders)
		{
			return Paginate(pageSize, targetType, orders, null);
		}
		
		/// <summary>
		/// Paginates using an <see cref="ARPaginableCriteria"/>
		/// </summary>
		public static IPager Paginate(int pageSize, Type targetType, Order[] orders, params ICriterion[] criterions)
		{
			IARPaginable q = new ARPaginableCriteria(targetType, orders, criterions);
			return Paginate(pageSize, q);
		}

		/// <summary>
		/// Paginates using the specified <see cref="IARPaginable"/>.
		/// </summary>
		public static IPager Paginate(int pageSize, IARPaginable q)
		{
			return new ARPager(pageSize, q);
		}
		#endregion
	}
}
