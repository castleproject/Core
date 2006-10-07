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

	using NHibernate;

	/// <summary>
	/// Query the database for a count (using COUNT(*) ) of all the entites of the specified type.
	/// Optionally using a where clause;
	/// </summary>
	public class CountQuery : HqlBasedQuery
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CountQuery"/> class.
		/// </summary>
		/// <param name="targetType">Type of the target.</param>
		/// <param name="filter">The filter.</param>
		/// <param name="parameters">The parameters.</param>
		public CountQuery(Type targetType, string filter, params object[] parameters)
			: base(targetType,  "SELECT COUNT(*) FROM " + targetType.Name + " WHERE " + filter, parameters) 
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CountQuery"/> class.
		/// </summary>
		/// <param name="targetType">Type of the target.</param>
		public CountQuery(Type targetType) : this(targetType, "1=1", null) 
		{
		}

		/// <summary>
		/// Simply creates the query and then call its <see cref="IQuery.List()"/> method.
		/// </summary>
		/// <param name="session">The <c>NHibernate</c>'s <see cref="ISession"/></param>
		/// <returns></returns>
		protected override object InternalExecute(ISession session)
		{
			IQuery q = base.CreateQuery(session);

			IList result = q.List();
				
			return (int) result[0];
		}
	}
}
