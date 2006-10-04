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

namespace Castle.ActiveRecord.Queries
{
	using System;

	using NHibernate;

	/// <summary>
	/// Represents a query that can result in a value
	/// of the type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The resulting object type</typeparam>
	public class ScalarQuery<T> : ScalarQuery, IActiveRecordQuery<T>
	{
		#region Constructors
		/// <summary>
		/// Creates a new <c>ScalarQuery</c> for the giving <paramref name="query"/>,
		/// using the specified positional <paramref name="positionalParameters"/> and
		/// the target ActiveRecord type specified in <paramref name="targetType"/>.
		/// </summary>
		/// <param name="targetType">The target ActiveRecord type</param>
		/// <param name="query">The query</param>
		/// <param name="positionalParameters">The positional positionalParameters</param>
		public ScalarQuery(Type targetType, String query, params Object[] positionalParameters)
			: base(targetType, query, positionalParameters)
		{
		}

		/// <summary>
		/// Creates a new <c>ScalarQuery</c> for the giving <paramref name="query"/> and
		/// the target ActiveRecord type specified in <paramref name="targetType"/>.
		/// </summary>
		/// <param name="targetType">The target ActiveRecord type</param>
		/// <param name="query">The query</param>
		public ScalarQuery(Type targetType, String query)
			: base(targetType, query)
		{
		}

		/// <summary>
		/// Creates a new <c>ScalarQuery</c> for the giving <paramref name="query"/>,
		/// using the specified positional <paramref name="positionalParameters"/> and
		/// the target ActiveRecord type specified in <paramref name="targetType"/>.
		/// </summary>
		/// <param name="targetType">The target ActiveRecord type</param>
		/// <param name="queryLanguage">The language of the query</param>
		/// <param name="query">The query</param>
		/// <param name="positionalParameters">The positional positionalParameters</param>
		public ScalarQuery(Type targetType, QueryLanguage queryLanguage, String query, params Object[] positionalParameters)
			: base(targetType, queryLanguage, query, positionalParameters)
		{
		}

		/// <summary>
		/// Creates a new <c>ScalarQuery</c> for the giving <paramref name="query"/> and
		/// the target ActiveRecord type specified in <paramref name="targetType"/>.
		/// </summary>
		/// <param name="targetType">The target ActiveRecord type</param>
		/// <param name="queryLanguage">The language of the query</param>
		/// <param name="query">The query</param>
		public ScalarQuery(Type targetType, QueryLanguage queryLanguage, String query)
			: base(targetType, queryLanguage, query)
		{
		}
		#endregion

		#region IActiveRecordQuery<T> implementation
		T IActiveRecordQuery<T>.Execute(ISession session)
		{
			return (T) InternalExecute(session);
		}
		#endregion

		/// <summary>
		/// Executes the query and gets the result.
		/// </summary>
		public T Execute()
		{
			return (T) ActiveRecordMediator.ExecuteQuery(this);
		}
	}
}

#endif