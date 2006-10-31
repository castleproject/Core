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
	using System;
	using System.Collections;
	using Castle.ActiveRecord.Framework;
	
	using NHibernate;
	using NHibernate.Expression;

	/// <summary>
	/// Base class for all ActiveRecord Generic classes. 
	/// Implements all the functionality to simplify the code on the subclasses.
	/// </summary>
	[Serializable]
	public abstract class ActiveRecordBase<T> : ActiveRecordBase
	{
		/// <summary>
		/// Constructs an ActiveRecordBase subclass.
		/// </summary>
		public ActiveRecordBase()
		{
		}

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
			ActiveRecordBase.DeleteAll(typeof(T));
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
			ActiveRecordBase.DeleteAll(typeof(T), where);
		}

		/// <summary>
		/// Deletes all <typeparamref name="T"/> objects, based on the primary keys
		/// supplied on <paramref name="pkValues" />.
		/// </summary>
		/// <returns>The number of objects deleted</returns>
		public static int DeleteAll(IEnumerable pkValues)
		{
			return ActiveRecordBase.DeleteAll(typeof(T), pkValues);
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
			return ActiveRecordBase.Execute(typeof(T), call, instance);
		}

		/// <summary>
		/// Executes the query and return a strongly typed result
		/// </summary>
		/// <param name="query">The query.</param>
		/// <returns></returns>
		protected internal static R ExecuteQuery2<R>(IActiveRecordQuery<R> query)
		{
			return (R)ActiveRecordBase.ExecuteQuery(query);
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
		///     return CountAll(); // Equivalent to: CountAll(typeof(User));
		///   }
		/// }
		/// </code>
		/// </example>
		/// <returns>The count query result</returns>
		protected internal static int CountAll()
		{
			return ActiveRecordBase.CountAll(typeof(T));
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
		///     return CountAll("IsLocked = ?", true); // Equivalent to: CountAll(typeof(User), "IsLocked = ?", true);
		///   }
		/// }
		/// </code>
		/// </example>
		/// <param name="filter">A sql where string i.e. Person=? and DOB &gt; ?</param>
		/// <param name="args">Positional parameters for the filter string</param>
		/// <returns>The count result</returns>
		protected internal static int CountAll(String filter, params object[] args)
		{
			return ActiveRecordBase.CountAll(typeof(T), filter, args);
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
			return ActiveRecordBase.Exists(typeof(T), filter, args);
		}

		/// <summary>
		/// Check if the <paramref name="id"/> exists in the database.
		/// </summary>
		/// <typeparam name="PkType">The <c>System.Type</c> of the PrimaryKey</typeparam>
		/// <param name="id">The id to check on</param>
		/// <returns><c>true</c> if the ID exists; otherwise <c>false</c>.</returns>
		public static bool Exists<PkType>(PkType id)
		{
			return ActiveRecordBase.Exists(typeof(T), id);
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
			return (T[])ActiveRecordBase.FindAll(typeof(T), criteria, orders);	
		}
		
		/// <summary>
		/// Returns all instances found for <typeparamref name="T"/>
		/// </summary>
		/// <returns></returns>
		public static T[] FindAll()
		{
			return (T[]) ActiveRecordBase.FindAll(typeof(T));
		}

		/// <summary>
		/// Returns all instances found for <typeparamref name="T"/>
		/// using the specified sort order and criterias.
		/// </summary>
		public static T[] FindAll(Order order, params ICriterion[] criterias)
		{
			return (T[])ActiveRecordBase.FindAll(typeof(T), new Order[] { order }, criterias);
		}
		/// <summary>
		/// Returns all instances found for <typeparamref name="T"/>
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
		/// Returns all instances found for <typeparamref name="T"/>
		/// using criterias.
		/// </summary>
		/// <param name="criterias"></param>
		/// <returns></returns>
		public static T[] FindAll(params ICriterion[] criterias)
		{
			return (T[]) ActiveRecordBase.FindAll(typeof(T), criterias);
		}

		#endregion

		#region FindAllByProperty

		/// <summary>
		/// Finds records based on a property value
		/// </summary>
		/// <param name="property">A property name (not a column name)</param>
		/// <param name="value">The value to be equals to</param>
		/// <returns></returns>
		public static T[] FindAllByProperty(String property, object value)
		{
			return (T[]) ActiveRecordBase.FindAllByProperty(typeof(T), property, value);
		}

		/// <summary>
		/// Finds records based on a property value
		/// </summary>
		/// <param name="orderByColumn">The column name to be ordered ASC</param>
		/// <param name="property">A property name (not a column name)</param>
		/// <param name="value">The value to be equals to</param>
		/// <returns></returns>
		public static T[] FindAllByProperty(String orderByColumn, String property, object value)
		{
			return (T[]) ActiveRecordBase.FindAllByProperty(typeof(T), orderByColumn, property, value);
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
			return (T) ActiveRecordBase.FindByPrimaryKey(typeof(T), id, true);
		}

		/// <summary>
		/// Finds an object instance by an unique ID.
		/// If the row is not found this method will not throw an exception.
		/// </summary>
		/// <param name="id">ID value</param>
		/// <returns></returns>
		public static T TryFind(object id)
		{
			return (T) ActiveRecordBase.FindByPrimaryKey(typeof(T), id, false);
		}

		/// <summary>
		/// Finds an object instance by an unique ID for <typeparamref name="T"/>
		/// </summary>
		/// <param name="id">ID value</param>
		/// <returns></returns>
		protected internal static T FindByPrimaryKey(object id)
		{
			return (T) ActiveRecordBase.FindByPrimaryKey(typeof(T), id);
		}

		/// <summary>
		/// Finds an object instance by a unique ID for <typeparamref name="T"/>
		/// </summary>
		/// <param name="id">ID value</param>
		/// <param name="throwOnNotFound"><c>true</c> if you want to catch an exception 
		/// if the object is not found</param>
		/// <returns></returns>
		/// <exception cref="ObjectNotFoundException">if <c>throwOnNotFound</c> is set to 
		/// <c>true</c> and the row is not found</exception>
		protected internal static T FindByPrimaryKey(object id, bool throwOnNotFound)
		{
			return (T) ActiveRecordBase.FindByPrimaryKey(typeof(T), id, throwOnNotFound);
		}

		#endregion

		#region FindFirst

		/// <summary>
		/// Searches and returns the first row for <typeparamref name="T"/>
		/// </summary>
		/// <param name="order">The sort order - used to determine which record is the first one</param>
		/// <param name="criterias">The criteria expression</param>
		/// <returns>A <c>targetType</c> instance or <c>null</c></returns>
		public static T FindFirst(Order order, params ICriterion[] criterias)
		{
			return (T)ActiveRecordBase.FindFirst(typeof(T), new Order[] { order }, criterias);
		}
		
		/// <summary>
		/// Searches and returns the first row for <typeparamref name="T"/>
		/// </summary>
		/// <param name="orders">The sort order - used to determine which record is the first one</param>
		/// <param name="criterias">The criteria expression</param>
		/// <returns>A <c>targetType</c> instance or <c>null</c></returns>
		public static T FindFirst(Order[] orders, params ICriterion[] criterias)
		{
			return (T) ActiveRecordBase.FindFirst(typeof(T), orders, criterias);
		}

		/// <summary>
		/// Searches and returns the first row for <typeparamref name="T"/>
		/// </summary>
		/// <param name="criterias">The criteria expression</param>
		/// <returns>A <c>targetType</c> instance or <c>null</c></returns>
		public static T FindFirst(params ICriterion[] criterias)
		{
			return (T) ActiveRecordBase.FindFirst(typeof(T), criterias);
		}

		#endregion

		#region FindOne

		/// <summary>
		/// Searches and returns a row. If more than one is found, 
		/// throws <see cref="ActiveRecordException"/>
		/// </summary>
		/// <param name="criterias">The criteria expression</param>
		/// <returns>A <c>targetType</c> instance or <c>null</c></returns>
		public static T FindOne(params ICriterion[] criterias)
		{
			return (T) ActiveRecordBase.FindOne(typeof(T), criterias);
		}

		/// <summary>
		/// Searches and returns a row. If more than one is found, 
		/// throws <see cref="ActiveRecordException"/>
		/// </summary>
		/// <param name="criteria">The criteria</param>
		/// <returns>A <c>targetType</c> instance or <c>null</c></returns>
		public static T FindOne(DetachedCriteria criteria)
		{
			return (T)ActiveRecordBase.FindOne(typeof(T), criteria);
		}

		
		#endregion

		#region SlicedFindAll

		/// <summary>
		/// Returns a portion of the query results (sliced)
		/// </summary>
		public static T[] SlicedFindAll(int firstResult, int maxResults, Order[] orders,
		                                              params ICriterion[] criterias)
		{
			return (T[]) ActiveRecordBase.SlicedFindAll(typeof(T), firstResult, maxResults, orders, criterias);
		}

		/// <summary>
		/// Returns a portion of the query results (sliced)
		/// </summary>
		public static T[] SlicedFindAll(int firstResult, int maxResults,
		                                              params ICriterion[] criterias)
		{
			return (T[]) ActiveRecordBase.SlicedFindAll(typeof(T), firstResult, maxResults, criterias);
		}

        /// <summary>
        /// Returns a portion of the query results (sliced)
        /// </summary>
        public static T[] SlicedFindAll(int firstResult, int maxResults, DetachedCriteria criteria, params Order[] orders)
	    {
            return (T[]) ActiveRecordBase.SlicedFindAll(typeof (T), firstResult, maxResults, orders, criteria);
	    }

		#endregion

		#endregion
	}
}

#endif
