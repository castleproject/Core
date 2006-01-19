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

	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework;

	using NHibernate;

	/// <summary>
	/// Base class for all paginable queries, including custom ones.
	/// </summary>
	/// <remarks>
	/// Extenders should override the <see cref="BuildHQL"/>.
	/// Optionally, the methods <see cref="SetQueryParameters"/>
	/// and <see cref="ExecuteQuery"/> can also be overriden.
	/// </remarks>
	public abstract class AbstractPaginableQuery : ActiveRecordBaseQuery, IARPaginableDataSource
	{
		protected int pageSize, currentPage;

		public AbstractPaginableQuery(Type targetType) : base(targetType)
		{
		}

		/// <summary>
		/// Executes a query to return the record count
		/// </summary>
		public virtual int ObtainCount()
		{
			ISessionFactoryHolder holder = ActiveRecordMediator.GetSessionFactoryHolder();
			ISession session = holder.CreateSession(targetType);

			try
			{
				IQuery query = session.CreateQuery("select count(a) from " + targetType.Name + " a");

				return (int) query.UniqueResult();
			}
			finally
			{
				holder.ReleaseSession(session);
			}
		}

		/// <summary>
		/// Returns the page items.
		/// Actually, the implementation just sets the protected fields
		/// <see cref="pageSize"/> and <see cref="currentPage"/>,
		/// gets an <see cref="ISession" /> from <c>SessionFactoryHolder</c>
		/// and calls <see cref="InternalExecute"/> in order to execute
		/// the custom query and fetch only the page items.
		/// </summary>
		/// <param name="pageSize">The page size</param>
		/// <param name="currentPage">The current page</param>
		/// <returns>The page items</returns>
		public virtual IEnumerable Paginate(int pageSize, int currentPage)
		{
			this.pageSize = pageSize;
			this.currentPage = currentPage;

			ISessionFactoryHolder holder = ActiveRecordMediator.GetSessionFactoryHolder();
			ISession session = holder.CreateSession(targetType);

			try
			{
				return InternalPaginate(session, false);
			}
			finally
			{
				holder.ReleaseSession(session);
			}
		}
		
		public virtual IEnumerable ListAll()
		{
			return (IEnumerable) ActiveRecordMediator.ExecuteQuery(this);
		}

		/// <summary>
		/// The implementation of the <see cref="Execute"/> method,
		/// as required by <see cref="ActiveRecordBaseQuery"/>.
		/// Should not be overriden.
		/// </summary>
		/// <param name="session">The NHibernate Session</param>
		/// <returns>The query results.</returns>
		protected sealed override object InternalExecute(ISession session)
		{
			return InternalPaginate(session, true);
		}
		
		private IEnumerable InternalPaginate(ISession session, bool skipPagination)
		{
			IQuery query = session.CreateQuery(BuildHQL());
			SetQueryParameters(query);

			if (!skipPagination)
			{
				PrepareQueryForPagination(query);
			}

			return ExecuteQuery(query);
		}

		/// <summary>
		/// For internal use only.
		/// </summary>
		private void PrepareQueryForPagination(IQuery query)
		{
			query.SetFirstResult(pageSize * (currentPage-1));
			query.SetMaxResults(pageSize);
		}

		/// <summary>
		/// Should be overriden to return the custom HQL to be ran.
		/// </summary>
		/// <returns>The custom HQL to be ran</returns>
		protected abstract string BuildHQL();
		
		/// <summary>
		/// May be overriden, in order to set custom query parameters.
		/// </summary>
		/// <param name="query">The query</param>
		protected virtual void SetQueryParameters(IQuery query) { }

		/// <summary>
		/// Override to provide a custom query execution.
		/// The default behaviour is to just call <see cref="IQuery.List"/>.
		/// </summary>
		/// <param name="query"></param>
		/// <returns>The query results.</returns>
		protected virtual IEnumerable ExecuteQuery(IQuery query)
		{
			return query.List();
		}
	}
}
