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

namespace Castle.ActiveRecord.Queries.Modifiers
{
	using NHibernate;

	/// <summary>
	/// Limits a query to the specified results.
	/// </summary>
	public class QueryRange : IQueryModifier
	{
		private readonly int firstResult;
		private readonly int maxResults;

		/// <summary>
		/// Initializes a new instance of the <see cref="QueryRange"/> class.
		/// </summary>
		/// <param name="firstResult">The first result.</param>
		/// <param name="maxResults">The max results.</param>
		public QueryRange(int firstResult, int maxResults)
		{
			this.firstResult = firstResult;
			this.maxResults = maxResults;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QueryRange"/> class.
		/// </summary>
		/// <param name="maxResults">The max results.</param>
		public QueryRange(int maxResults)
			: this(0, maxResults)
		{
		}

		/// <summary>
		/// Gets the first result.
		/// </summary>
		/// <value>The first result.</value>
		public int FirstResult
		{
			get { return firstResult; }
		}

		/// <summary>
		/// Gets the max results.
		/// </summary>
		/// <value>The max results.</value>
		public int MaxResults
		{
			get { return maxResults; }
		}

		/// <summary>
		/// Applies this modifier to the query.
		/// </summary>
		/// <param name="query">The query</param>
		public void Apply(IQuery query)
		{
			query.SetFirstResult(FirstResult);
			query.SetMaxResults(MaxResults);
		}
	}
}