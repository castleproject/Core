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
	using System.Collections;

	using Castle.ActiveRecord.Framework;
	using Castle.ActiveRecord.Queries.Modifiers;

	using NHibernate;
	using NHibernate.Type;
	using NHibernate.Transform;

	/// <summary>
	/// defines the possible query langauges
	/// </summary>
	public enum QueryLanguage
	{
		/// <summary>
		/// Hibernate Query Language
		/// </summary>
		Hql = 0,
		/// <summary>
		/// Structured Query Language
		/// </summary>
		Sql = 1,
	}

	/// <summary>
	/// Base class for all HQL or SQL-based queries.
	/// </summary>
	public class HqlBasedQuery : ActiveRecordBaseQuery
	{
		private readonly QueryLanguage queryLanguage;
		private String query;

		/// <summary>
		/// Initializes a new instance of the <see cref="HqlBasedQuery"/> class.
		/// </summary>
		/// <param name="targetType">Type of the target.</param>
		/// <param name="query">The query.</param>
		public HqlBasedQuery(Type targetType, string query)
			: this(targetType, QueryLanguage.Hql, query)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HqlBasedQuery"/> class.
		/// </summary>
		/// <param name="targetType">Type of the target.</param>
		/// <param name="query">The query.</param>
		/// <param name="positionalParameters">The positional parameters.</param>
		public HqlBasedQuery(Type targetType, string query, params object[] positionalParameters)
			: this(targetType, QueryLanguage.Hql, query, positionalParameters)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HqlBasedQuery"/> class.
		/// </summary>
		/// <param name="targetType">Type of the target.</param>
		/// <param name="queryLanguage">The query language.</param>
		/// <param name="query">The query.</param>
		public HqlBasedQuery(Type targetType, QueryLanguage queryLanguage, string query)
			: base(targetType)
		{
			this.query = query;
			this.queryLanguage = queryLanguage;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HqlBasedQuery"/> class.
		/// </summary>
		/// <param name="targetType">Type of the target.</param>
		/// <param name="queryLanguage">The query language.</param>
		/// <param name="query">The query.</param>
		/// <param name="positionalParameters">The positional parameters.</param>
		public HqlBasedQuery(Type targetType, QueryLanguage queryLanguage, string query, params object[] positionalParameters)
			: this(targetType, queryLanguage, query)
		{
			if (positionalParameters != null && positionalParameters.Length > 0)
			{
				int i = 0;
				foreach (object value in positionalParameters)
				{
					ValueAndTypeTuple valueAndTypeTuple = ValueAndTypeTuple.Wrap(value);
					AddModifier(new QueryParameter(i++, valueAndTypeTuple.Value, valueAndTypeTuple.Type));
				}
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

		/// <summary>
		/// Sets a parameter with the given name.
		/// </summary>
		/// <param name="parameterName">Name of the parameter.</param>
		/// <param name="value">The value.</param>
		public void SetParameter(string parameterName, object value)
		{
			AddModifier(new QueryParameter(parameterName, value));
		}

		/// <summary>
		/// Sets a parameter with the given name and type
		/// </summary>
		/// <param name="parameterName">Name of the parameter.</param>
		/// <param name="value">The value.</param>
		/// <param name="type">The type.</param>
		public void SetParameter(string parameterName, object value, IType type)
		{
			AddModifier(new QueryParameter(parameterName, value, type));
		}

		/// <summary>
		/// Sets a parameter with the given name with a list of values
		/// </summary>
		/// <param name="parameterName">Name of the parameter.</param>
		/// <param name="list">The list.</param>
		public void SetParameterList(string parameterName, ICollection list)
		{
			AddModifier(new QueryParameter(parameterName, list));
		}

		/// <summary>
		/// Sets a parameter with the given name with a list of values and type
		/// </summary>
		/// <param name="parameterName">Name of the parameter.</param>
		/// <param name="list">The list.</param>
		/// <param name="type">The type.</param>
		public void SetParameterList(string parameterName, ICollection list, IType type)
		{
			AddModifier(new QueryParameter(parameterName, list, type));
		}

		#endregion

		#region SetQueryRange

		/// <summary>
		/// Sets the query range (paging)
		/// </summary>
		/// <param name="firstResult">The first result.</param>
		/// <param name="maxResults">The maximum number of results returned (page size)</param>
		public void SetQueryRange(int firstResult, int maxResults)
		{
			AddModifier(new QueryRange(firstResult, maxResults));
		}

		/// <summary>
		/// Sets the query range (maximum number of items returned)
		/// </summary>
		/// <param name="maxResults">The maximum number of results.</param>
		public void SetQueryRange(int maxResults)
		{
			AddModifier(new QueryRange(maxResults));
		}

		#endregion

		#region SqlQuery Modifiers

		/// <summary>
		/// Adds a SQL query return definition.
		/// See <see cref="NHibernate.ISession.CreateSQLQuery(string,string[],Type[])"/> for more information.
		/// </summary>
		public void AddSqlReturnDefinition(Type returnType, String returnAlias)
		{
			AddModifier(new SqlQueryReturnDefinition(returnType, returnAlias));
		}

		/// <summary>
		/// Adds a SQL query join definition.
		/// See <see cref="NHibernate.ISession.CreateSQLQuery(string,string[],Type[])"/> for more information.
		/// </summary>
		public void AddSqlJoinDefinition(String associationPath, String associationAlias)
		{
			AddModifier(new SqlQueryJoinDefinition(associationPath, associationAlias));
		}

		/// <summary>
		/// Adds a SQL query scalar definition.
		/// See <see cref="NHibernate.ISession.CreateSQLQuery(string,string[],Type[])"/> for more information.
		/// </summary>
		public void AddSqlScalarDefinition(IType scalarType, String columnAlias)
		{
			AddModifier(new SqlQueryScalarDefinition(scalarType, columnAlias));
		}

		#endregion

		#region SetResultTransformer

		/// <summary>
		/// Adds a query result transformer.
		/// See <see cref="IResultTransformer"/> for more information.
		/// </summary>
		public void SetResultTransformer(IResultTransformer transformer)
		{
			AddModifier(new QueryResultTransformer(transformer));
		}

		#endregion

		/// <summary>
		/// Creates the <see cref="IQuery"/> instance.
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		protected override IQuery CreateQuery(ISession session)
		{
			IQuery nhibQuery;

			switch (queryLanguage)
			{
				case QueryLanguage.Hql:
					nhibQuery = session.CreateQuery(Query);
					break;

				case QueryLanguage.Sql:
					nhibQuery = session.CreateSQLQuery(Query);
					break;

				default:
					throw new ActiveRecordException("Query language not supported: " + queryLanguage);
			}

			ApplyModifiers(nhibQuery);

			return nhibQuery;
		}
	}
}
