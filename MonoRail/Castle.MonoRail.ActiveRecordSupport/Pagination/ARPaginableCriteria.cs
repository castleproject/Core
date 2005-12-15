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

	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework;

	using NHibernate;
	using NHibernate.Expression;

	/// <summary>
	/// A paginable criteria.
	/// Mimics the <see cref="ActiveRecordMediator.FindAll"/> interface.
	/// </summary>
	public class ARPaginableCriteria : IARPaginable
	{
		Type targetType;
		ICriterion[] criterions;
		Order[] orders;
		
		int pageSize, currentPage;

		public ARPaginableCriteria(Type targetType, Order[] orders, params ICriterion[] criterions)
		{
			this.targetType = targetType;
			this.orders = orders;
			this.criterions = criterions;
		}

		public ARPaginableCriteria(Type targetType, params ICriterion[] criterions)
			: this(targetType, null, criterions) { }

		public ARPaginableCriteria(Type targetType, params Order[] orders)
			: this(targetType, orders, null) { }

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
				ICriteria c = session.CreateCriteria(targetType);
			
				if (criterions != null)
					foreach (ICriterion cc in criterions)
						c.Add(cc);
			
				if (orders != null)
					foreach (Order order in orders)
						c.AddOrder(order);

				if (!skipPagination)
				{
					c.SetFirstResult(pageSize * (currentPage-1));
					c.SetMaxResults(pageSize);
				}

				// return GetResultsArray(targetType, c.List(), false);
				return c.List();
			}
			finally
			{
				holder.ReleaseSession(session);
			}
		}
	}
}
