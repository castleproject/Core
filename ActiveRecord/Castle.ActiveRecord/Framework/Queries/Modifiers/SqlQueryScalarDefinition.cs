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

namespace Castle.ActiveRecord.Queries.Modifiers
{
	using System;

	using NHibernate;
	using NHibernate.Type;

	/// <summary>
	/// Represents a SQL query scalar definition.
	/// See <see cref="NHibernate.ISession.CreateSQLQuery(string,string[],Type[])"/> for more information.
	/// </summary>
	public class SqlQueryScalarDefinition : IQueryModifier
	{
		private readonly IType scalarType;
		private readonly String columnAlias;

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlQueryScalarDefinition"/> class.
		/// </summary>
		/// <param name="scalarType">The scalar type.</param>
		/// <param name="columnAlias">The column alias.</param>
		public SqlQueryScalarDefinition(IType scalarType, String columnAlias)
		{
			if (scalarType == null) throw new ArgumentNullException("scalarType");
			if (columnAlias == null) throw new ArgumentNullException("columnAlias");

			this.scalarType = scalarType;
			this.columnAlias = columnAlias;
		}

		/// <summary>
		/// Gets the scalar type
		/// </summary>
		public IType ScalarType
		{
			get { return scalarType; }
		}

		/// <summary>
		/// Gets the column alias for the scalar
		/// </summary>
		public String ColumnAlias
		{
			get { return columnAlias; }
		}

		#region "Apply" method

		/// <summary>
		/// Applies this modifier to the query.
		/// </summary>s
		/// <param name="query">The query</param>
		void IQueryModifier.Apply(IQuery query)
		{
			ISQLQuery sqlQuery = query as ISQLQuery;

			if (sqlQuery != null)
			{
				sqlQuery.AddScalar(columnAlias, scalarType);
			}
		}
		
		#endregion
	}
}
