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
	using NHibernate;
	using NHibernate.Criterion;
	using Castle.ActiveRecord.Framework;

	/// <summary>
	/// Allow programmers to use the 
	/// ActiveRecord functionality without direct reference
	/// to <see cref="ActiveRecordBase"/>
	/// </summary>
	public class ActiveRecordMediator
	{
		/// <summary>
		/// Invokes the specified delegate passing a valid 
		/// NHibernate session. Used for custom NHibernate queries.
		/// </summary>
		/// <param name="targetType">The target ActiveRecordType</param>
		/// <param name="call">The delegate instance</param>
		/// <param name="instance">The ActiveRecord instance</param>
		/// <returns>Whatever is returned by the delegate invocation</returns>
		public static object Execute(Type targetType, NHibernateDelegate call, object instance)
		{
			return ActiveRecordBase.Execute(targetType, call, instance);
		}

		/// <summary>
		/// Finds an object instance by its primary key.
		/// </summary>
		/// <param name="targetType">The AR subclass type</param>
		/// <param name="id">ID value</param>
		/// <param name="throwOnNotFound"><c>true</c> if you want an exception to be thrown
		/// if the object is not found</param>
		/// <exception cref="ObjectNotFoundException">if <c>throwOnNotFound</c> is set to 
		/// <c>true</c> and the row is not found</exception>
		public static object FindByPrimaryKey(Type targetType, object id, bool throwOnNotFound)
		{
			return ActiveRecordBase.FindByPrimaryKey(targetType, id, throwOnNotFound);
		}

		/// <summary>
		/// Finds an object instance by its primary key.
		/// </summary>
		/// <param name="targetType">The AR subclass type</param>
		/// <param name="id">ID value</param>
		public static object FindByPrimaryKey(Type targetType, object id)
		{
			return FindByPrimaryKey(targetType, id, true);
		}

		/// <summary>
		/// Searches and returns the first row.
		/// </summary>
		/// <param name="targetType">The target type</param>
		/// <param name="orders">The sort order - used to determine which record is the first one</param>
		/// <param name="criterias">The criteria expression</param>
		/// <returns>A <c>targetType</c> instance or <c>null</c></returns>
		public static object FindFirst(Type targetType, Order[] orders, params ICriterion[] criterias)
		{
			return ActiveRecordBase.FindFirst(targetType, orders, criterias);
		}

		/// <summary>
		/// Searches and returns the first row.
		/// </summary>
		/// <param name="targetType">The target type</param>
		/// <param name="criterias">The criteria expression</param>
		/// <returns>A <c>targetType</c> instance or <c>null</c></returns>
		public static object FindFirst(Type targetType, params ICriterion[] criterias)
		{
			return FindFirst(targetType, null, criterias);
		}

		/// <summary>
		/// Searches and returns the first row.
		/// </summary>
		/// <param name="targetType">The target type.</param>
		/// <param name="detachedCriteria">The criteria.</param>
		/// <param name="orders">The sort order - used to determine which record is the first one.</param>
		/// <returns>A <c>targetType</c> instance or <c>null.</c></returns>
		public static object FindFirst(Type targetType, DetachedCriteria detachedCriteria, params Order[] orders)
		{
			return ActiveRecordBase.FindFirst(targetType, detachedCriteria, orders);
		}

		/// <summary>
		/// Searches and returns the first row.
		/// </summary>
		/// <param name="targetType">The target type</param>
		/// <param name="criteria">The criteria expression</param>
		/// <returns>A <c>targetType</c> instance or <c>null</c></returns>
		public static object FindFirst(Type targetType, DetachedCriteria criteria)
		{
			return ActiveRecordBase.FindFirst(targetType, criteria);
		}

		/// <summary>
		/// Searches and returns the a row. If more than one is found, 
		/// throws <see cref="ActiveRecordException"/>
		/// </summary>
		/// <param name="targetType">The target type</param>
		/// <param name="criterias">The criteria expression</param>
		/// <returns>A <c>targetType</c> instance or <c>null</c></returns>
		public static object FindOne(Type targetType, params ICriterion[] criterias)
		{
			return ActiveRecordBase.FindOne(targetType, criterias);
		}

		/// <summary>
		/// Searches and returns a row. If more than one is found, 
		/// throws <see cref="ActiveRecordException"/>
		/// </summary>
		/// <param name="targetType">The target type</param>
		/// <param name="criteria">The criteria</param>
		/// <returns>A <c>targetType</c> instance or <c>null</c></returns>
		public static object FindOne(Type targetType, DetachedCriteria criteria)
		{
			return ActiveRecordBase.FindOne(targetType, criteria);
		}

		/// <summary>
		/// Returns a portion of the query results (sliced)
		/// </summary>
		public static Array SlicedFindAll(Type targetType, int firstResult, int maxresults,
		                                  Order[] orders, params ICriterion[] criterias)
		{
			return ActiveRecordBase.SlicedFindAll(targetType, firstResult,
			                                      maxresults, orders, criterias);
		}

		/// <summary>
		/// Returns a portion of the query results (sliced)
		/// </summary>
		public static Array SlicedFindAll(Type targetType, int firstResult, int maxresults, params ICriterion[] criterias)
		{
			return SlicedFindAll(targetType, firstResult, maxresults, null, criterias);
		}

		/// <summary>
		/// Returns a portion of the query results (sliced)
		/// </summary>
		public static Array SlicedFindAll(Type targetType, int firstResult, int maxResults,
													  Order[] orders, DetachedCriteria criteria)
		{
			return ActiveRecordBase.SlicedFindAll(targetType, firstResult, maxResults, orders, criteria);
		}

		/// <summary>
		/// Returns a portion of the query results (sliced)
		/// </summary>
		public static Array SlicedFindAll(Type targetType, int firstResult, int maxResults,
													  DetachedCriteria criteria)
		{
			return ActiveRecordBase.SlicedFindAll(targetType, firstResult, maxResults, criteria);
		}

		/// <summary>
		/// Returns all instances found for the specified type.
		/// </summary>
		/// <param name="targetType"></param>
		/// <returns></returns>
		public static Array FindAll(Type targetType)
		{
			return FindAll(targetType, (Order[]) null);
		}

		/// <summary>
		/// Returns all instances found for the specified type 
		/// using sort orders and criterias.
		/// </summary>
		/// <param name="targetType"></param>
		/// <param name="orders"></param>
		/// <param name="criterias"></param>
		/// <returns></returns>
		public static Array FindAll(Type targetType, Order[] orders, params ICriterion[] criterias)
		{
			return ActiveRecordBase.FindAll(targetType, orders, criterias);
		}

		/// <summary>
		/// Returns all instances found for the specified type 
		/// using criterias.
		/// </summary>
		/// <param name="targetType"></param>
		/// <param name="criterias"></param>
		/// <returns></returns>
		public static Array FindAll(Type targetType, params ICriterion[] criterias)
		{
			return FindAll(targetType, null, criterias);
		}

		/// <summary>
		/// Returns all instances found for the specified type according to the criteria
		/// </summary>
		public static Array FindAll(Type targetType, DetachedCriteria detachedCriteria, params Order[] orders)
		{
			return ActiveRecordBase.FindAll(targetType, detachedCriteria, orders);
		}

		/// <summary>
		/// Finds records based on a property value - automatically converts null values to IS NULL style queries. 
		/// </summary>
		/// <param name="targetType">The target type</param>
		/// <param name="property">A property name (not a column name)</param>
		/// <param name="value">The value to be equals to</param>
		/// <returns></returns>
		public static Array FindAllByProperty(Type targetType, String property, object value)
		{
			return ActiveRecordBase.FindAllByProperty(targetType, property, value);
		}

		/// <summary>
		/// Finds records based on a property value - automatically converts null values to IS NULL style queries. 
		/// </summary>
		/// <param name="targetType">The target type</param>
		/// <param name="orderByColumn">The column name to be ordered ASC</param>
		/// <param name="property">A property name (not a column name)</param>
		/// <param name="value">The value to be equals to</param>
		/// <returns></returns>
		public static Array FindAllByProperty(Type targetType, String orderByColumn, String property, object value)
		{
			return ActiveRecordBase.FindAllByProperty(targetType, orderByColumn, property, value);
		}

		/// <summary>
		/// Deletes all entities of the specified type (and their inheritors)
		/// </summary>
		/// <param name="type">The type.</param>
		public static void DeleteAll(Type type)
		{
			ActiveRecordBase.DeleteAll(type);
		}

		/// <summary>
		/// Deletes all entities of specified type that match the HQL where clause
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="where">The where.</param>
		public static void DeleteAll(Type type, string where)
		{
			ActiveRecordBase.DeleteAll(type, where);
		}

		/// <summary>
		/// Deletes all <paramref name="targetType" /> objects, based on the primary keys
		/// supplied on <paramref name="pkValues" />.
		/// </summary>
		/// <param name="targetType">The target ActiveRecord type</param>
		/// <param name="pkValues">A list of primary keys</param>
		/// <returns>The number of objects deleted</returns>
		public static int DeleteAll(Type targetType, IEnumerable pkValues)
		{
			return ActiveRecordBase.DeleteAll(targetType, pkValues);
		}

		/// <summary>
		/// Enumerates the query.
		/// Note: Only use if you expect most of the values to be on the second level cache
		/// </summary>
		/// <param name="q">The query</param>
		/// <returns></returns>
		public static IEnumerable EnumerateQuery(IActiveRecordQuery q)
		{
			return ActiveRecordBase.EnumerateQuery(q);
		}

		/// <summary>
		/// Executes the query
		/// </summary>
		/// <param name="q">The query</param>
		/// <returns></returns>
		public static object ExecuteQuery(IActiveRecordQuery q)
		{
			return ActiveRecordBase.ExecuteQuery(q);
		}

		/// <summary>
		/// Returns the number of records of the specified 
		/// type in the database
		/// </summary>
		/// <example>
		/// <code>
		/// [ActiveRecord]
		/// public class User : ActiveRecordBase
		/// {
		///   ...
		///   
		///   public static int CountUsers()
		///   {
		///     return Count(typeof(User));
		///   }
		/// }
		/// </code>
		/// </example>
		/// <param name="targetType">Type of the target.</param>
		/// <returns>The count result</returns>
		protected internal static int Count(Type targetType)
		{
			return ActiveRecordBase.Count(targetType);
		}

		/// <summary>
		/// Returns the number of records of the specified 
		/// type in the database
		/// </summary>
		/// <example>
		/// <code>
		/// [ActiveRecord]
		/// public class User : ActiveRecordBase
		/// {
		///   ...
		///   
		///   public static int CountUsersLocked()
		///   {
		///     return Count(typeof(User), "IsLocked = ?", true);
		///   }
		/// }
		/// </code>
		/// </example>
		/// <param name="targetType">Type of the target.</param>
		/// <param name="filter">A sql where string i.e. Person=? and DOB &gt; ?</param>
		/// <param name="args">Positional parameters for the filter string</param>
		/// <returns>The count result</returns>
		public static int Count(Type targetType, string filter, params object[] args)
		{
			return ActiveRecordBase.Count(targetType, filter, args);
		}

		/// <summary>
		/// Returns the number of records of the specified 
		/// type in the database
		/// </summary>
		/// <param name="targetType">The target type.</param>
		/// <param name="criteria">The criteria expression</param>
		/// <returns>The count result</returns>
		public static int Count(Type targetType, params ICriterion[] criteria) {
			return ActiveRecordBase.Count(targetType, criteria);
		}

		/// <summary>
		/// Returns the number of records of the specified 
		/// type in the database
		/// </summary>
		/// <param name="targetType">The target type.</param>
		/// <param name="detachedCriteria">The criteria expression</param>
		/// <returns>The count result</returns>
		public static int Count(Type targetType, DetachedCriteria detachedCriteria)
		{
			return ActiveRecordBase.Count(targetType, detachedCriteria);
		}

		/// <summary>
		/// Check if there is any records in the db for the target type
		/// </summary>
		/// <param name="targetType">Type of the target.</param>
		/// <returns><c>true</c> if there's at least one row</returns>
		public static bool Exists(Type targetType)
		{
			return ActiveRecordBase.Exists(targetType);
		}


		/// <summary>
		/// Check if there is any records in the db for the target type
		/// </summary>
		/// <param name="targetType">Type of the target.</param>
		/// <param name="filter">A sql where string i.e. Person=? and DOB &gt; ?</param>
		/// <param name="args">Positional parameters for the filter string</param>
		/// <returns><c>true</c> if there's at least one row</returns>
		public static bool Exists(Type targetType, string filter, params object[] args)
		{
			return ActiveRecordBase.Exists(targetType, filter, args);
		}

		/// <summary>
		/// Check if the <paramref name="id"/> exists in the database.
		/// </summary>
		/// <param name="targetType">Type of the target.</param>
		/// <param name="id">The id to check on</param>
		/// <returns><c>true</c> if the ID exists; otherwise <c>false</c>.</returns>
		public static bool Exists(Type targetType, object id)
		{
			return ActiveRecordBase.Exists(targetType, id);
		}

		/// <summary>
		/// Check if any instance matches the criteria.
		/// </summary>
		/// <returns><c>true</c> if an instance is found; otherwise <c>false</c>.</returns>
		public static bool Exists(Type targetType, params ICriterion[] criterias)
		{
			return ActiveRecordBase.Exists(targetType, criterias);
		}

		/// <summary>
		/// Check if any instance matching the criteria exists in the database.
		/// </summary>
		/// <param name="targetType">The target type.</param>
		/// <param name="detachedCriteria">The criteria expression</param>		
		/// <returns><c>true</c> if an instance is found; otherwise <c>false</c>.</returns>
		public static bool Exists(Type targetType, DetachedCriteria detachedCriteria)
		{
			return ActiveRecordBase.Exists(targetType, detachedCriteria);
		}

		/// <summary>
		/// Saves the instance to the database
		/// </summary>
		/// <param name="instance">The ActiveRecord instance to be deleted</param>
		public static void Save(object instance)
		{
			ActiveRecordBase.Save(instance);
		}

		/// <summary>
		/// Saves the instance to the database and flushes the session. If the primary key is unitialized
		/// it creates the instance on the database. Otherwise it updates it.
		/// <para>
		/// If the primary key is assigned, then you must invoke <see cref="Create(object)"/>
		/// or <see cref="Update(object)"/> instead.
		/// </para>
		/// </summary>
		/// <param name="instance">The ActiveRecord instance to be saved</param>
		public static void SaveAndFlush(object instance)
		{
			ActiveRecordBase.SaveAndFlush(instance);
		}

        /// <summary>
        /// Saves a copy of instance to the database
        /// </summary>
        /// <param name="instance">The transient instance to be copied</param>
        /// <returns>The saved ActiveRecord instance</returns>
        public static object SaveCopy(object instance)
        {
            return ActiveRecordBase.SaveCopy(instance);
        }

        /// <summary>
        /// Saves a copy of the instance to the database and flushes the session. If the primary key is unitialized
        /// it creates the instance on the database. Otherwise it updates it.
        /// <para>
        /// If the primary key is assigned, then you must invoke <see cref="Create(object)"/>
        /// or <see cref="Update(object)"/> instead.
        /// </para>
        /// </summary>
        /// <param name="instance">The transient instance to be copied</param>
        /// <returns>The saved ActiveRecord instance</returns>
        public static void SaveCopyAndFlush(object instance)
        {
            ActiveRecordBase.SaveCopyAndFlush(instance);
        }

		/// <summary>
		/// Creates (Saves) a new instance to the database.
		/// </summary>
		/// <param name="instance">The ActiveRecord instance to be deleted</param>
		public static void Create(object instance)
		{
			ActiveRecordBase.Create(instance);
		}

		/// <summary>
		/// Creates (Saves) a new instance to the database and flushes the session.
		/// </summary>
		/// <param name="instance">The ActiveRecord instance to be created on the database</param>
		public static void CreateAndFlush(object instance)
		{
			ActiveRecordBase.CreateAndFlush(instance);
		}

		/// <summary>
		/// Persists the modification on the instance
		/// state to the database.
		/// </summary>
		/// <param name="instance">The ActiveRecord instance to be deleted</param>
		public static void Update(object instance)
		{
			ActiveRecordBase.Update(instance);
		}

		/// <summary>
		/// Persists the modification on the instance
		/// state to the database and flushes the session.
		/// </summary>
		/// <param name="instance">The ActiveRecord instance to be updated on the database</param>
		public static void UpdateAndFlush(object instance)
		{
			ActiveRecordBase.UpdateAndFlush(instance);
		}

		/// <summary>
		/// Deletes the instance from the database.
		/// </summary>
		/// <param name="instance">The ActiveRecord instance to be deleted</param>
		public static void Delete(object instance)
		{
			ActiveRecordBase.Delete(instance);
		}

		/// <summary>
		/// Deletes the instance from the database and flushes the session.
		/// </summary>
		/// <param name="instance">The ActiveRecord instance to be deleted</param>
		public static void DeleteAndFlush(object instance)
		{
			ActiveRecordBase.DeleteAndFlush(instance);
		}

		/// <summary>
		/// Refresh the instance from the database.
		/// </summary>
		/// <param name="instance">The ActiveRecord instance to be reloaded</param>
		public static void Refresh(object instance)
		{
			ActiveRecordBase.Refresh(instance);
		}

		/// <summary>
		/// Testing hock only.
		/// </summary>
		public static ISessionFactoryHolder GetSessionFactoryHolder()
		{
			return ActiveRecordBase.holder;
		}

		/// <summary>
		/// From NHibernate documentation: 
		/// Persist all reachable transient objects, reusing the current identifier 
		/// values. Note that this will not trigger the Interceptor of the Session.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <param name="replicationMode">The replication mode.</param>
		public static void Replicate(object instance, ReplicationMode replicationMode)
		{
			ActiveRecordBase.Replicate(instance, replicationMode);
		}

		/// <summary>
		/// Evicts the specified instance from the first level cache (session level).
		/// </summary>
		/// <param name="instance">The instance.</param>
		public static void Evict(object instance)
		{
			if (instance == null) throw new ArgumentNullException("instance");

			if (SessionScope.Current != null)
			{
				SessionScope.Current.Evict(instance);
			}
		}

		/// <summary>
		/// Evicts the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		public static void GlobalEvict(Type type)
		{
			if (type == null) throw new ArgumentNullException("type");

			ISessionFactory factory = GetFactory(type);

			factory.Evict(type);
		}

		/// <summary>
		/// Evicts the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="id">The id.</param>
		public static void GlobalEvict(Type type, object id)
		{
			if (type == null) throw new ArgumentNullException("type");

			ISessionFactory factory = ActiveRecordBase.holder.GetSessionFactory(type);

			if (factory == null)
			{
				throw new ActiveRecordException("Could not find registered session factory for type " + type.FullName);
			}

			factory.Evict(type, id);
		}

		/// <summary> 
		/// From NH docs: Evict all entries from the second-level cache. This method occurs outside
		/// of any transaction; it performs an immediate "hard" remove, so does not respect
		/// any transaction isolation semantics of the usage strategy. Use with care.
		/// </summary>
		public static void EvictEntity(string entityName)
		{
			ISessionFactory[] factories = ActiveRecordBase.holder.GetSessionFactories();

			foreach(ISessionFactory factory in factories)
			{
				factory.EvictEntity(entityName);
			}
		}

		/* NOT AVAILABLE ON NH 2.0.x

		/// <summary>
		/// From NH docs: Evict an entry from the second-level  cache. This method occurs outside
		/// of any transaction; it performs an immediate "hard" remove, so does not respect
		/// any transaction isolation semantics of the usage strategy. Use with care.
		/// </summary>
		/// <param name="entityName">Name of the entity.</param>
		/// <param name="id">The id.</param>
		public static void EvictEntity(string entityName, object id)
		{
			ISessionFactory[] factories = ActiveRecordBase.holder.GetSessionFactories();

			foreach(ISessionFactory factory in factories)
			{
				factory.EvictEntity(entityName, id);
			}
		}
		*/

		/// <summary>
		/// From NH docs: Evict all entries from the process-level cache.  This method occurs outside
		/// of any transaction; it performs an immediate "hard" remove, so does not respect
		/// any transaction isolation semantics of the usage strategy.  Use with care.
		/// </summary>
		/// <param name="roleName">Name of the role.</param>
		public static void EvictCollection(string roleName)
		{
			ISessionFactory[] factories = ActiveRecordBase.holder.GetSessionFactories();

			foreach(ISessionFactory factory in factories)
			{
				factory.EvictCollection(roleName);
			}
		}

		/// <summary>
		/// From NH docs: Evict an entry from the process-level cache.  This method occurs outside
		/// of any transaction; it performs an immediate "hard" remove, so does not respect
		/// any transaction isolation semantics of the usage strategy.  Use with care.
		/// </summary>
		/// <param name="roleName">Name of the role.</param>
		/// <param name="id">The id.</param>
		public static void EvictCollection(string roleName, object id)
		{
			ISessionFactory[] factories = ActiveRecordBase.holder.GetSessionFactories();

			foreach(ISessionFactory factory in factories)
			{
				factory.EvictCollection(roleName, id);
			}
		}

		/// <summary>
		/// From NH docs: Evict any query result sets cached in the named query cache region.
		/// </summary>
		/// <param name="cacheRegion">The cache region.</param>
		public static void EvictQueries(string cacheRegion)
		{
			ISessionFactory[] factories = ActiveRecordBase.holder.GetSessionFactories();

			foreach(ISessionFactory factory in factories)
			{
				factory.EvictQueries(cacheRegion);
			}
		}

		private static ISessionFactory GetFactory(Type type)
		{
			ISessionFactory factory = ActiveRecordBase.holder.GetSessionFactory(type);

			if (factory == null)
			{
				throw new ActiveRecordException("Could not find registered session factory for type " + type.FullName);
			}
			return factory;
		}
	}
}
