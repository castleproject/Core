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

	/// <summary>
	/// Represents a SQL query join definition.
	/// See <see cref="NHibernate.ISession.CreateSQLQuery(string,string[],Type[])"/> for more information.
	/// </summary>
	public class SqlQueryJoinDefinition : IQueryModifier
	{
		private readonly String associationPath;
		private readonly String associationAlias;

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlQueryJoinDefinition"/> class.
		/// </summary>
		/// <param name="associationPath">The association path.</param>
		/// <param name="associationAlias">The association alias.</param>
		public SqlQueryJoinDefinition(String associationPath, String associationAlias)
		{
			if (associationPath == null) throw new ArgumentNullException("associationPath");
			if (associationAlias == null) throw new ArgumentNullException("associationAlias");

			this.associationPath = associationPath;
			this.associationAlias = associationAlias;
		}

		/// <summary>
		/// Gets the path of the assocation
		/// </summary>
		public String AssociationPath
		{
			get { return associationPath; }
		}

		/// <summary>
		/// Gets the alias for the association
		/// </summary>
		public String AssociationAlias
		{
			get { return associationAlias; }
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
				sqlQuery.AddJoin(associationAlias, associationPath);
			}
		}
		
		#endregion
	}
}
