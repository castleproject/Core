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

#if DOTNET2

namespace Castle.ActiveRecord.Queries
{
	using System;
	using System.Collections;
	using NHibernate;
	using NHibernate.Expression;

	/// <summary>
	/// Perform a scalar projection ( aggeregate ) type of query:
	/// avg, max, count(*), etc.
	/// </summary>
	/// <typeparam name="ARType">The type of the entity we are querying</typeparam>
	/// <typeparam name="TResult">The type of the scalar from this query</typeparam>
	/// <example>
	/// <code>
	/// ScalarProjectionQuery&lt;Blog, int&gt; proj = new ScalarProjectionQuery&lt;Blog, int&gt;(Projections.RowCount());
	/// int rowCount = proj.Execute();
	/// </code>
	/// </example>
	public class ScalarProjectionQuery<ARType, TResult> : IActiveRecordQuery
	{
		private readonly IProjection projection;
		private readonly ICriterion[] criterions;
		private readonly DetachedCriteria detachedCirteria;

		/// <summary>
		/// Gets the target type of this query
		/// </summary>
		/// <value></value>
		public Type Target
		{
			get { return typeof(ARType); }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ScalarProjectionQuery{ARType,TResult}"/> class.
		/// </summary>
		/// <param name="projection">The projection.</param>
		/// <param name="criterions">The criterions.</param>
		public ScalarProjectionQuery(IProjection projection, params ICriterion[] criterions)
		{
			this.projection = projection;
			this.criterions = criterions;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ScalarProjectionQuery{ARType,TResult}"/> class.
		/// </summary>
		/// <param name="projection">The projection.</param>
		/// <param name="criteria">The detachedCirteria.</param>
		public ScalarProjectionQuery(IProjection projection, DetachedCriteria criteria)
		{
			this.projection = projection;
			this.detachedCirteria = criteria;
		}

		/// <summary>
		/// Executes the specified query and return the results
		/// </summary>
		/// <param name="session">The session to execute the query in.</param>
		/// <returns>the result of the query</returns>
		object IActiveRecordQuery.Execute(ISession session)
		{
			return this.Execute(session);
		}

		/// <summary>
		/// Enumerates over the result of the query.
		/// Always returns a single result
		/// </summary>
		public IEnumerable Enumerate(ISession session)
		{
			yield return Execute(session);
		}

		/// <summary>
		/// Executes the specified query and return the results
		/// </summary>
		/// <param name="session">The session to execute the query in.</param>
		/// <returns>the result of the query</returns>
		public TResult Execute(ISession session)
		{
			if (this.detachedCirteria != null)
			{
				return detachedCirteria.GetExecutableCriteria(session)
					.SetProjection(projection)
					.UniqueResult<TResult>();
			}
			else
			{
				ICriteria criteria = session.CreateCriteria(this.Target);
				foreach (ICriterion criterion in criterions)
				{
					criteria.Add(criterion);
				}
				criteria.SetProjection(projection);
				return criteria.UniqueResult<TResult>();
			}
		}

		/// <summary>
		/// Executes the specified query and return the results
		/// </summary>
		/// <returns>the result of the query</returns>
		public TResult Execute()
		{
			return (TResult)ActiveRecordMediator.ExecuteQuery(this);
		}
	}
}

#endif