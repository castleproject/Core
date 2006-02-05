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

namespace Castle.ActiveRecord
{
	using System;
	using System.Collections;

	using NHibernate;
	using NHibernate.Expression;

	using Castle.ActiveRecord.Framework;


	/// <summary>
	/// Allow programmers to use the 
	/// ActiveRecord functionality without direct reference
	/// to <see cref="ActiveRecordBase"/>
	/// </summary>
	public static class ActiveRecordMediator<T> where T : class
	{
		/// <summary>
		/// Invokes the specified delegate passing a valid 
		/// NHibernate session. Used for custom NHibernate queries.
		/// </summary>
		/// <param name="call">The delegate instance</param>
		/// <param name="instance">The ActiveRecord instance</param>
		/// <returns>Whatever is returned by the delegate invocation</returns>
		public static object Execute(NHibernateDelegate call, T instance)
		{
			return ActiveRecordBase.Execute(typeof(T), call, instance);
		}

		/// <summary>
		/// Finds an object instance by a unique ID
		/// </summary>
		/// <param name="id">ID value</param>
		/// <param name="throwOnNotFound"><c>true</c> if you want to catch an exception 
		/// if the object is not found</param>
		/// <returns></returns>
		/// <exception cref="ObjectNotFoundException">if <c>throwOnNotFound</c> is set to 
		/// <c>true</c> and the row is not found</exception>
		public static T FindByPrimaryKey(object id, bool throwOnNotFound)
		{
			return (T) ActiveRecordBase.FindByPrimaryKey(typeof(T), id, throwOnNotFound);
		}

		/// <summary>
		/// Finds an object instance by a unique ID
		/// </summary>
		/// <param name="id">ID value</param>
		/// <returns></returns>
		public static T FindByPrimaryKey(object id)
		{
			return FindByPrimaryKey(id, true);
		}

		/// <summary>
		/// Returns all instances found for the specified type.
		/// </summary>
		/// <returns></returns>
		public static T[] FindAll()
		{
			return FindAll((Order[]) null);
		}

		/// <summary>
		/// Returns a portion of the query results (sliced)
		/// </summary>
		public static T[] SlicedFindAll(int firstResult, int maxresults, Order[] orders, params ICriterion[] criterias)
		{
			return (T[]) ActiveRecordBase.SlicedFindAll(typeof(T), firstResult, maxresults, orders, criterias);
		}

		/// <summary>
		/// Returns a portion of the query results (sliced)
		/// </summary>
		public static T[] SlicedFindAll(int firstResult, int maxresults, params ICriterion[] criterias)
		{
			return SlicedFindAll(firstResult, maxresults, null, criterias);
		}

		/// <summary>
		/// Returns all instances found for the specified type 
		/// using sort orders and criterias.
		/// </summary>
		/// <param name="orders"></param>
		/// <param name="criterias"></param>
		/// <returns></returns>
		public static T[] FindAll(Order[] orders, params ICriterion[] criterias)
		{
			return (T[]) ActiveRecordBase.FindAll(typeof(T), orders, criterias);
		}

		/// <summary>
		/// Returns all instances found for the specified type 
		/// using criterias.
		/// </summary>
		/// <param name="criterias"></param>
		/// <returns></returns>
		public static T[] FindAll(params ICriterion[] criterias)
		{
			return FindAll(null, criterias);
		}

		public static void DeleteAll()
		{
			ActiveRecordBase.DeleteAll(typeof(T));
		}

		public static void DeleteAll(string where)
		{
			ActiveRecordBase.DeleteAll(typeof(T), where);
		}

		public static object ExecuteQuery(IActiveRecordQuery q)
		{
			return ActiveRecordBase.ExecuteQuery(q);
		}

		public static R ExecuteQuery2<R>(IActiveRecordQuery<R> query)
		{
			return ActiveRecordBase<T>.ExecuteQuery2(query);
		}
		
		/// <summary>
		/// Saves the instance to the database
		/// </summary>
		/// <param name="instance"></param>
		public static void Save(T instance)
		{
			ActiveRecordBase.Save(instance);
		}

		/// <summary>
		/// Creates (Saves) a new instance to the database.
		/// </summary>
		/// <param name="instance"></param>
		public static void Create(T instance)
		{
			ActiveRecordBase.Create(instance);
		}

		/// <summary>
		/// Persists the modification on the instance
		/// state to the database.
		/// </summary>
		/// <param name="instance"></param>
		public static void Update(T instance)
		{
			ActiveRecordBase.Update(instance);
		}

		/// <summary>
		/// Deletes the instance from the database.
		/// </summary>
		/// <param name="instance"></param>
		public static void Delete(T instance)
		{
			ActiveRecordBase.Delete(instance);
		}

		/// <summary>
		/// Testing hock only.
		/// </summary>
		public static ISessionFactoryHolder GetSessionFactoryHolder()
		{
			return ActiveRecordBase.holder;
		}

	}
}
#endif 