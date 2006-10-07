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

namespace Castle.ActiveRecord.Queries
{
	using System;
	using System.Collections;

	using NHibernate;

	/// <summary>
	/// Query that return a single result
	/// </summary>
	public class ScalarQuery : HqlBasedQuery
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ScalarQuery"/> class.
		/// </summary>
		/// <param name="targetType">Type of the target.</param>
		/// <param name="query">The query.</param>
		/// <param name="positionalParameters">The positional parameters.</param>
		public ScalarQuery(Type targetType, string query, params Object[] positionalParameters)
			: base(targetType, query, positionalParameters)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ScalarQuery"/> class.
		/// </summary>
		/// <param name="targetType">Type of the target.</param>
		/// <param name="query">The query.</param>
		public ScalarQuery(Type targetType, string query)
			: base(targetType, query)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ScalarQuery"/> class.
		/// </summary>
		/// <param name="targetType">Type of the target.</param>
		/// <param name="queryLanguage">The query language.</param>
		/// <param name="query">The query.</param>
		/// <param name="positionalParameters">The positional parameters.</param>
		public ScalarQuery(Type targetType, QueryLanguage queryLanguage, string query, params Object[] positionalParameters)
			: base(targetType, queryLanguage, query, positionalParameters)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ScalarQuery"/> class.
		/// </summary>
		/// <param name="targetType">Type of the target.</param>
		/// <param name="queryLanguage">The query language.</param>
		/// <param name="query">The query.</param>
		public ScalarQuery(Type targetType, QueryLanguage queryLanguage, string query)
			: base(targetType, queryLanguage, query)
		{
		}
		
		#endregion

		/// <summary>
		/// Executes the query and returns its scalar result.
		/// </summary>
		/// <param name="session">The NHibernate's <see cref="ISession"/></param>
		/// <returns>The query's scalar result</returns>
		protected override object InternalExecute(ISession session)
		{
			return CreateQuery(session).UniqueResult();
		}

		/// <summary>
		/// Creates a single-position object array containing 
		/// the query's scalar result.
		/// </summary>
		/// <param name="session">The NHibernate's <see cref="ISession"/></param>
		/// <returns>An <c>object[1]</c> containing the query's scalar result.</returns>
		protected override IEnumerable InternalEnumerate(ISession session)
		{
			return new Object[] { InternalExecute(session) };
		}
	}
}