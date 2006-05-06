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

	using Castle.ActiveRecord.Queries.Modifiers;

	using NHibernate;
	using NHibernate.Type;

	/// <summary>
	/// Base class for all HQL-based queries.
	/// </summary>
	public class HqlBasedQuery : ActiveRecordBaseQuery
	{
		private String hql;

		public HqlBasedQuery(Type targetType, string hql)
			: base(targetType)
		{
			this.hql = hql;
		}

		public HqlBasedQuery(Type targetType, string hql, params object[] positionalParameters)
			: this(targetType, hql)
		{
			if (positionalParameters != null && positionalParameters.Length > 0)
			{
				int i = 0;
				foreach (object value in positionalParameters)
					AddModifier(new QueryParameter(i++, value));
			}
		}

		/// <summary>
		/// The query text.
		/// </summary>
		public string Query
		{
			get { return hql; }
			set { hql = value; }
		}

		#region SetParameter and SetParameterList
		public void SetParameter(string parameterName, object value)
		{
			AddModifier(new QueryParameter(parameterName, value));
		}

		public void SetParameter(string parameterName, object value, IType type)
		{
			AddModifier(new QueryParameter(parameterName, value, type));
		}

		public void SetParameterList(string parameterName, ICollection list)
		{
			AddModifier(new QueryParameter(parameterName, list));
		}

		public void SetParameterList(string parameterName, ICollection list, IType type)
		{
			AddModifier(new QueryParameter(parameterName, list, type));
		}
		#endregion

		#region SetQueryRange
		public void SetQueryRange(int firstResult, int maxResults)
		{
			AddModifier(new QueryRange(firstResult, maxResults));
		}

		public void SetQueryRange(int maxResults)
		{
			AddModifier(new QueryRange(maxResults));
		}
		#endregion

		protected override IQuery CreateQuery(ISession session)
		{
			IQuery query = session.CreateQuery(hql);

			ApplyModifiers(query);

			return query;
		}
	}
}