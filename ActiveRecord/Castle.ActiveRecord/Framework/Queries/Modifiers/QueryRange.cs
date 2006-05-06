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

		public QueryRange(int firstResult, int maxResults)
		{
			this.firstResult = firstResult;
			this.maxResults = maxResults;
		}

		public QueryRange(int maxResults)
			: this(0, maxResults)
		{
		}

		public int FirstResult
		{
			get { return firstResult; }
		}

		public int MaxResults
		{
			get { return maxResults; }
		}

		public void Apply(IQuery query)
		{
			query.SetFirstResult(FirstResult);
			query.SetMaxResults(MaxResults);
		}
	}
}