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

namespace Castle.ActiveRecord
{
	using NHibernate.Expression;

	/// <summary>
	/// Allow programmers to use the 
	/// ActiveRecord functionality without extending <see cref="ActiveRecordBase"/>
	/// </summary>
	public class ActiveRecordMediator<T> : ActiveRecordMediator where T : class
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
			return ActiveRecordMediator.Execute(typeof(T), call, instance);
		}

		/// <summary>
		/// Finds an object instance by its primary key.
		/// </summary>
		/// <param name="id">ID value</param>
		/// <param name="throwOnNotFound"><c>true</c> if you want an exception to be thrown
		/// if the object is not found</param>
		/// <exception cref="ObjectNotFoundException">if <c>throwOnNotFound</c> is set to 
		/// <c>true</c> and the row is not found</exception>
		public static T FindByPrimaryKey(object id, bool throwIfNotFound)
		{
			return (T) ActiveRecordMediator.FindByPrimaryKey(typeof(T), id, throwIfNotFound);
		}

		/// <summary>
		/// Finds an object instance by its primary key.
		/// </summary>
		/// <param name="id">ID value</param>
		public static T FindByPrimaryKey(object id)
		{
			return (T) ActiveRecordMediator.FindByPrimaryKey(typeof(T), id, true);
		}

		/// <summary>
		/// Searches and returns the first row.
		/// </summary>
		/// <param name="orders">The sort order - used to determine which record is the first one</param>
		/// <param name="criterias">The criteria expression</param>
		/// <returns>A <c>targetType</c> instance or <c>null</c></returns>
		public static T FindFirst(Order[] orders, params ICriterion[] criterias)
		{
			return (T) ActiveRecordMediator.FindFirst(typeof(T), orders, criterias);
		}

		/// <summary>
		/// Searches and returns the first row.
		/// </summary>
		/// <param name="criterias">The criteria expression</param>
		/// <returns>A <c>targetType</c> instance or <c>null</c></returns>
		public static T FindFirst(params ICriterion[] criterias)
		{
			return (T) ActiveRecordMediator.FindFirst(typeof(T), criterias);
		}

		/// <summary>
		/// Searches and returns the first row.
		/// </summary>
		/// <param name="criterias">The criterias.</param>
		/// <returns>A instance the targetType or <c>null</c></returns>
		public static T FindOne(params ICriterion[] criterias)
		{
			return (T) ActiveRecordMediator.FindOne(typeof(T), criterias);
		}

		/// <summary>
		/// Returns all instances found for the specified type.
		/// </summary>
		/// <returns></returns>
		public static T[] FindAll()
		{
			return (T[]) ActiveRecordMediator.FindAll(typeof(T));
		}

		/// <summary>
		/// Returns a portion of the query results (sliced)
		/// </summary>
		public static T[] SlicedFindAll(int firstResult, int maxResults, Order[] orders, params ICriterion[] criterias)
		{
			return (T[]) ActiveRecordMediator.SlicedFindAll(typeof(T), firstResult, maxResults, orders, criterias);
		}

		/// <summary>
		/// Returns a portion of the query results (sliced)
		/// </summary>
		public static T[] SlicedFindAll(int firstResult, int maxResults, params ICriterion[] criterias)
		{
			return (T[]) ActiveRecordMediator.SlicedFindAll(typeof(T), firstResult, maxResults, null, criterias);
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
			return (T[]) ActiveRecordMediator.FindAll(typeof(T), orders, criterias);
		}

		/// <summary>
		/// Returns all instances found for the specified type 
		/// using criterias.
		/// </summary>
		/// <param name="criterias"></param>
		/// <returns></returns>
		public static T[] FindAll(params ICriterion[] criterias)
		{
			return (T[]) ActiveRecordMediator.FindAll(typeof(T), criterias);
		}

		public static void DeleteAll()
		{
			ActiveRecordMediator.DeleteAll(typeof(T));
		}

		public static void DeleteAll(string where)
		{
			ActiveRecordMediator.DeleteAll(typeof(T), where);
		}

		/// <summary>
		/// Saves the instance to the database
		/// </summary>
		/// <param name="instance"></param>
		public static void Save(T instance)
		{
			ActiveRecordMediator.Save(instance);
		}

		/// <summary>
		/// Creates (Saves) a new instance to the database.
		/// </summary>
		/// <param name="instance"></param>
		public static void Create(T instance)
		{
			ActiveRecordMediator.Create(instance);
		}

		/// <summary>
		/// Persists the modification on the instance
		/// state to the database.
		/// </summary>
		/// <param name="instance"></param>
		public static void Update(T instance)
		{
			ActiveRecordMediator.Update(instance);
		}

		/// <summary>
		/// Deletes the instance from the database.
		/// </summary>
		/// <param name="instance"></param>
		public static void Delete(T instance)
		{
			ActiveRecordMediator.Delete(instance);
		}
	}
}

#endif