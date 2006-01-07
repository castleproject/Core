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

#if dotNet2

namespace Castle.ActiveRecord.Queries
{
	using System;

	/// <summary>
	/// Represents a query that can result in an array of 
	/// objects of the type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The resulting object type</typeparam>
	public class SimpleQuery<T> : SimpleQuery
	{
		/// <summary>
		/// Creates a new <c>SimpleQuery</c> for the giving <paramref name="hql"/>,
		/// using the specified positional <paramref name="parameters"/>.
		/// The target ActiveRecord type is <typeparamref name="T"/>.
		/// </summary>
		/// <param name="hql">The HQL</param>
		/// <param name="parameters">The positional parameters</param>
		public SimpleQuery(String hql, params Object[] parameters)
			: base(typeof(T), hql, parameters) { }

		/// <summary>
		/// Creates a new <c>SimpleQuery</c> for the giving <paramref name="hql"/>,
		/// using the specified positional <paramref name="parameters"/> and
		/// the target ActiveRecord type specified in <paramref name="targetType"/>.
		/// </summary>
		/// <param name="targetType">The target ActiveRecord type</param>
		/// <param name="hql">The HQL</param>
		/// <param name="parameters">The positional parameters</param>
		public SimpleQuery(Type targetType, String hql, params Object[] parameters)
			: base(targetType, typeof(T), hql, parameters) { }

		/// <summary>
		/// Executes the query and gets the results.
		/// </summary>
		public T[] Execute()
		{
			return (T[]) ActiveRecordMediator.ExecuteQuery(this);
		}
	}
}

#endif