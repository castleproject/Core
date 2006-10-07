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

namespace Castle.ActiveRecord.Framework.Queries
{
	using System;
	using System.Collections;
	using NHibernate;
	using NHibernate.Expression;

	/// <summary>
	/// Perform a projection ( aggeregate ) type of query:
	/// avg, max, count(*), etc.
	/// </summary>
	/// <typeparam name="ARType">The type of the entity we are querying</typeparam>
	/// <typeparam name="TResult">The type of the result from this query</typeparam>
	/// <example>
	/// <code>
	/// ProjectionQuery&lt;Blog, int&gt; proj = new ProjectionQuery&lt;Blog, int&gt;(Projections.RowCount());
	/// int rowCount = proj.Execute();
	/// </code>
	/// </example>
	public class ProjectionQuery<ARType,TResult> : IActiveRecordQuery
	{
		private readonly IProjection projection;
		private readonly ICriterion[] criterions;

		/// <summary>
		/// Gets the target type of this query
		/// </summary>
		/// <value></value>
		public Type Target
		{
			get { return typeof(ARType); }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ProjectionQuery&lt;ARType, TResult&gt;"/> class.
		/// </summary>
		/// <param name="projection">The projection.</param>
		/// <param name="criterions">The criterions.</param>
		public ProjectionQuery(IProjection projection, params ICriterion[] criterions)
		{
			this.projection = projection;
			this.criterions = criterions;
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
			ICriteria criteria = session.CreateCriteria(this.Target);
			foreach (ICriterion criterion in criterions)
			{
				criteria.Add(criterion);
			}
			//TODO: Next time NHibernate is updated, this should be changed to SetProjection()
			criteria.Projection = (projection);
			return (TResult)criteria.UniqueResult();
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