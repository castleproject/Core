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
	using System.Text.RegularExpressions;

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
		
		/// <summary>
		/// Tries to obtain the record count for the current query.
		/// </summary>
		/// <returns>The record count for the current query, or <c>-1</c> if failed.</returns>
		public virtual int Count()
		{
			try
			{
				ScalarQuery q = new ScalarQuery(Target, PrepareQueryForCount(Query));
				if (queryModifiers != null)
				{
					foreach (IQueryModifier mod in queryModifiers)
					{
						if (mod is QueryRange)
							continue;
						q.AddModifier(mod);
					}
				}

				return Convert.ToInt32(ActiveRecordMediator.ExecuteQuery(q));
			}
			catch (Exception ex)
			{
				// log the exception and return -1
				Log.Debug("Error while obtaining count. Will return -1 as result.", ex);
				return -1;
			}
		}
		
		static readonly Regex 
			rxOrderBy = new Regex(@"\s+order\s+by\s+.*", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase),
			rxNoSelect = new Regex(@"^\s*from\s+", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
		protected virtual String PrepareQueryForCount(String query)
		{
			query = rxOrderBy.Replace(query, String.Empty);
			if (rxNoSelect.IsMatch(query))
				query = "select count(*) " + query;
			else
				query = "select count(*) from (" + query + ")";
			
			Log.Debug("Query prepared for count: {0}", query);
			
			return query;
		}

		protected override IQuery CreateQuery(ISession session)
		{
			IQuery query = session.CreateQuery(hql);

			ApplyModifiers(query);

			return query;
		}
	}
}