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
	using System.Collections.Generic;

	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework;

	using NHibernate;

	//TODO: Test CountQuery and CriteriaQuery
	//I (the author) suspect that this implementation will fail for CriteriaBased queries like CountQuery and CriteriaQuery

	/// <summary>
	/// wrapper for an IMultiQuery that executes a collection of queries.
	/// </summary>
	public class ActiveRecordMultiQuery : IActiveRecordQuery
	{
		private readonly Type _rootType;
		private readonly List<ActiveRecordBaseQuery> _queryList = new List<ActiveRecordBaseQuery>();

		/// <summary>
		/// Initializes a new instance of the <see cref="ActiveRecordMultiQuery"/> class.
		/// </summary>
		/// <param name="RootType">the root type for all of the queries that will be included in the <c>IMultiQuery</c></param>
		public ActiveRecordMultiQuery(Type RootType)
		{
			_rootType = RootType;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ActiveRecordMultiQuery"/> class.
		/// </summary>
		/// <param name="RootType">the root type for all of the queries that will be included in the <c>IMultiQuery</c></param>
		/// <param name="activeRecordQueries">an array of <c>IActiveRecordQuery</c></param>
		public ActiveRecordMultiQuery(Type RootType, ActiveRecordBaseQuery[] activeRecordQueries)
			: this(RootType)
		{
			_queryList.AddRange(activeRecordQueries);
		}

		/// <summary>
		/// Add an <c>IActiveRecordQuery</c> to our <see cref="ActiveRecordMultiQuery"/>
		/// </summary>
		/// <param name="activeRecordQuery"><c>IActiveRecordQuery</c> to be added to the MultiQuery</param>
		public void Add(ActiveRecordBaseQuery activeRecordQuery)
		{
			_queryList.Add(activeRecordQuery);
		}

		#region IActiveRecordQuery Members

		/// <summary>
		/// Gets the target type of this query
		/// </summary>
		public Type RootType
		{
			get { return _rootType; }
		}

		/// <summary>
		/// Executes the specified query and return the results
		/// </summary>
		/// <param name="session">The session to execute the query in.</param>
		/// <returns>an array of results, one for each query added</returns>
		public object Execute(ISession session)
		{
			// create a multi-query
			IMultiQuery multiQuery = session.CreateMultiQuery();
			foreach (ActiveRecordBaseQuery arQuery in _queryList)
			{
				// rule: if a query implements InternalExecute, it will fail
				// (we are depending on the ability to call CreateQuery() 
				//  and get a self-contained query object)
				if ((arQuery is CountQuery) || (arQuery is ActiveRecordCriteriaQuery))
				{
					throw new ActiveRecordException("Criteria Based queries are not supported!");
				}
				// add the executable IQuery to our multi-query
				arQuery.AddQuery(session, multiQuery);
			}
			// execute multiquery
			object resultSetArray = multiQuery.List();
			return resultSetArray;
		}

		/// <summary>
		/// (Not Implemented!)
		/// Enumerates over the result of the query.
		/// Note: Only use if you expect most of your values to already exist in the second level cache!
		/// </summary>
		public System.Collections.IEnumerable Enumerate(ISession session)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion
	}
}
