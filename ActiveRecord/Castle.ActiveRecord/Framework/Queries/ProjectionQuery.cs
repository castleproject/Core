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

	public class ProjectionQuery<ARType,TResult> : IActiveRecordQuery
	{
		private readonly IProjection projection;
		private readonly ICriterion[] criterions;

		public Type Target
		{
			get { return typeof(ARType); }
		}

		public ProjectionQuery(IProjection projection, params ICriterion[] criterions)
		{
			this.projection = projection;
			this.criterions = criterions;
		}

		object IActiveRecordQuery.Execute(ISession session)
		{
			return this.Execute(session);
		}

		public IEnumerable Enumerate(ISession session)
		{
			yield return Execute(session);
		}

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

		public TResult Execute()
		{
			return (TResult)ActiveRecordMediator.ExecuteQuery(this);
		}
	}
}

#endif