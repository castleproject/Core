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

namespace Castle.ActiveRecord
{
	using System;
	using Framework;
	using NHibernate.Criterion;

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
			return Execute(typeof(T), call, instance);
		}

		/// <summary>
		/// Finds an object instance by its primary key.
		/// </summary>
		/// <param name="id">ID value</param>
		/// <param name="throwOnNotFound"><c>true</c> if you want an exception to be thrown
		/// if the object is not found</param>
		/// <exception cref="NHibernate.ObjectNotFoundException">if <c>throwOnNotFound</c> is set to 
		/// <c>true</c> and the row is not found</exception>
		public static T FindByPrimaryKey(object id, bool throwOnNotFound)
		{
			return (T) FindByPrimaryKey(typeof(T), id, throwOnNotFound);
		}

		/// <summary>
		/// Finds an object instance by its primary key.
		/// </summary>
		/// <param name="id">ID value</param>
		public static T FindByPrimaryKey(object id)
		{
			return (T) FindByPrimaryKey(typeof(T), id, true);
		}

		/// <summary>
		/// Searches and returns the first row.
		/// </summary>
		/// <param name="orders">The sort order - used to determine which record is the first one</param>
		/// <param name="criterias">The criteria expression</param>
		/// <returns>A <c>targetType</c> instance or <c>null</c></returns>
		public static T FindFirst(Order[] orders, params ICriterion[] criterias)
		{
			return (T) FindFirst(typeof(T), orders, criterias);
		}

		/// <summary>
		/// Searches and returns the first row.
		/// </summary>
		/// <param name="criterias">The criteria expression</param>
		/// <returns>A <c>targetType</c> instance or <c>null</c></returns>
		public static T FindFirst(params ICriterion[] criterias)
		{
			return (T) FindFirst(typeof(T), criterias);
		}

		/// <summary>
		/// Searches and returns the first row.
		/// </summary>
		/// <param name="detachedCriteria">The criteria.</param>
		/// <param name="orders">The sort order - used to determine which record is the first one.</param>
		/// <returns>A <c>targetType</c> instance or <c>null.</c></returns>
		public static T FindFirst(DetachedCriteria detachedCriteria, params Order[] orders)
		{
			return (T) FindFirst(typeof(T), detachedCriteria, orders);
		}

		/// <summary>
		/// Searches and returns the first row.
		/// </summary>
		/// <param name="criteria">The criteria expression</param>
		/// <returns>A <c>targetType</c> instance or <c>null</c></returns>
		public static T FindFirst(DetachedCriteria criteria)
		{
			return (T) FindFirst(typeof(T),criteria);
		}

		/// <summary>
		/// Searches and returns the first row.
		/// </summary>
		/// <param name="criterias">The criterias.</param>
		/// <returns>A instance the targetType or <c>null</c></returns>
		public static T FindOne(params ICriterion[] criterias)
		{
			return (T) FindOne(typeof(T), criterias);
		}

		/// <summary>
		/// Searches and returns a row. If more than one is found, 
		/// throws <see cref="ActiveRecordException"/>
		/// </summary>
		/// <param name="criteria">The criteria</param>
		/// <returns>A <c>targetType</c> instance or <c>null</c></returns>
		public static T FindOne(DetachedCriteria criteria)
		{
			return (T) FindOne(typeof(T), criteria);
		}

		/// <summary>
		/// Finds records based on a property value - automatically converts null values to IS NULL style queries. 
		/// </summary>
		/// <param name="property">A property name (not a column name)</param>
		/// <param name="value">The value to be equals to</param>
		/// <returns></returns>
		public static T[] FindAllByProperty(String property, object value)
		{
			return ActiveRecordBase<T>.FindAllByProperty(property, value);
		}

		/// <summary>
		/// Finds records based on a property value - automatically converts null values to IS NULL style queries. 
		/// </summary>
		/// <param name="orderByColumn">The column name to be ordered ASC</param>
		/// <param name="property">A property name (not a column name)</param>
		/// <param name="value">The value to be equals to</param>
		/// <returns></returns>
		public static T[] FindAllByProperty(String orderByColumn, String property, object value)
		{
			return ActiveRecordBase<T>.FindAllByProperty(orderByColumn, property, value);
		}

		/// <summary>
		/// Returns all instances found for the specified type.
		/// </summary>
		/// <returns></returns>
		public static T[] FindAll()
		{
			return (T[]) FindAll(typeof(T));
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
			return (T[]) FindAll(typeof(T), orders, criterias);
		}

		/// <summary>
		/// Returns all instances found for the specified type 
		/// using criterias.
		/// </summary>
		/// <param name="criterias"></param>
		/// <returns></returns>
		public static T[] FindAll(params ICriterion[] criterias)
		{
			return (T[]) FindAll(typeof(T), criterias);
		}

		/// <summary>
		/// Returns all instances found for the specified type according to the criteria
		/// </summary>
		public static T[] FindAll(DetachedCriteria detachedCriteria, params Order[] orders)
		{
			return (T[]) FindAll(typeof(T), detachedCriteria, orders);
		}

		/// <summary>
		/// Returns a portion of the query results (sliced)
		/// </summary>
		public static T[] SlicedFindAll(int firstResult, int maxResults, Order[] orders, params ICriterion[] criterias)
		{
			return (T[]) SlicedFindAll(typeof(T), firstResult, maxResults, orders, criterias);
		}

		/// <summary>
		/// Returns a portion of the query results (sliced)
		/// </summary>
		public static T[] SlicedFindAll(int firstResult, int maxResults, params ICriterion[] criterias)
		{
			return (T[]) SlicedFindAll(typeof(T), firstResult, maxResults, null, criterias);
		}

		/// <summary>
		/// Returns a portion of the query results (sliced)
		/// </summary>
		public static T[] SlicedFindAll(int firstResult, int maxResults, DetachedCriteria criteria, params Order[] orders)
		{
			return (T[]) SlicedFindAll(typeof(T), firstResult, maxResults, orders, criteria);
		}

		/// <summary>
		/// Deletes all entities of <typeparamref name="T"/>.
		/// </summary>
		public static void DeleteAll()
		{
			DeleteAll(typeof(T));
		}

		/// <summary>
		/// Deletes all entities of <typeparamref name="T"/> that match the HQL where clause.
		/// </summary>
		public static void DeleteAll(string where)
		{
			DeleteAll(typeof(T), where);
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
        /// Saves a copy of the instance to the database
        /// </summary>
        /// <param name="instance"></param>
        /// <returns>The saved instance</returns>
        public static T SaveCopy(T instance)
        {
            return (T) ActiveRecordMediator.SaveCopy(instance);
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

		/// <summary>
		/// Refresh the instance from the database.
		/// </summary>
		/// <param name="instance">The ActiveRecord instance to be reloaded</param>
		public static void Refresh(T instance)
		{
			ActiveRecordMediator.Refresh(instance);
		}

		/// <summary>
		/// Executes the query and return a strongly typed result
		/// </summary>
		/// <param name="query">The query.</param>
		/// <returns></returns>
		public static R ExecuteQuery2<R>(IActiveRecordQuery<R> query)
		{
			return ActiveRecordBase<T>.ExecuteQuery2(query);
		}

		/// <summary>
		/// Check if the <paramref name="id"/> exists in the database.
		/// </summary>
		/// <typeparam name="PkType">The <c>System.Type</c> of the PrimaryKey</typeparam>
		/// <param name="id">The id to check on</param>
		/// <returns><c>true</c> if the ID exists; otherwise <c>false</c>.</returns>
		public static bool Exists<PkType>(PkType id)
		{
			return ActiveRecordBase<T>.Exists(id);
		}

		/// <summary>
		/// Returns the number of records of the specified 
		/// type in the database
		/// </summary>
		/// <returns>The count result</returns>
		public static int Count()
		{
			return ActiveRecordBase.Count(typeof(T));
		}

		/// <summary>
		/// Returns the number of records of the specified 
		/// type in the database that match the given critera
		/// </summary>
		/// <param name="criteria">The criteria expression</param>
		/// <returns>The count result</returns>
		public static int Count(params ICriterion[] criteria)
		{
			return ActiveRecordBase.Count(typeof(T), criteria);
		}

		/// <summary>
		/// Returns the number of records of the specified 
		/// type in the database
		/// </summary>
		/// <param name="filter">A sql where string i.e. Person=? and DOB &gt; ?</param>
		/// <param name="args">Positional parameters for the filter string</param>
		/// <returns>The count result</returns>
		public static int Count(string filter, params object[] args)
		{
			return ActiveRecordBase.Count(typeof(T), filter, args);
		}

		/// <summary>
		/// Returns the number of records of the specified 
		/// type in the database
		/// </summary>
		/// <param name="detachedCriteria">The criteria expression</param>
		/// <returns>The count result</returns>
		public static int Count(DetachedCriteria detachedCriteria)
		{
			return ActiveRecordBase.Count(typeof(T), detachedCriteria);
		}

		/// <summary>
		/// Check if there is any records in the db for the target type
		/// </summary>
		/// <returns><c>true</c> if there's at least one row</returns>
		public static bool Exists()
		{
			return ActiveRecordBase.Exists(typeof(T));
		}


		/// <summary>
		/// Check if there is any records in the db for the target type
		/// </summary>
		/// <param name="filter">A sql where string i.e. Person=? and DOB &gt; ?</param>
		/// <param name="args">Positional parameters for the filter string</param>
		/// <returns><c>true</c> if there's at least one row</returns>
		public static bool Exists(string filter, params object[] args)
		{
			return ActiveRecordBase.Exists(typeof(T), filter, args);
		}

		/// <summary>
		/// Check if the <paramref name="id"/> exists in the database.
		/// </summary>
		/// <param name="id">The id to check on</param>
		/// <returns><c>true</c> if the ID exists; otherwise <c>false</c>.</returns>
		public static bool Exists(object id)
		{
			return ActiveRecordBase.Exists(typeof(T), id);
		}

		/// <summary>
		/// Check if any instance matches the criteria.
		/// </summary>
		/// <returns><c>true</c> if an instance is found; otherwise <c>false</c>.</returns>
		public static bool Exists(params ICriterion[] criterias)
		{
			return ActiveRecordBase.Exists(typeof(T), criterias);
		}

		/// <summary>
		/// Check if any instance matching the criteria exists in the database.
		/// </summary>
		/// <param name="detachedCriteria">The criteria expression</param>		
		/// <returns><c>true</c> if an instance is found; otherwise <c>false</c>.</returns>
		public static bool Exists(DetachedCriteria detachedCriteria)
		{
			return ActiveRecordBase.Exists(typeof(T), detachedCriteria);
		}
	}
}
