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
	using System.Collections;
	using Castle.ActiveRecord.Framework;
	using NHibernate;
	using NHibernate.Criterion;

	/// <summary>
	/// Base class for all ActiveRecord Generic classes. 
	/// Implements all the functionality to simplify the code on the subclasses.
	/// </summary>
	[Serializable]
	public abstract class ActiveRecordBase<T> : ActiveRecordBase
	{
		#region protected internal static

		#region Create/Update/Save/Delete/Refresh

		#region Create

		/// <summary>
		/// Creates (Saves) a new instance to the database.
		/// </summary>
		/// <param name="instance">The ActiveRecord instance to be created on the database</param>
		protected internal static void Create(T instance)
		{
			ActiveRecordBase.Create(instance);
		}

		#endregion

		#region Delete

		/// <summary>
		/// Deletes the instance from the database.
		/// </summary>
		/// <param name="instance">The ActiveRecord instance to be deleted</param>
		protected internal static void Delete(T instance)
		{
			ActiveRecordBase.Delete(instance);
		}

		#endregion

		#region DeleteAll

		/// <summary>
		/// Deletes all rows for the specified ActiveRecord type
		/// </summary>
		/// <remarks>
		/// This method is usually useful for test cases.
		/// </remarks>
		public static void DeleteAll()
		{
			DeleteAll(typeof(T));
		}

		/// <summary>
		/// Deletes all rows for the specified ActiveRecord type that matches
		/// the supplied HQL condition
		/// </summary>
		/// <remarks>
		/// This method is usually useful for test cases.
		/// </remarks>
		/// <param name="where">HQL condition to select the rows to be deleted</param>
		public static void DeleteAll(String where)
		{
			DeleteAll(typeof(T), where);
		}

		/// <summary>
		/// Deletes all <typeparamref name="T"/> objects, based on the primary keys
		/// supplied on <paramref name="pkValues" />.
		/// </summary>
		/// <returns>The number of objects deleted</returns>
		public static int DeleteAll(IEnumerable pkValues)
		{
			return DeleteAll(typeof(T), pkValues);
		}

		#endregion

		#region Refresh

		/// <summary>
		/// Refresh the instance from the database.
		/// </summary>
		/// <param name="instance">The ActiveRecord instance to be reloaded</param>
		protected internal static void Refresh(T instance)
		{
			ActiveRecordBase.Refresh(instance);
		}

		#endregion

		#region Update

		/// <summary>
		/// Persists the modification on the instance
		/// state to the database.
		/// </summary>
		/// <param name="instance">The ActiveRecord instance to be updated on the database</param>
		protected internal static void Update(T instance)
		{
			ActiveRecordBase.Update(instance);
		}

		#endregion

		#region Save

		/// <summary>
		/// Saves the instance to the database. If the primary key is unitialized
		/// it creates the instance on the database. Otherwise it updates it.
		/// <para>
		/// If the primary key is assigned, then you must invoke <see cref="Create"/>
		/// or <see cref="Update"/> instead.
		/// </para>
		/// </summary>
		/// <param name="instance">The ActiveRecord instance to be saved</param>
		protected internal static void Save(T instance)
		{
			ActiveRecordBase.Save(instance);
		}

        /// <summary>
        /// Saves a copy of the instance to the database. If the primary key is unitialized
        /// it creates the instance on the database. Otherwise it updates it.
        /// <para>
        /// If the primary key is assigned, then you must invoke <see cref="Create"/>
        /// or <see cref="Update"/> instead.
        /// </para>
        /// </summary>
        /// <param name="instance">The transient instance to be saved</param>
        /// <returns>The saved ActiveRecord instance.</returns>
        protected internal static T SaveCopy(T instance)
        {
            return (T) ActiveRecordBase.SaveCopy(instance);
        }

		#endregion

		#endregion

		#region Execute

		/// <summary>
		/// Invokes the specified delegate passing a valid 
		/// NHibernate session. Used for custom NHibernate queries.
		/// </summary>
		/// <param name="call">The delegate instance</param>
		/// <param name="instance">The ActiveRecord instance</param>
		/// <returns>Whatever is returned by the delegate invocation</returns>
		protected static object Execute(NHibernateDelegate call, object instance)
		{
			return Execute(typeof(T), call, instance);
		}

		/// <summary>
		/// Executes the query and return a strongly typed result
		/// </summary>
		/// <param name="query">The query.</param>
		/// <returns>The query result.</returns>
		protected internal static R ExecuteQuery2<R>(IActiveRecordQuery<R> query)
		{
			object result = ExecuteQuery(query);
			if (result == null) return default(R);

			if (!typeof(R).IsAssignableFrom(result.GetType()))
				throw new NHibernate.QueryException(
					string.Format("Problem: A query was executed requesting {0} as result, but the query returned an object of type {1}.", typeof(R).Name, result.GetType().Name));
			return (R)result;
		}

		#endregion

		#region Count

		/// <summary>
		/// Returns the number of records of <typeparamref name="T"/> in the database
		/// </summary>
		/// <example>
		/// <code escaped="true">
		/// [ActiveRecord]
		/// public class User : ActiveRecordBase&lt;User&gt;
		/// {
		///   ...
		///   
		///   public static int CountAllUsers()
		///   {
		///     return Count(); // Equivalent to: Count(typeof(User));
		///   }
		/// }
		/// </code>
		/// </example>
		/// <returns>The count query result</returns>
		protected internal static int Count()
		{
			return Count(typeof(T));
		}

		/// <summary>
		/// Returns the number of records of <typeparamref name="T"/> in the database
		/// </summary>
		/// <example>
		/// <code escaped="true">
		/// [ActiveRecord]
		/// public class User : ActiveRecordBase&lt;User&gt;
		/// {
		///   ...
		///   
		///   public static int CountAllUsersLocked()
		///   {
		///     return Count("IsLocked = ?", true); // Equivalent to: Count(typeof(User), "IsLocked = ?", true);
		///   }
		/// }
		/// </code>
		/// </example>
		/// <param name="filter">A sql where string i.e. Person=? and DOB &gt; ?</param>
		/// <param name="args">Positional parameters for the filter string</param>
		/// <returns>The count result</returns>
		protected internal static int Count(String filter, params object[] args)
		{
			return Count(typeof(T), filter, args);
		}

		/// <summary>
		/// Check if any instance matching the criteria exists in the database.
		/// </summary>
		/// <param name="criteria">The criteria expression</param>		
		/// <returns>The count result</returns>
		protected internal static int Count(params ICriterion[] criteria)
		{
			return Count(typeof(T), criteria);
		}

		/// <summary>
		/// Returns the number of records of the specified 
		/// type in the database
		/// </summary>
		/// <param name="detachedCriteria">The criteria expression</param>
		/// <returns>The count result</returns>
		protected internal static int Count(DetachedCriteria detachedCriteria)
		{
			return Count(typeof(T), detachedCriteria);
		}

		#endregion

		#region Exists

		/// <summary>
		/// Check if there is any records in the db for <typeparamref name="T"/>
		/// </summary>
		/// <returns><c>true</c> if there's at least one row</returns>
		public static bool Exists()
		{
			return ActiveRecordBase.Exists(typeof(T));
		}

		/// <summary>
		/// Check if there is any records in the db for <typeparamref name="T"/>
		/// </summary>
		/// <param name="filter">A sql where string i.e. Person=? and DOB &gt; ?</param>
		/// <param name="args">Positional parameters for the filter string</param>
		/// <returns><c>true</c> if there's at least one row</returns>
		public static bool Exists(String filter, params object[] args)
		{
			return Exists(typeof(T), filter, args);
		}

		/// <summary>
		/// Check if the <paramref name="id"/> exists in the database.
		/// </summary>
		/// <typeparam name="PkType">The <c>System.Type</c> of the PrimaryKey</typeparam>
		/// <param name="id">The id to check on</param>
		/// <returns><c>true</c> if the ID exists; otherwise <c>false</c>.</returns>
		public static bool Exists<PkType>(PkType id)
		{
			if (typeof(ICriterion).IsAssignableFrom(typeof(PkType)))
				return Exists(new ICriterion[] {(ICriterion) id});
			return Exists(typeof(T), id);
		}

		/// <summary>
		/// Check if any instance matching the criteria exists in the database.
		/// </summary>
		/// <param name="criteria">The criteria expression</param>		
		/// <returns><c>true</c> if an instance is found; otherwise <c>false</c>.</returns>
		public static bool Exists(params ICriterion[] criteria)
		{
			return Exists(typeof(T), criteria);
		}

		/// <summary>
		/// Check if any instance matching the criteria exists in the database.
		/// </summary>
		/// <param name="detachedCriteria">The criteria expression</param>		
		/// <returns><c>true</c> if an instance is found; otherwise <c>false</c>.</returns>
		public static bool Exists(DetachedCriteria detachedCriteria)
		{
			return Exists(typeof(T), detachedCriteria);
		}

		#endregion

		#region FindAll

		/// <summary>
		/// Returns all the instances that match the detached criteria.
		/// </summary>
		/// <param name="criteria">Detached criteria</param>
		/// <param name="orders">Optional ordering</param>
		/// <returns>All entities that match the criteria</returns>
		public static T[] FindAll(DetachedCriteria criteria, params Order[] orders)
		{
			return (T[]) FindAll(typeof(T), criteria, orders);
		}

		/// <summary>
		/// Returns all instances found for <typeparamref name="T"/>
		/// </summary>
		/// <returns>An <see cref="Array"/> of <typeparamref name="T"/></returns>
		public static T[] FindAll()
		{
			return (T[]) FindAll(typeof(T));
		}

		/// <summary>
		/// Returns all instances found for the specified type 
		/// using sort orders and criteria.
		/// </summary>
		/// <param name="order">An <see cref="Order"/> object.</param>
		/// <param name="criteria">The criteria expression</param>
		/// <returns>The <see cref="Array"/> of results.</returns>
		public static T[] FindAll(Order order, params ICriterion[] criteria)
		{
			return (T[]) FindAll(typeof(T), new Order[] {order}, criteria);
		}

		/// <summary>
		/// Returns all instances found for <typeparamref name="T"/>
		/// using sort orders and criteria.
		/// </summary>
		/// <param name="orders"></param>
		/// <param name="criteria"></param>
		/// <returns>An <see cref="Array"/> of <typeparamref name="T"/></returns>
		public static T[] FindAll(Order[] orders, params ICriterion[] criteria)
		{
			return (T[]) FindAll(typeof(T), orders, criteria);
		}

		/// <summary>
		/// Returns all instances found for <typeparamref name="T"/>
		/// using criteria.
		/// </summary>
		/// <param name="criteria"></param>
		/// <returns>An <see cref="Array"/> of <typeparamref name="T"/></returns>
		public static T[] FindAll(params ICriterion[] criteria)
		{
			return (T[]) FindAll(typeof(T), criteria);
		}

		#endregion

		#region FindAllByProperty

		/// <summary>
		/// Finds records based on a property value
		/// </summary>
		/// <param name="property">A property name (not a column name)</param>
		/// <param name="value">The value to be equals to</param>
		/// <returns>An <see cref="Array"/> of <typeparamref name="T"/></returns>
		public static T[] FindAllByProperty(String property, object value)
		{
			return (T[]) FindAllByProperty(typeof(T), property, value);
		}

		/// <summary>
		/// Finds records based on a property value
		/// </summary>
		/// <param name="orderByColumn">The column name to be ordered ASC</param>
		/// <param name="property">A property name (not a column name)</param>
		/// <param name="value">The value to be equals to</param>
		/// <returns>An <see cref="Array"/> of <typeparamref name="T"/></returns>
		public static T[] FindAllByProperty(String orderByColumn, String property, object value)
		{
			return (T[]) FindAllByProperty(typeof(T), orderByColumn, property, value);
		}

		#endregion

		#region FindByPrimaryKey/Find/TryFind

		/// <summary>
		/// Finds an object instance by an unique ID 
		/// </summary>
		/// <param name="id">ID value</param>
		/// <exception cref="ObjectNotFoundException">if the row is not found</exception>
		/// <returns>T</returns>
		public static T Find(object id)
		{
			return (T) FindByPrimaryKey(typeof(T), id, true);
		}

		/// <summary>
		/// Finds an object instance by an unique ID.
		/// If the row is not found this method will not throw an exception.
		/// </summary>
		/// <param name="id">ID value</param>
		/// <returns>A <typeparamref name="T"/></returns>
		public static T TryFind(object id)
		{
			return (T) FindByPrimaryKey(typeof(T), id, false);
		}

		/// <summary>
		/// Finds an object instance by an unique ID for <typeparamref name="T"/>
		/// </summary>
		/// <param name="id">ID value</param>
		/// <returns>A <typeparamref name="T"/></returns>
		protected internal static T FindByPrimaryKey(object id)
		{
			return (T) FindByPrimaryKey(typeof(T), id);
		}

		/// <summary>
		/// Finds an object instance by a unique ID for <typeparamref name="T"/>
		/// </summary>
		/// <param name="id">ID value</param>
		/// <param name="throwOnNotFound"><c>true</c> if you want to catch an exception 
		/// if the object is not found</param>
		/// <returns>A <typeparamref name="T"/></returns>
		/// <exception cref="ObjectNotFoundException">if <c>throwOnNotFound</c> is set to 
		/// <c>true</c> and the row is not found</exception>
		protected internal static T FindByPrimaryKey(object id, bool throwOnNotFound)
		{
			return (T) FindByPrimaryKey(typeof(T), id, throwOnNotFound);
		}

		#endregion

		#region FindFirst

		/// <summary>
		/// Searches and returns the first row for <typeparamref name="T"/>.
		/// </summary>
		/// <param name="criteria">Detached criteria.</param>
		/// <param name="orders">The sort order - used to determine which record is the first one.</param>
		/// <returns>A <c>targetType</c> instance or <c>null</c>.</returns>
		public static T FindFirst(DetachedCriteria criteria, params Order[] orders)
		{
			return (T) FindFirst(typeof(T), criteria, orders);
		}

		/// <summary>
		/// Searches and returns the first row for <typeparamref name="T"/>
		/// </summary>
		/// <param name="order">The sort order - used to determine which record is the first one</param>
		/// <param name="criteria">The criteria expression</param>
		/// <returns>A <c>targetType</c> instance or <c>null</c></returns>
		public static T FindFirst(Order order, params ICriterion[] criteria)
		{
			return (T) FindFirst(typeof(T), new Order[] {order}, criteria);
		}

		/// <summary>
		/// Searches and returns the first row for <typeparamref name="T"/>
		/// </summary>
		/// <param name="orders">The sort order - used to determine which record is the first one</param>
		/// <param name="criteria">The criteria expression</param>
		/// <returns>A <c>targetType</c> instance or <c>null</c></returns>
		public static T FindFirst(Order[] orders, params ICriterion[] criteria)
		{
			return (T) FindFirst(typeof(T), orders, criteria);
		}

		/// <summary>
		/// Searches and returns the first row for <typeparamref name="T"/>
		/// </summary>
		/// <param name="criteria">The criteria expression</param>
		/// <returns>A <c>targetType</c> instance or <c>null</c></returns>
		public static T FindFirst(params ICriterion[] criteria)
		{
			return (T) FindFirst(typeof(T), criteria);
		}

		#endregion

		#region FindOne

		/// <summary>
		/// Searches and returns a row. If more than one is found, 
		/// throws <see cref="ActiveRecordException"/>
		/// </summary>
		/// <param name="criteria">The criteria expression</param>
		/// <returns>A <c>targetType</c> instance or <c>null</c></returns>
		public static T FindOne(params ICriterion[] criteria)
		{
			return (T) FindOne(typeof(T), criteria);
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

		#endregion

		#region SlicedFindAll

		/// <summary>
		/// Returns a portion of the query results (sliced)
		/// </summary>
		/// <param name="firstResult">The number of the first row to retrieve.</param>
		/// <param name="maxResults">The maximum number of results retrieved.</param>
		/// <param name="orders">An <see cref="Array"/> of <see cref="Order"/> objects.</param>
		/// <param name="criteria">The criteria expression</param>
		/// <returns>The sliced query results.</returns>
		public static T[] SlicedFindAll(int firstResult, int maxResults, Order[] orders,
		                                params ICriterion[] criteria)
		{
			return (T[]) SlicedFindAll(typeof(T), firstResult, maxResults, orders, criteria);
		}

		/// <summary>
		/// Returns a portion of the query results (sliced)
		/// </summary>
		/// <param name="firstResult">The number of the first row to retrieve.</param>
		/// <param name="maxResults">The maximum number of results retrieved.</param>
		/// <param name="criteria">The criteria expression</param>
		/// <returns>The sliced query results.</returns>
		public static T[] SlicedFindAll(int firstResult, int maxResults,
		                                params ICriterion[] criteria)
		{
			return (T[]) SlicedFindAll(typeof(T), firstResult, maxResults, criteria);
		}

		/// <summary>
		/// Returns a portion of the query results (sliced)
		/// </summary>
		/// <param name="firstResult">The number of the first row to retrieve.</param>
		/// <param name="maxResults">The maximum number of results retrieved.</param>
		/// <param name="orders">An <see cref="Array"/> of <see cref="Order"/> objects.</param>
		/// <param name="criteria">The criteria expression</param>
		/// <returns>The sliced query results.</returns>
		public static T[] SlicedFindAll(int firstResult, int maxResults, DetachedCriteria criteria, params Order[] orders)
		{
			return (T[]) SlicedFindAll(typeof(T), firstResult, maxResults, orders, criteria);
		}

		#endregion

		#endregion
	}
}
