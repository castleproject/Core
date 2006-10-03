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

	using Castle.ActiveRecord.Framework;
	using Castle.ActiveRecord.Queries.Modifiers;

	using NHibernate;
	using NHibernate.Type;

	public enum QueryLanguage
	{
		Hql = 0,
		Sql = 1,
	}
	
	/// <summary>
	/// Base class for all HQL or SQL-based queries.
	/// </summary>
	public class HqlBasedQuery : ActiveRecordBaseQuery
	{
		static readonly Regex 
			rxOrderBy = new Regex(@"\s+order\s+by\s+.*", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase),
			rxNoSelect = new Regex(@"^\s*from\s+", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

		private QueryLanguage queryLanguage;
		private String query;

		public HqlBasedQuery(Type targetType, string query) : this(targetType, QueryLanguage.Hql, query)
		{
		}
		
		public HqlBasedQuery(Type targetType, string query, params object[] positionalParameters) : this(targetType, QueryLanguage.Hql, query, positionalParameters)
		{
		}
		
		public HqlBasedQuery(Type targetType, QueryLanguage queryLanguage, string query) : base(targetType)
		{
			this.query = query;
			this.queryLanguage = queryLanguage;
		}

		public HqlBasedQuery(Type targetType, QueryLanguage queryLanguage, string query, params object[] positionalParameters)
			: this(targetType, queryLanguage, query)
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
			get { return query; }
			set { query = value; }
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
		
		#region AddSqlReturnDefinition
		
		/// <summary>
		/// Adds a SQL query return definition.
		/// See <see cref="NHibernate.ISession.CreateSQLQuery(string,string[],Type[])"/> for more information.
		/// </summary>
		public void AddSqlReturnDefinition(Type returnType, string returnAlias)
		{
			AddModifier(new SqlQueryReturnDefinition(returnType, returnAlias));
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
				ScalarQuery q = new ScalarQuery(Target, PrepareQueryForCount(this.Query));
				
				if (queryModifiers != null)
				{
					foreach (IQueryModifier mod in queryModifiers)
					{
						if (mod is QueryRange) continue;
						
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
		
		protected virtual String PrepareQueryForCount(String countQuery)
		{
			countQuery = rxOrderBy.Replace(countQuery, String.Empty);
			
			if (rxNoSelect.IsMatch(countQuery))
			{
				countQuery = "select count(*) " + countQuery;
			}
			else
			{
				countQuery = "select count(*) from (" + countQuery + ")";
			}
			
			Log.Debug("Query prepared for count: {0}", countQuery);
			
			return countQuery;
		}

		protected override IQuery CreateQuery(ISession session)
		{
			IQuery nhibQuery;
			
			switch (queryLanguage)
			{
				case QueryLanguage.Hql:
					nhibQuery = session.CreateQuery(this.Query);
					break;
				
				case QueryLanguage.Sql:
					ArrayList queryReturnAliases = new ArrayList();
					ArrayList queryReturnTypes = new ArrayList();
					if (queryModifiers != null)
					{
						foreach (IQueryModifier mod in queryModifiers)
						{
							SqlQueryReturnDefinition returnDef = mod as SqlQueryReturnDefinition;
							if (returnDef == null)
								continue;
							
							queryReturnAliases.Add(returnDef.ReturnAlias);
							queryReturnTypes.Add(returnDef.ReturnType);
						}
					}
					nhibQuery = session.CreateSQLQuery(this.Query, 
					                                   (String[]) queryReturnAliases.ToArray(typeof(String)), 
					                                   (Type[]) queryReturnTypes.ToArray(typeof(Type)));
					break;
				
				default:
					throw new ActiveRecordException("Query language not supported: " + queryLanguage);
			}

			ApplyModifiers(nhibQuery);

			return nhibQuery;
		}
	}
}