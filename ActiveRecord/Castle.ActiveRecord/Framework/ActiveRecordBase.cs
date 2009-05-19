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
	using System.Collections.Generic;
	using Castle.ActiveRecord.Framework;
	using Castle.ActiveRecord.Framework.Internal;
	using Castle.ActiveRecord.Queries;

	using NHibernate;
	using NHibernate.Criterion;
	using Castle.Components.Validator;

	/// <summary>
	/// Allow custom executions using the NHibernate's ISession.
	/// </summary>
	public delegate object NHibernateDelegate(ISession session, object instance);

	/// <summary>
	/// Base class for all ActiveRecord classes. Implements 
	/// all the functionality to simplify the code on the 
	/// subclasses.
	/// </summary>
	[Serializable]
	public abstract class ActiveRecordBase : ActiveRecordHooksBase
	{
		/// <summary>
		/// The global holder for the session factories.
		/// </summary>
		protected internal static ISessionFactoryHolder holder;

		#region internal static

		internal static void EnsureInitialized(Type type)
		{
			if (holder == null)
			{
				String message = String.Format("An ActiveRecord class ({0}) was used but the framework seems not " +
											   "properly initialized. Did you forget about ActiveRecordStarter.Initialize() ?",
											   type.FullName);
				throw new ActiveRecordException(message);
			}
			if (type != typeof(ActiveRecordBase) && GetModel(type) == null)
			{
				String message = String.Format("You have accessed an ActiveRecord class that wasn't properly initialized. " +
											   "There are two possible explanations: that the call to ActiveRecordStarter.Initialize() didn't include {0} class, or that {0} class is not decorated with the [ActiveRecord] attribute.",
											   type.FullName);
				throw new ActiveRecordException(message);
			}
		}

		/// <summary>
		/// Internally used
		/// </summary>
		/// <param name="arType">The type.</param>
		/// <param name="model">The model.</param>
		internal static void Register(Type arType, ActiveRecordModel model)
		{
			ActiveRecordModel.Register(arType, model);
		}

		/// <summary>
		/// Internally used
		/// </summary>
		/// <param name="arType">The type.</param>
		/// <returns>An <see cref="ActiveRecordModel"/></returns>
		internal static ActiveRecordModel GetModel(Type arType)
		{
			return ActiveRecordModel.GetModel(arType);
		}

		#endregion

		#region protected internal static

		#region Create/Update/Save/Delete/DeleteAll/Refresh

		#region Create

		/// <summary>
		/// Creates (Saves) a new instance to the database.
		/// </summary>
		/// <param name="instance">The ActiveRecord instance to be created on the database</param>
		protected internal static void Create(object instance)
		{
			InternalCreate(instance, false);
		}

		/// <summary>
		/// Creates (Saves) a new instance to the database and flushes the session.
		/// </summary>
		/// <param name="instance">The ActiveRecord instance to be created on the database</param>
		protected internal static void CreateAndFlush(object instance)
		{
			InternalCreate(instance, true);
		}

		/// <summary>
		/// Creates (Saves) a new instance to the database.
		/// </summary>
		/// <param name="instance">The ActiveRecord instance to be created on the database</param>
		/// <param name="flush">if set to <c>true</c>, the operation will be followed by a session flush.</param>
		private static void InternalCreate(object instance, bool flush)
		{
			if (instance == null) throw new ArgumentNullException("instance");

			EnsureInitialized(instance.GetType());

			ISession session = holder.CreateSession(instance.GetType());

			try
			{
				session.Save(instance);

				if (flush)
				{
					session.Flush();
				}
			}
			catch(Exception ex)
			{
				holder.FailSession(session);

				// NHibernate catches our ValidationException, and as such it is the innerexception here
				if (ex is ValidationException)
					throw ;

				if (ex.InnerException is ValidationException)
					throw ex.InnerException;

				throw new ActiveRecordException("Could not perform Create for " + instance.GetType().Name, ex);

			}
			finally
			{
				holder.ReleaseSession(session);
			}
		}

		#endregion

		#region Delete

		/// <summary>
		/// Deletes the instance from the database.
		/// </summary>
		/// <param name="instance">The ActiveRecord instance to be deleted</param>
		protected internal static void Delete(object instance)
		{
			InternalDelete(instance, false);
		}

		/// <summary>
		/// Deletes the instance from the database and flushes the session.
		/// </summary>
		/// <param name="instance">The ActiveRecord instance to be deleted</param>
		protected internal static void DeleteAndFlush(object instance)
		{
			InternalDelete(instance, true);
		}

		/// <summary>
		/// Deletes the instance from the database.
		/// </summary>
		/// <param name="instance">The ActiveRecord instance to be deleted</param>
		/// <param name="flush">if set to <c>true</c>, the operation will be followed by a session flush.</param>
		private static void InternalDelete(object instance, bool flush)
		{
			if (instance == null) throw new ArgumentNullException("instance");

			EnsureInitialized(instance.GetType());

			ISession session = holder.CreateSession(instance.GetType());

			try
			{
				session.Delete(instance);

				if (flush)
				{
					session.Flush();
				}
			}
			catch (ValidationException)
			{
				holder.FailSession(session);

				throw;

			}
			catch (Exception ex)
			{
				holder.FailSession(session);

				throw new ActiveRecordException("Could not perform Delete for " + instance.GetType().Name, ex);
			}
			finally
			{
				holder.ReleaseSession(session);
			}
		}

		#endregion

		#region Replicate

		/// <summary>
		/// From NHibernate documentation: 
		/// Persist all reachable transient objects, reusing the current identifier 
		/// values. Note that this will not trigger the Interceptor of the Session.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <param name="replicationMode">The replication mode.</param>
		protected internal static void Replicate(object instance, ReplicationMode replicationMode)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}

			EnsureInitialized(instance.GetType());

			ISession session = holder.CreateSession(instance.GetType());

			try
			{
				session.Replicate(instance, replicationMode);
			}
			catch(Exception ex)
			{
				holder.FailSession(session);

				// NHibernate catches our ValidationException, and as such it is the innerexception here
				if (ex.InnerException is ValidationException)
				{
					throw ex.InnerException;
				}
				else
				{
					throw new ActiveRecordException("Could not perform Replicate for " + instance.GetType().Name, ex);
				}
			}
			finally
			{
				holder.ReleaseSession(session);
			}
		}

		#endregion

		#region Refresh

		/// <summary>
		/// Refresh the instance from the database.
		/// </summary>
		/// <param name="instance">The ActiveRecord instance to be reloaded</param>
		protected internal static void Refresh(object instance)
		{
			if (instance == null) throw new ArgumentNullException("instance");

			EnsureInitialized(instance.GetType());

			ISession session = holder.CreateSession(instance.GetType());

			try
			{
				session.Refresh(instance);
			}
			catch (Exception ex)
			{
				holder.FailSession(session);

				// NHibernate catches our ValidationException, and as such it is the innerexception here
				if (ex.InnerException is ValidationException)
				{
					throw ex.InnerException;
				}
				else
				{
					throw new ActiveRecordException("Could not perform Refresh for " + instance.GetType().Name, ex);
				}
			}
			finally
			{
				holder.ReleaseSession(session);
			}
		}

		#endregion

		#region DeleteAll

		/// <summary>
		/// Deletes all rows for the specified ActiveRecord type
		/// </summary>
		/// <remarks>
		/// This method is usually useful for test cases.
		/// </remarks>
		/// <param name="type">ActiveRecord type on which the rows on the database should be deleted</param>
		protected internal static void DeleteAll(Type type)
		{
			EnsureInitialized(type);

			ISession session = holder.CreateSession(type);

			try
			{
				session.Delete(String.Format("from {0}", type.Name));

				session.Flush();
			}
			catch (ValidationException)
			{
				holder.FailSession(session);

				throw;
			}
			catch (Exception ex)
			{
				holder.FailSession(session);

				throw new ActiveRecordException("Could not perform DeleteAll for " + type.Name, ex);
			}
			finally
			{
				holder.ReleaseSession(session);
			}
		}

		/// <summary>
		/// Deletes all rows for the specified ActiveRecord type that matches
		/// the supplied HQL condition
		/// </summary>
		/// <remarks>
		/// This method is usually useful for test cases.
		/// </remarks>
		/// <param name="type">ActiveRecord type on which the rows on the database should be deleted</param>
		/// <param name="where">HQL condition to select the rows to be deleted</param>
		protected internal static void DeleteAll(Type type, String where)
		{
			EnsureInitialized(type);

			ISession session = holder.CreateSession(type);

			try
			{
				session.Delete(String.Format("from {0} where {1}", type.Name, where));

				session.Flush();
			}
			catch (ValidationException)
			{
				holder.FailSession(session);

				throw;
			}
			catch (Exception ex)
			{
				holder.FailSession(session);

				throw new ActiveRecordException("Could not perform DeleteAll for " + type.Name, ex);
			}
			finally
			{
				holder.ReleaseSession(session);
			}
		}

		/// <summary>
		/// Deletes all <paramref name="targetType" /> objects, based on the primary keys
		/// supplied on <paramref name="pkValues" />.
		/// </summary>
		/// <param name="targetType">The target ActiveRecord type</param>
		/// <param name="pkValues">A list of primary keys</param>
		/// <returns>The number of objects deleted</returns>
		protected internal static int DeleteAll(Type targetType, IEnumerable pkValues)
		{
			if (pkValues == null)
			{
				return 0;
			}

			int counter = 0;

			foreach (object pk in pkValues)
			{
				Object obj = FindByPrimaryKey(targetType, pk, false);

				if (obj != null)
				{
					ActiveRecordBase arBase = obj as ActiveRecordBase;

					if (arBase != null)
					{
						arBase.Delete(); // in order to allow override of the virtual "Delete()" method
					}
					else
					{
						Delete(obj);
					}

					counter++;
				}
			}

			return counter;
		}

		#endregion

		#region Update

		/// <summary>
		/// Persists the modification on the instance
		/// state to the database.
		/// </summary>
		/// <param name="instance">The ActiveRecord instance to be updated on the database</param>
		protected internal static void Update(object instance)
		{
			InternalUpdate(instance, false);
		}

		/// <summary>
		/// Persists the modification on the instance
		/// state to the database and flushes the session.
		/// </summary>
		/// <param name="instance">The ActiveRecord instance to be updated on the database</param>
		protected internal static void UpdateAndFlush(object instance)
		{
			InternalUpdate(instance, true);
		}

		/// <summary>
		/// Persists the modification on the instance
		/// state to the database.
		/// </summary>
		/// <param name="instance">The ActiveRecord instance to be updated on the database</param>
		/// <param name="flush">if set to <c>true</c>, the operation will be followed by a session flush.</param>
		private static void InternalUpdate(object instance, bool flush)
		{
			if (instance == null) throw new ArgumentNullException("instance");

			EnsureInitialized(instance.GetType());

			ISession session = holder.CreateSession(instance.GetType());

			try
			{
				session.Update(instance);

				if (flush)
				{
					session.Flush();
				}
			}
			catch (ValidationException)
			{
				holder.FailSession(session);

				throw;
			}
			catch (Exception ex)
			{
				holder.FailSession(session);

				throw new ActiveRecordException("Could not perform Update for " + instance.GetType().Name, ex);
			}
			finally
			{
				holder.ReleaseSession(session);
			}
		}

		#endregion

		#region Save

		/// <summary>
		/// Saves the instance to the database. If the primary key is unitialized
		/// it creates the instance on the database. Otherwise it updates it.
		/// <para>
		/// If the primary key is assigned, then you must invoke <see cref="Create()"/>
		/// or <see cref="Update()"/> instead.
		/// </para>
		/// </summary>
		/// <param name="instance">The ActiveRecord instance to be saved</param>
		protected internal static void Save(object instance)
		{
			InternalSave(instance, false);
		}

		/// <summary>
		/// Saves the instance to the database and flushes the session. If the primary key is unitialized
		/// it creates the instance on the database. Otherwise it updates it.
		/// <para>
		/// If the primary key is assigned, then you must invoke <see cref="Create()"/>
		/// or <see cref="Update()"/> instead.
		/// </para>
		/// </summary>
		/// <param name="instance">The ActiveRecord instance to be saved</param>
		protected internal static void SaveAndFlush(object instance)
		{
			InternalSave(instance, true);
		}

        /// <summary>
        /// Saves a copy of the instance to the database. If the primary key is unitialized
        /// it creates the instance on the database. Otherwise it updates it.
        /// <para>
        /// If the primary key is assigned, then you must invoke <see cref="Create()"/>
        /// or <see cref="Update()"/> instead.
        /// </para>
        /// </summary>
        /// <param name="instance">The transient instance to be saved</param>
        /// <returns>The saved ActiveRecord instance</returns>
        protected internal static object SaveCopy(object instance)
        {
            return InternalSaveCopy(instance, false);
        }

        /// <summary>
        /// Saves a copy of the instance to the database and flushes the session. If the primary key is unitialized
        /// it creates the instance on the database. Otherwise it updates it.
        /// <para>
        /// If the primary key is assigned, then you must invoke <see cref="Create()"/>
        /// or <see cref="Update()"/> instead.
        /// </para>
        /// </summary>
        /// <param name="instance">The transient instance to be saved</param>
        /// <returns>The saved ActiveRecord instance</returns>
        protected internal static object SaveCopyAndFlush(object instance)
        {
            return InternalSaveCopy(instance, true);
        }

		/// <summary>
		/// Saves the instance to the database. If the primary key is unitialized
		/// it creates the instance on the database. Otherwise it updates it.
		/// <para>
		/// If the primary key is assigned, then you must invoke <see cref="Create()"/>
		/// or <see cref="Update()"/> instead.
		/// </para>
		/// </summary>
		/// <param name="instance">The ActiveRecord instance to be saved</param>
		/// <param name="flush">if set to <c>true</c>, the operation will be followed by a session flush.</param>
		private static void InternalSave(object instance, bool flush)
		{
			if (instance == null) throw new ArgumentNullException("instance");

			EnsureInitialized(instance.GetType());

			ISession session = holder.CreateSession(instance.GetType());

			try
			{
				session.SaveOrUpdate(instance);

				if (flush)
				{
					session.Flush();
				}
			}
			catch (ValidationException)
			{
				holder.FailSession(session);

				throw;
			}
			catch(Exception ex)
			{
				holder.FailSession(session);

				// NHibernate catches our ValidationException on Create so it could be the innerexception here
				if (ex.InnerException is ValidationException)
				{
					throw ex.InnerException;
				}
				else
				{
					throw new ActiveRecordException("Could not perform Save for " + instance.GetType().Name, ex);
				}
			}
			finally
			{
				holder.ReleaseSession(session);
			}
		}

        /// <summary>
        /// Saves a copy of the instance to the database. If the primary key is unitialized
        /// it creates the instance on the database. Otherwise it updates it.
        /// <para>
        /// If the primary key is assigned, then you must invoke <see cref="Create()"/>
        /// or <see cref="Update()"/> instead.
        /// </para>
        /// </summary>
        /// <param name="instance">The transient instance to be saved</param>
        /// <param name="flush">if set to <c>true</c>, the operation will be followed by a session flush.</param>
        /// <returns>The saved ActiveRecord instance.</returns>
        private static object InternalSaveCopy(object instance, bool flush)
        {
            if (instance == null) throw new ArgumentNullException("instance");

            EnsureInitialized(instance.GetType());

            ISession session = holder.CreateSession(instance.GetType());

            try
            {
                object persistent = session.SaveOrUpdateCopy(instance);

                if (flush)
                {
                    session.Flush();
                }

                return persistent;
            }
            catch (ValidationException)
            {
                holder.FailSession(session);

                throw;
            }
            catch (Exception ex)
            {
                holder.FailSession(session);

                // NHibernate catches our ValidationException on Create so it could be the innerexception here
                if (ex.InnerException is ValidationException)
                {
                    throw ex.InnerException;
                }
                else
                {
                    throw new ActiveRecordException("Could not perform SaveCopy for " + instance.GetType().Name, ex);
                }
            }
            finally
            {
                holder.ReleaseSession(session);
            }
        }

		#endregion

		#endregion

		#region Execute

		/// <summary>
		/// Invokes the specified delegate passing a valid 
		/// NHibernate session. Used for custom NHibernate queries.
		/// </summary>
		/// <param name="targetType">The target ActiveRecordType</param>
		/// <param name="call">The delegate instance</param>
		/// <param name="instance">The ActiveRecord instance</param>
		/// <returns>Whatever is returned by the delegate invocation</returns>
		protected internal static object Execute(Type targetType, NHibernateDelegate call, object instance)
		{
			if (targetType == null) throw new ArgumentNullException("targetType", "Target type must be informed");
			if (call == null) throw new ArgumentNullException("call", "Delegate must be passed");

			EnsureInitialized(targetType);

			ISession session = holder.CreateSession(targetType);

			try
			{
				return call(session, instance);
			}
			catch (ValidationException)
			{
				holder.FailSession(session);

				throw;
			}
			catch (Exception ex)
			{
				holder.FailSession(session);

				throw new ActiveRecordException("Error performing Execute for " + targetType.Name, ex);
			}
			finally
			{
				holder.ReleaseSession(session);
			}
		}

		#endregion

		#region ExecuteQuery

		/// <summary>
		/// Enumerates the query
		/// Note: only use if you expect most of the values to exist on the second level cache.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <returns>An <see cref="IEnumerable"/></returns>
		protected internal static IEnumerable EnumerateQuery(IActiveRecordQuery query)
		{
			Type rootType = query.RootType;

			EnsureInitialized(rootType);

			ISession session = holder.CreateSession(rootType);

			try
			{
				return query.Enumerate(session);
			}
			catch (Exception ex)
			{
				holder.FailSession(session);

				throw new ActiveRecordException("Could not perform EnumerateQuery for " + rootType.Name, ex);
			}
			finally
			{
				holder.ReleaseSession(session);
			}
		}

		#endregion

		#region ExecuteQuery

		/// <summary>
		/// Executes the query.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <returns>The query result.</returns>
		public static object ExecuteQuery(IActiveRecordQuery query)
		{
			Type rootType = query.RootType;

			EnsureInitialized(rootType);

			ISession session = holder.CreateSession(rootType);

			try
			{
				return query.Execute(session);
			}
			catch (Exception ex)
			{
				holder.FailSession(session);

				throw new ActiveRecordException("Could not perform ExecuteQuery for " + rootType.Name, ex);
			}
			finally
			{
				holder.ReleaseSession(session);
			}
		}

		#endregion

		#region Count

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
		///   public static int CountAllUsers()
		///   {
		///     return Count(typeof(User));
		///   }
		/// }
		/// </code>
		/// </example>
		/// <param name="targetType">The target type.</param>
		/// <returns>The count result</returns>
		protected internal static int Count(Type targetType)
		{
			CountQuery query = new CountQuery(targetType);

			return (int)ExecuteQuery(query);
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
		///   public static int CountAllUsersLocked()
		///   {
		///     return Count(typeof(User), "IsLocked = ?", true);
		///   }
		/// }
		/// </code>
		/// </example>
		/// <param name="targetType">The target type.</param>
		/// <param name="filter">A sql where string i.e. Person=? and DOB &gt; ?</param>
		/// <param name="args">Positional parameters for the filter string</param>
		/// <returns>The count result</returns>
		protected internal static int Count(Type targetType, string filter, params object[] args)
		{
			CountQuery query = new CountQuery(targetType, filter, args);

			return (int)ExecuteQuery(query);
		}

		/// <summary>
		/// Returns the number of records of the specified 
		/// type in the database
		/// </summary>
		/// <param name="targetType">The target type.</param>
		/// <param name="criteria">The criteria expression</param>
		/// <returns>The count result</returns>
		protected internal static int Count(Type targetType, ICriterion[] criteria)
		{
			CountQuery query = new CountQuery(targetType, criteria);

			return (int) ExecuteQuery(query);
		}

		/// <summary>
		/// Returns the number of records of the specified 
		/// type in the database
		/// </summary>
		/// <param name="targetType">The target type.</param>
		/// <param name="detachedCriteria">The criteria expression</param>
		/// <returns>The count result</returns>
		protected internal static int Count(Type targetType, DetachedCriteria detachedCriteria)
		{
			CountQuery query = new CountQuery(targetType, detachedCriteria);

			return (int)ExecuteQuery(query);
		}

		#endregion

		#region Exists

		/// <summary>
		/// Check if there is any records in the db for the target type
		/// </summary>
		/// <param name="targetType">The target type.</param>
		/// <returns><c>true</c> if there's at least one row</returns>
		protected internal static bool Exists(Type targetType)
		{
			return Count(targetType) > 0;
		}

		/// <summary>
		/// Check if there is any records in the db for the target type
		/// </summary>
		/// <param name="targetType">The target type.</param>
		/// <param name="filter">A sql where string i.e. Person=? and DOB &gt; ?</param>
		/// <param name="args">Positional parameters for the filter string</param>
		/// <returns><c>true</c> if there's at least one row</returns>
		protected internal static bool Exists(Type targetType, string filter, params object[] args)
		{
			return Count(targetType, filter, args) > 0;
		}

		/// <summary>
		/// Check if the <paramref name="id"/> exists in the database.
		/// </summary>
		/// <param name="targetType">The target type.</param>
		/// <param name="id">The id to check on</param>
		/// <returns><c>true</c> if the ID exists; otherwise <c>false</c>.</returns>
		protected internal static bool Exists(Type targetType, object id)
		{
			EnsureInitialized(targetType);
			ISession session = holder.CreateSession(targetType);
			
			try
			{
				return session.Get(targetType, id) != null;	
			}
			catch(Exception ex)
			{
					throw new ActiveRecordException("Could not perform Exists for " + targetType.Name + ". Id: " + id, ex);
			}
			finally
			{
				holder.ReleaseSession(session);
			}
		}

		/// <summary>
		/// Check if any instance matching the criteria exists in the database.
		/// </summary>
		/// <param name="targetType">The target type.</param>
		/// <param name="criteria">The criteria expression</param>		
		/// <returns><c>true</c> if an instance is found; otherwise <c>false</c>.</returns>
		protected internal static bool Exists(Type targetType, params ICriterion[] criteria)
		{
			return Count(targetType, criteria) > 0;
		}

		/// <summary>
		/// Check if any instance matching the criteria exists in the database.
		/// </summary>
		/// <param name="targetType">The target type.</param>
		/// <param name="detachedCriteria">The criteria expression</param>		
		/// <returns><c>true</c> if an instance is found; otherwise <c>false</c>.</returns>
		protected internal static bool Exists(Type targetType, DetachedCriteria detachedCriteria)
		{
			return Count(targetType, detachedCriteria) > 0;
		}
		
		#endregion

		#region FindAll

		/// <summary>
		/// Returns all instances found for the specified type according to the criteria
		/// </summary>
		/// <param name="targetType">The target type.</param>
		/// <param name="detachedCriteria">The criteria.</param>
		/// <param name="orders">An <see cref="Array"/> of <see cref="Order"/> objects.</param>
		/// <returns>The <see cref="Array"/> of results.</returns>
		protected internal static Array FindAll(Type targetType, DetachedCriteria detachedCriteria, params Order[] orders)
		{
			EnsureInitialized(targetType);

			ISession session = holder.CreateSession(targetType);
			
			try
			{
				ICriteria criteria = detachedCriteria.GetExecutableCriteria(session);

				AddOrdersToCriteria(criteria, orders);

				return SupportingUtils.BuildArray(targetType, criteria.List());
			}
			catch(ValidationException)
			{
				holder.FailSession(session);

				throw;
			}
			catch(Exception ex)
			{
				holder.FailSession(session);

				throw new ActiveRecordException("Could not perform FindAll for " + targetType.Name, ex);
			}
			finally
			{
				holder.ReleaseSession(session);
			}
		}

		/// <summary>
		/// Returns all instances found for the specified type.
		/// </summary>
		/// <param name="targetType">The target type.</param>
		/// <returns>The <see cref="Array"/> of results</returns>
		protected internal static Array FindAll(Type targetType)
		{
			return FindAll(targetType, (Order[])null);
		}

		/// <summary>
		/// Returns all instances found for the specified type 
		/// using sort orders and criteria.
		/// </summary>
		/// <param name="targetType">The The target type.</param>
		/// <param name="orders">An <see cref="Array"/> of <see cref="Order"/> objects.</param>
		/// <param name="criteria">The criteria expression</param>
		/// <returns>The <see cref="Array"/> of results.</returns>
		protected internal static Array FindAll(Type targetType, Order[] orders, params ICriterion[] criteria)
		{
			EnsureInitialized(targetType);

			ISession session = holder.CreateSession(targetType);

			try
			{
				ICriteria sessionCriteria = session.CreateCriteria(targetType);

				foreach(ICriterion cond in criteria)
				{
					sessionCriteria.Add(cond);
				}

				AddOrdersToCriteria(sessionCriteria, orders);

				return SupportingUtils.BuildArray(targetType, sessionCriteria.List());
			}
			catch (ValidationException)
			{
				holder.FailSession(session);

				throw;
			}
			catch (Exception ex)
			{
				holder.FailSession(session);

				throw new ActiveRecordException("Could not perform FindAll for " + targetType.Name, ex);
			}
			finally
			{
				holder.ReleaseSession(session);
			}
		}

		private static void AddOrdersToCriteria(ICriteria criteria, IEnumerable<Order> orders)
		{
			if (orders != null)
			{
				foreach (Order order in orders)
				{
					criteria.AddOrder(order);
				}
			}
		}

		/// <summary>
		/// Returns all instances found for the specified type 
		/// using criteria.
		/// </summary>
		/// <param name="targetType">The target type.</param>
		/// <param name="criteria">The criteria expression</param>
		/// <returns>The <see cref="Array"/> of results.</returns>
		protected internal static Array FindAll(Type targetType, params ICriterion[] criteria)
		{
			return FindAll(targetType, null, criteria);
		}

		#endregion

		#region FindAllByProperty

		/// <summary>
		/// Finds records based on a property value - automatically converts null values to IS NULL style queries. 
		/// </summary>
		/// <param name="targetType">The target type</param>
		/// <param name="property">A property name (not a column name)</param>
		/// <param name="value">The value to be equals to</param>
		/// <returns>The <see cref="Array"/> of results.</returns>
		protected internal static Array FindAllByProperty(Type targetType, String property, object value)
		{
			ICriterion criteria = (value == null) ? Expression.IsNull(property) : Expression.Eq(property, value);
			return FindAll(targetType, criteria);
		}

		/// <summary>
		/// Finds records based on a property value - automatically converts null values to IS NULL style queries. 
		/// </summary>
		/// <param name="targetType">The target type</param>
		/// <param name="orderByColumn">The column name to be ordered ASC</param>
		/// <param name="property">A property name (not a column name)</param>
		/// <param name="value">The value to be equals to</param>
		/// <returns>The <see cref="Array"/> of results.</returns>
		protected internal static Array FindAllByProperty(Type targetType, String orderByColumn, String property, object value)
		{
			ICriterion criteria = (value == null) ? Expression.IsNull(property) : Expression.Eq(property, value);
			return FindAll(targetType, new Order[] {Order.Asc(orderByColumn)}, criteria);
		}

		#endregion

		#region FindByPrimaryKey

		/// <summary>
		/// Finds an object instance by an unique ID
		/// </summary>
		/// <param name="targetType">The AR subclass type</param>
		/// <param name="id">ID value</param>
		/// <returns>The object instance.</returns>
		protected internal static object FindByPrimaryKey(Type targetType, object id)
		{
			return FindByPrimaryKey(targetType, id, true);
		}

		/// <summary>
		/// Finds an object instance by an unique ID
		/// </summary>
		/// <param name="targetType">The AR subclass type</param>
		/// <param name="id">ID value</param>
		/// <param name="throwOnNotFound"><c>true</c> if you want to catch an exception 
		/// if the object is not found</param>
		/// <returns>The object instance.</returns>
		/// <exception cref="ObjectNotFoundException">if <c>throwOnNotFound</c> is set to 
		/// <c>true</c> and the row is not found</exception>
		protected internal static object FindByPrimaryKey(Type targetType, object id, bool throwOnNotFound)
		{
			EnsureInitialized(targetType);
			bool hasScope = holder.ThreadScopeInfo.HasInitializedScope;
			ISession session = holder.CreateSession(targetType);

			try
			{
				object loaded;
				// Load() and Get() has different semantics with regard to the way they
				// handle null values, Get() _must_ check that the value exists, Load() is allowed
				// to return an uninitialized proxy that will throw when you access it later.
				// in order to play well with proxies, we need to use this approach.
				if (throwOnNotFound)
				{
					loaded = session.Load(targetType, id);
				}
				else
				{
					loaded = session.Get(targetType, id);
				}
				//If we are not in a scope, we want to initialize the entity eagerly, since other wise the 
				//user will get an exception when it access the entity's property, and it will try to lazy load itself and find that
				//it has no session.
				//If we are in a scope, it is the user responsability to keep the scope alive if he wants to use 
				if (!hasScope)
				{
					NHibernateUtil.Initialize(loaded);
				}
				return loaded;
			}
			catch (ObjectNotFoundException ex)
			{
				holder.FailSession(session);

				String message = String.Format("Could not find {0} with id {1}", targetType.Name, id);
				throw new NotFoundException(message, ex);
			}
			catch (ValidationException)
			{
				holder.FailSession(session);

				throw;
			}
			catch (Exception ex)
			{
				holder.FailSession(session);

				throw new ActiveRecordException("Could not perform FindByPrimaryKey for " + targetType.Name + ". Id: " + id, ex);
			}
			finally
			{
				holder.ReleaseSession(session);
			}
		}

		#endregion

		#region FindFirst

		/// <summary>
		/// Searches and returns the first row.
		/// </summary>
		/// <param name="targetType">The target type.</param>
		/// <param name="detachedCriteria">The criteria.</param>
		/// <param name="orders">The sort order - used to determine which record is the first one.</param>
		/// <returns>A <c>targetType</c> instance or <c>null.</c></returns>
		protected internal static object FindFirst(Type targetType, DetachedCriteria detachedCriteria, params Order[] orders)
		{
			Array result = SlicedFindAll(targetType, 0, 1, orders, detachedCriteria);
			return (result != null && result.Length > 0 ? result.GetValue(0) : null);
		}

		/// <summary>
		/// Searches and returns the first row.
		/// </summary>
		/// <param name="targetType">The target type</param>
		/// <param name="orders">The sort order - used to determine which record is the first one</param>
		/// <param name="criteria">The criteria expression</param>
		/// <returns>A <c>targetType</c> instance or <c>null</c></returns>
		protected internal static object FindFirst(Type targetType, Order[] orders, params ICriterion[] criteria)
		{
			Array result = SlicedFindAll(targetType, 0, 1, orders, criteria);
			return (result != null && result.Length > 0 ? result.GetValue(0) : null);
		}

		/// <summary>
		/// Searches and returns the first row.
		/// </summary>
		/// <param name="targetType">The target type</param>
		/// <param name="criteria">The criteria expression</param>
		/// <returns>A <c>targetType</c> instance or <c>null</c></returns>
		protected internal static object FindFirst(Type targetType, params ICriterion[] criteria)
		{
			return FindFirst(targetType, null, criteria);
		}

		#endregion

		#region FindOne

		/// <summary>
		/// Searches and returns a row. If more than one is found, 
		/// throws <see cref="ActiveRecordException"/>
		/// </summary>
		/// <param name="targetType">The target type</param>
		/// <param name="criteria">The criteria expression</param>
		/// <returns>A <c>targetType</c> instance or <c>null</c></returns>
		protected internal static object FindOne(Type targetType, params ICriterion[] criteria)
		{
			Array result = SlicedFindAll(targetType, 0, 2, criteria);

			if (result.Length > 1)
			{
				throw new ActiveRecordException(targetType.Name + ".FindOne returned " + result.Length +
												" rows. Expecting one or none");
			}

			return (result.Length == 0) ? null : result.GetValue(0);
		}

		/// <summary>
		/// Searches and returns a row. If more than one is found, 
		/// throws <see cref="ActiveRecordException"/>
		/// </summary>
		/// <param name="targetType">The target type</param>
		/// <param name="criteria">The criteria</param>
		/// <returns>A <c>targetType</c> instance or <c>null</c></returns>
		protected internal static object FindOne(Type targetType, DetachedCriteria criteria)
		{
			Array result = SlicedFindAll(targetType, 0, 2, criteria);

			if (result.Length > 1)
			{
				throw new ActiveRecordException(targetType.Name + ".FindOne returned " + result.Length +
				                                " rows. Expecting one or none");
			}

			return (result.Length == 0) ? null : result.GetValue(0);
		}

		#endregion

		#region SlicedFindAll

		/// <summary>
		/// Returns a portion of the query results (sliced)
		/// </summary>
		/// <param name="targetType">The target type.</param>
		/// <param name="firstResult">The number of the first row to retrieve.</param>
		/// <param name="maxResults">The maximum number of results retrieved.</param>
		/// <param name="orders">An <see cref="Array"/> of <see cref="Order"/> objects.</param>
		/// <param name="criteria">The criteria expression</param>
		/// <returns>The sliced query results.</returns>
		protected internal static Array SlicedFindAll(Type targetType, int firstResult, int maxResults,
		                                              Order[] orders, params ICriterion[] criteria)
		{
			EnsureInitialized(targetType);

			ISession session = holder.CreateSession(targetType);

			try
			{
				ICriteria sessionCriteria = session.CreateCriteria(targetType);

				foreach(ICriterion cond in criteria)
				{
					sessionCriteria.Add(cond);
				}

				if (orders != null)
				{
					foreach (Order order in orders)
					{
						sessionCriteria.AddOrder(order);
					}
				}

				sessionCriteria.SetFirstResult(firstResult);
				sessionCriteria.SetMaxResults(maxResults);

				return SupportingUtils.BuildArray(targetType, sessionCriteria.List());
			}
			catch (ValidationException)
			{
				holder.FailSession(session);

				throw;
			}
			catch (Exception ex)
			{
				holder.FailSession(session);

				throw new ActiveRecordException("Could not perform SlicedFindAll for " + targetType.Name, ex);
			}
			finally
			{
				holder.ReleaseSession(session);
			}
		}

		/// <summary>
		/// Returns a portion of the query results (sliced)
		/// </summary>
		/// <param name="targetType">The target type.</param>
		/// <param name="firstResult">The number of the first row to retrieve.</param>
		/// <param name="maxResults">The maximum number of results retrieved.</param>
		/// <param name="orders">An <see cref="Array"/> of <see cref="Order"/> objects.</param>
		/// <param name="criteria">The criteria expression</param>
		/// <returns>The sliced query results.</returns>
		protected internal static Array SlicedFindAll(Type targetType, int firstResult, int maxResults,
		                                              Order[] orders, DetachedCriteria criteria)
		{
			EnsureInitialized(targetType);

			ISession session = holder.CreateSession(targetType);

			try
			{
				ICriteria executableCriteria = criteria.GetExecutableCriteria(session);
				AddOrdersToCriteria(executableCriteria, orders);
				executableCriteria.SetFirstResult(firstResult);
				executableCriteria.SetMaxResults(maxResults);

				return SupportingUtils.BuildArray(targetType, executableCriteria.List());
			}
			catch(ValidationException)
			{
				holder.FailSession(session);

				throw;
			}
			catch(Exception ex)
			{
				holder.FailSession(session);

				throw new ActiveRecordException("Could not perform SlicedFindAll for " + targetType.Name, ex);
			}
			finally
			{
				holder.ReleaseSession(session);
			}
		}

		/// <summary>
		/// Returns a portion of the query results (sliced)
		/// </summary>
		/// <param name="targetType">The target type.</param>
		/// <param name="firstResult">The number of the first row to retrieve.</param>
		/// <param name="maxResults">The maximum number of results retrieved.</param>
		/// <param name="criteria">The criteria expression</param>
		/// <returns>The sliced query results.</returns>
		protected internal static Array SlicedFindAll(Type targetType, int firstResult, int maxResults,
		                                              params ICriterion[] criteria)
		{
			return SlicedFindAll(targetType, firstResult, maxResults, null, criteria);
		}

		/// <summary>
		/// Returns a portion of the query results (sliced)
		/// </summary>
		/// <param name="targetType">The target type.</param>
		/// <param name="firstResult">The number of the first row to retrieve.</param>
		/// <param name="maxResults">The maximum number of results retrieved.</param>
		/// <param name="criteria">The criteria expression</param>
		/// <returns>The sliced query results.</returns>
		protected internal static Array SlicedFindAll(Type targetType, int firstResult, int maxResults,
		                                              DetachedCriteria criteria)
		{
			return SlicedFindAll(targetType, firstResult, maxResults, null, criteria);
		}

		#endregion

		#endregion

		#region protected internal

		/// <summary>
		/// Invokes the specified delegate passing a valid 
		/// NHibernate session. Used for custom NHibernate queries.
		/// </summary>
		/// <param name="call">The delegate instance</param>
		/// <returns>Whatever is returned by the delegate invocation</returns>
		protected virtual internal object Execute(NHibernateDelegate call)
		{
			return Execute(GetType(), call, this);
		}

		#endregion

		#region public virtual

		/// <summary>
		/// Saves the instance information to the database.
		/// May Create or Update the instance depending 
		/// on whether it has a valid ID.
		/// </summary>
		/// <remarks>
		/// If within a <see cref="SessionScope"/> the operation
		/// is going to be on hold until NHibernate (or you) decides to flush
		/// the session.
		/// </remarks>
		public virtual void Save()
		{
			Save(this);
		}

		/// <summary>
		/// Saves the instance information to the database.
		/// May Create or Update the instance depending 
		/// on whether it has a valid ID.
		/// </summary>
		/// <remarks>
		/// Even within a <see cref="SessionScope"/> the operation
		/// is going to be flushed immediately. This might have side effects such as
		/// flushing (persisting) others operations that were on hold.
		/// </remarks>
		public virtual void SaveAndFlush()
		{
			SaveAndFlush(this);
		}

        /// <summary>
        /// Saves a copy of the instance information to the database.
        /// May Create or Update the instance depending 
        /// on whether it has a valid ID.
        /// </summary>
        /// <returns>An saved ActiveRecord instance</returns>
        /// <remarks>
        /// If within a <see cref="SessionScope"/> the operation
        /// is going to be on hold until NHibernate (or you) decides to flush
        /// the session.
        /// </remarks>
        public virtual object SaveCopy()
        {
            return SaveCopy(this);
        }

        /// <summary>
        /// Saves a copy of the instance information to the database.
        /// May Create or Update the instance depending 
        /// on whether it has a valid ID.
        /// </summary>
        /// <returns>A saved ActiveRecord instance</returns>
        /// <remarks>
        /// Even within a <see cref="SessionScope"/> the operation
        /// is going to be flushed immediately. This might have side effects such as
        /// flushing (persisting) others operations that were on hold.
        /// </remarks>
        public virtual object SaveCopyAndFlush()
        {
            return SaveCopyAndFlush(this);
        }

		/// <summary>
		/// Creates (Saves) a new instance to the database.
		/// </summary>
		/// <remarks>
		/// If within a <see cref="SessionScope"/> the operation
		/// is going to be on hold until NHibernate (or you) decides to flush
		/// the session.
		/// </remarks>
		public virtual void Create()
		{
			Create(this);
		}

		/// <summary>
		/// Creates (Saves) a new instance to the database.
		/// </summary>
		/// <remarks>
		/// Even within a <see cref="SessionScope"/> the operation
		/// is going to be flushed immediately. This might have side effects such as
		/// flushing (persisting) others operations that were on hold.
		/// </remarks>
		public virtual void CreateAndFlush()
		{
			CreateAndFlush(this);
		}

		/// <summary>
		/// Persists the modification on the instance
		/// state to the database.
		/// </summary>
		/// <remarks>
		/// If within a <see cref="SessionScope"/> the operation
		/// is going to be on hold until NHibernate (or you) decides to flush
		/// the session.
		/// </remarks>
		public virtual void Update()
		{
			Update(this);
		}

		/// <summary>
		/// Persists the modification on the instance
		/// state to the database.
		/// </summary>
		/// <remarks>
		/// Even within a <see cref="SessionScope"/> the operation
		/// is going to be flushed immediately. This might have side effects such as
		/// flushing (persisting) others operations that were on hold.
		/// </remarks>
		public virtual void UpdateAndFlush()
		{
			UpdateAndFlush(this);
		}

		/// <summary>
		/// Deletes the instance from the database.
		/// </summary>
		/// <remarks>
		/// If within a <see cref="SessionScope"/> the operation
		/// is going to be on hold until NHibernate (or you) decides to flush
		/// the session.
		/// </remarks>
		public virtual void Delete()
		{
			Delete(this);
		}

		/// <summary>
		/// Deletes the instance from the database.
		/// </summary>
		/// <remarks>
		/// Even within a <see cref="SessionScope"/> the operation
		/// is going to be flushed immediately. This might have side effects such as
		/// flushing (persisting) others operations that were on hold.
		/// </remarks>
		public virtual void DeleteAndFlush()
		{
			DeleteAndFlush(this);
		}

		/// <summary>
		/// Refresh the instance from the database.
		/// </summary>
		public virtual void Refresh()
		{
			Refresh(this);
		}

		#endregion

		#region public override

		/// <summary>
		/// Return the type of the object with its PK value.
		/// Useful for logging/debugging
		/// </summary>
		/// <returns>A string representation of this object.</returns>
		public override String ToString()
		{
			ActiveRecordModel model = GetModel(GetType());

			if (model == null || model.PrimaryKey == null)
			{
				return base.ToString();
			}

			PrimaryKeyModel pkModel = model.PrimaryKey;

			object pkVal = pkModel.Property.GetValue(this, null);

			return base.ToString() + "#" + pkVal;
		}

		#endregion

		#region Sort Order

		internal static Order[] PropertyNamesToOrderArray(bool asc, params string[] propertyNames) 
		{
			Order[] orders = new Order[propertyNames.Length];

			for (int i = 0; i < propertyNames.Length; i++) 
			{
				orders[i] = new Order(propertyNames[i], asc);
			}
			return orders;
		}

		/// <summary>
		/// Ascending Order
		/// </summary>
		/// <remarks>
		/// Returns an array of Ascending <see cref="Order"/> instances specifing which properties to use to 
		/// order a result.
		/// </remarks>
		/// <param name="propertyNames">List of property names to order by ascending</param>
		/// <returns>Array of <see cref="Order"/> objects suitable for passing to FindAll and variants</returns>
		public static Order[] Asc(params string[] propertyNames) 
		{
			return PropertyNamesToOrderArray(true, propertyNames);
		}

		/// <summary>
		/// Descending Order
		/// </summary>
		/// <remarks>
		/// Returns an array of Descending <see cref="Order"/> instances specifing which properties to use to 
		/// order a result.
		/// </remarks>
		/// <param name="propertyNames">List of property names to order by descending</param>
		/// <returns>Array of <see cref="Order"/> objects suitable for passing to FindAll and variants</returns>
		public static Order[] Desc(params string[] propertyNames) 
		{
			return PropertyNamesToOrderArray(false, propertyNames);
		}

		#endregion
	}
}
