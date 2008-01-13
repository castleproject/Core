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

namespace Castle.MonoRail.ActiveRecordSupport.Pagination
{
	using System;
	using System.Collections;

	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework;

	using NHibernate;
	using NHibernate.Expressions;

	/// <summary>
	/// A paginable criteria.
	/// Mimics the <see cref="ActiveRecordMediator.FindAll(Type)"/> interface.
	/// </summary>
	public class ARPaginableCriteria : IARPaginableDataSource
	{
		private Type targetType;
		private ICriterion[] criterions;
		private Order[] orders;
		private DetachedCriteria detachedCriteria;

		private ICriteria cachedCriteria;
		
		private int pageSize, currentPage;

		public ARPaginableCriteria(Type targetType, Order[] orders, params ICriterion[] criterions)
		{
			this.targetType = targetType;
			this.orders = orders;
			this.criterions = criterions;
		}

		public ARPaginableCriteria(Type targetType, params ICriterion[] criterions) : this(targetType, null, criterions) { }

		public ARPaginableCriteria(Type targetType, params Order[] orders) : this(targetType, orders, null) { }

		public ARPaginableCriteria(Type targetType, DetachedCriteria detachedCriteria) : this(targetType, null, (ICriterion[])null)
		{
			this.detachedCriteria = detachedCriteria;
		}

		protected virtual ICriteria BuildCriteria(ISession session)
		{
			if (cachedCriteria == null)
			{
				if (detachedCriteria != null)
				{
					cachedCriteria = detachedCriteria.GetExecutableCriteria(session);
				}
				else
				{
					cachedCriteria = session.CreateCriteria(targetType);

					if (criterions != null)
					{
						foreach (ICriterion queryCriteria in criterions)
						{
							cachedCriteria.Add(queryCriteria);
						}
					}
				}

				if (orders != null)
				{
					foreach (Order order in orders)
					{
						cachedCriteria.AddOrder(order);
					}
				}
			}

			return cachedCriteria;
		}

		/// <summary>
		/// Implementors should execute a query
		/// to return the record count
		/// </summary>
		/// <remarks>
		/// This needs a performance boost. Couldn't think of a better
		/// way of get the count.
		/// </remarks>
		public virtual int ObtainCount()
		{
			ISessionFactoryHolder holder = ActiveRecordMediator.GetSessionFactoryHolder();
			ISession session = holder.CreateSession(targetType);

			try
			{
				ICriteria criteria = BuildCriteria(session);

				return criteria.List().Count;
			}
			finally
			{
				holder.ReleaseSession(session);
			}
		}

		public IEnumerable Paginate(int pageSize, int currentPage)
		{
			this.pageSize = pageSize;
			this.currentPage = currentPage;
			return InternalExecute(false);
		}
		
		public IEnumerable ListAll()
		{
			return InternalExecute(true);
		}
		
		private IEnumerable InternalExecute(bool skipPagination)
		{
			ISessionFactoryHolder holder = ActiveRecordMediator.GetSessionFactoryHolder();
			ISession session = holder.CreateSession(targetType);

			try
			{
				ICriteria criteria = BuildCriteria(session);

				if (!skipPagination)
				{
					criteria.SetFirstResult(pageSize * (currentPage-1));
					criteria.SetMaxResults(pageSize);
				}

				return criteria.List();
			}
			finally
			{
				holder.ReleaseSession(session);
			}
		}
	}
}
