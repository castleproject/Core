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

	using Castle.ActiveRecord.Queries;

	using NHibernate;

	/// <summary>
	/// Performs a simple query and paginate the results.
	/// </summary>
	/// <remarks>
	/// There's no need to supply a <c>returnType</c>, like in
	/// <see cref="SimpleQuery"/>, as we do not perform the
	/// conversion of the query results to an array.
	/// </remarks>
	public class ARPaginableSimpleQuery : ARPaginableQuery
	{
		string hql;
		object[] parameters;

		public ARPaginableSimpleQuery(Type targetType, string hql, params object[] parameters)
			: base(targetType)
		{
			this.hql = hql;
			this.parameters = parameters;
		}

		protected override string BuildHQL()
		{
			return hql;
		}

		protected override void SetQueryParameters(IQuery q)
		{
			int i = 0;
			foreach (object p in parameters)
				q.SetParameter(i++, p);
		}
	}
}