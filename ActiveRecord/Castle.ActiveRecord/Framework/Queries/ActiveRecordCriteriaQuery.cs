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

namespace Castle.ActiveRecord.Queries
{
	using System;

	using NHibernate;
	using NHibernate.Criterion;

	/// <summary>
	/// Criteria Query
	/// Note: This query can not be included in a MultiQuery.
	/// the problem is that NHibernate does not have a real CriteriaQuery class
	/// </summary>
	public class ActiveRecordCriteriaQuery : HqlBasedQuery
	{
		// constructors will set EITHER criterias OR detachedCriteria
		private readonly ICriterion[] criterias;
		private readonly DetachedCriteria detachedCriteria;

		/// <summary>
		/// Initializes a new instance of the <see cref="ActiveRecordCriteriaQuery"/> class.
		/// </summary>
		/// <param name="targetType">The target type.</param>
		/// <param name="criterias">Criteria applied to the query</param>
		public ActiveRecordCriteriaQuery(Type targetType, ICriterion[] criterias)
			: base(targetType, string.Empty, null)
		{
			this.criterias = criterias;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ActiveRecordCriteriaQuery"/> class.
		/// </summary>
		/// <param name="targetType">The target type.</param>
		/// <param name="detachedCriteria">Criteria applied to the query</param>
		public ActiveRecordCriteriaQuery(Type targetType, DetachedCriteria detachedCriteria)
			: base(targetType, string.Empty, null)
		{
			this.detachedCriteria = detachedCriteria;
		}

		/// <summary>
		/// Executes the query.
		/// </summary>
		/// <param name="session">The <c>NHibernate</c>'s <see cref="ISession"/></param>
		/// <returns><c>ArrayList</c> as an <c>object</c></returns>
		protected override object InternalExecute(ISession session)
		{
			if (detachedCriteria != null)
			{
				ICriteria criteria = detachedCriteria.GetExecutableCriteria(session);

				return criteria.List();
			}
			else if (criterias != null)
			{
				ICriteria criteria = session.CreateCriteria(RootType);

				CriteriaHelper.AddCriterionToCriteria(criteria, criterias);

				return criteria.List();
			}
			else
			{
				return base.CreateQuery(session).List();
			}
		}
	}
}
