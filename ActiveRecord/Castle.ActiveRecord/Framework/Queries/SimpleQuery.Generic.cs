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
	using System.Collections;
	using System.Collections.Generic;

	using Castle.ActiveRecord.Framework;
	
	using NHibernate;

	/// <summary>
	/// Represents a query that can result in an array of 
	/// objects of the type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The resulting object type</typeparam>
	public class SimpleQuery<T> : HqlBasedQuery, IActiveRecordQuery<T[]>
	{
		#region Constructors

		/// <summary>
		/// Creates a new <c>SimpleQuery</c> for the giving <paramref name="hql"/>,
		/// using the specified positional <paramref name="positionalParameters"/>.
		/// The target ActiveRecord type is <typeparamref name="T"/>.
		/// </summary>
		/// <param name="hql">The HQL</param>
		/// <param name="positionalParameters">The positional positionalParameters</param>
		public SimpleQuery(String hql, params Object[] positionalParameters)
			: base(typeof(T), hql, positionalParameters)
		{
		}

		/// <summary>
		/// Creates a new <c>SimpleQuery</c> for the giving <paramref name="hql"/>,
		/// using the specified positional <paramref name="positionalParameters"/> and
		/// the target ActiveRecord type specified in <paramref name="targetType"/>.
		/// </summary>
		/// <param name="targetType">The target ActiveRecord type</param>
		/// <param name="hql">The HQL</param>
		/// <param name="positionalParameters">The positional positionalParameters</param>
		public SimpleQuery(Type targetType, String hql, params Object[] positionalParameters)
			: base(targetType, hql, positionalParameters)
		{
		}

		#endregion

		#region IActiveRecordQuery<T[]> implementation

		T[] IActiveRecordQuery<T[]>.Execute(ISession session)
		{
			return (T[]) InternalExecute(session);
		}

		#endregion

		#region Public "Execute" and "Enumerate" Methods

		/// <summary>
		/// Executes the query and gets the results.
		/// </summary>
		public T[] Execute()
		{
			return ActiveRecordBase<T>.ExecuteQuery2<T[]>(this);
		}

		/// <summary>
		/// Enumerates the query results. Better suited for queries 
		/// which might return large results.
		/// <seealso cref="IQuery.Enumerable" />
		/// </summary>
		/// <remarks>
		/// It might not look obvious at first, but 
		/// <see cref="ActiveRecordMediator"/> will call our 
		/// <see cref="InternalEnumerate"/>, which will call our 
		/// <see cref="GenericEnumerate"/>, which will convert
		/// the <c>NHibernate</c>'s <see cref="IQuery.Enumerable"/> result
		/// returned by <see cref="ActiveRecordBaseQuery.InternalEnumerate"/>
		/// into a generic <see cref="IEnumerable{T}"/>.
		/// So, all we need to do is to cast it back to <see cref="IEnumerable{T}"/>.
		/// </remarks>
		public IEnumerable<T> Enumerate()
		{
			return (IEnumerable<T>) ActiveRecordMediator.EnumerateQuery(this);
		}

		#endregion

		protected override IEnumerable InternalEnumerate(ISession session)
		{
			return GenericEnumerate(session);
		}

		private IEnumerable<T> GenericEnumerate(ISession session)
		{
			IEnumerable en = InternalEnumerateFromBase(session);
			foreach(T item in en)
				yield return item;
		}

		/// <summary>
		/// Needed to avoid <c>CS1911</c>.
		/// </summary>
		private IEnumerable InternalEnumerateFromBase(ISession session)
		{
			return base.InternalEnumerate(session);
		}

		/// <summary>
		/// Executes the query and converts the results into a strongly-typed
		/// array of <typeparamref name="T"/>.
		/// </summary>
		/// <param name="session">The <c>NHibernate</c>'s <see cref="ISession"/></param>
		protected override object InternalExecute(ISession session)
		{
			IList results = (IList) base.InternalExecute(session);
			return SupportingUtils.BuildArray<T>(results, false);
		}
	}
}

#endif