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
	using System;

	using NHibernate;

	/// <summary>
	/// Represents a SQL query return definition.
	/// See <see cref="NHibernate.ISession.CreateSQLQuery(string,string[],Type[])"/> for more information.
	/// </summary>
	public class SqlQueryReturnDefinition : IQueryModifier
	{
		private readonly Type returnType;
		private readonly String returnAlias;

		public SqlQueryReturnDefinition(Type returnType, String returnAlias)
		{
			if (returnType == null) throw new ArgumentNullException("returnType");
			if (returnAlias == null) throw new ArgumentNullException("returnAlias");

			this.returnType = returnType;
			this.returnAlias = returnAlias;
		}

		public Type ReturnType
		{
			get { return returnType; }
		}

		public String ReturnAlias
		{
			get { return returnAlias; }
		}

		#region "Apply" method
		
		void IQueryModifier.Apply(IQuery query)
		{
			// SqlQueryReturnDefinition are not directly applied to queries.
		}
		
		#endregion
	}
}