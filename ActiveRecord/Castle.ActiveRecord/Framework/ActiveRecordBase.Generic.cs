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
    using System.Collections.Generic;

	using Castle.ActiveRecord.Framework;
    using Castle.ActiveRecord.Framework.Internal;
    using Castle.ActiveRecord.Queries;

	using NHibernate;
	using NHibernate.Expression;

    /// <summary>
    /// Base class for all ActiveRecord Generic classes. Implements all the functionality to simplify the code on the subclasses.
    /// </summary>
    [Serializable]
    public abstract class ActiveRecordBase<T> : ActiveRecordHooksBase where T : class
	{
        protected internal static ISessionFactoryHolder holder = ActiveRecordBase.holder;

        #region internal static
        internal static void EnsureInitialized()
        {
            Type type = typeof(T);
            if (holder == null)
            {
                String message = String.Format("An ActiveRecord class ({0}) was used but the framework seems not " +
                    "properly initialized. Did you forget about ActiveRecordStarter.Initialize() ?", type.FullName);
                throw new ActiveRecordException(message);
            }
            if (type != typeof(ActiveRecordBase<T>) && Framework.Internal.ActiveRecordModel.GetModel(type) == null)
            {
                String message = String.Format("You have accessed an ActiveRecord class that wasn't properly initialized. " +
                    "The only explanation is that the call to ActiveRecordStarter.Initialize() didn't include {0} class", type.FullName);
                throw new ActiveRecordException(message);
            }
        }

        /// <summary>
        /// Internally used
        /// </summary>
        /// <param name="arType"></param>
        /// <param name="model"></param>
        internal static void Register(Type arType, Framework.Internal.ActiveRecordModel model)
        {
            Framework.Internal.ActiveRecordModel.Register(arType, model);
        }

        #endregion

        #region public static

        #region Find Related

        #region FindAll
        /// <summary>
        /// Returns all instances found for the specified type.
        /// </summary>
        /// <returns></returns>
        public static T[] FindAll()
        {
            return FindAll((Order[])null);
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
            EnsureInitialized();
            Type type = typeof(T);

            ISession session = holder.CreateSession(type);

            try
            {
                ICriteria criteria = session.CreateCriteria(type);

                foreach (ICriterion cond in criterias)
                {
                    criteria.Add(cond);
                }

                if (orders != null)
                {
                    foreach (Order order in orders)
                    {
                        criteria.AddOrder(order);
                    }
                }

                return CreateReturnArray(criteria);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ActiveRecordException("Could not perform FindAll for " + type.Name, ex);
            }
            finally
            {
                holder.ReleaseSession(session);
            }
        }

        /// <summary>
        /// Returns all instances found for the specified type 
        /// using criterias.
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="criterias"></param>
        /// <returns></returns>
        public static T[] FindAll(params ICriterion[] criterias)
        {
            return FindAll(null, criterias);
        }
        #endregion

        #region FindAllByProperty
        /// <summary>
        /// Finds records based on a property value
        /// </summary>
        /// <remarks>
        /// Contributed by someone on the forum
        /// http://forum.castleproject.org/posts/list/300.page
        /// </remarks>
        /// <param name="property">A property name (not a column name)</param>
        /// <param name="value">The value to be equals to</param>
        /// <returns></returns>
        public static T[] FindAllByProperty(String property, object value)
        {
            return FindAll(Expression.Eq(property, value));
        }

        /// <summary>
        /// Finds records based on a property value
        /// </summary>
        /// <param name="property">A property name (not a column name)</param>
        /// <param name="value">The value to be equals to</param>
        /// <param name="orders">The sort order</param>
        /// <returns></returns>
        public static T[] FindAllByProperty(String property, object value, Order[] orders)
        {
            return FindAll(orders, Expression.Eq(property, value));
        }
        #endregion

        #region Find/TryFind
        /// <summary>
        /// Finds an object instance by a unique ID (typically primary key)
        /// </summary>
        /// <param name="id">ID value</param>
        /// <exception cref="ObjectNotFoundException">if the row is not found</exception>
        /// <returns>T</returns>
        public static T Find(object id)
        {
            return FindByPrimaryKey(id, true);
        }

        /// <summary>
        /// Finds an object instance by a unique ID (typically primary key). 
        /// If the row is not found this method will not throw an exception.
        /// </summary>
        /// <param name="id">ID value</param>
        /// <returns></returns>
        public static T TryFind(object id)
        {
            return FindByPrimaryKey(id, false);
        }
        #endregion

        #region FindFirst
        /// <summary>
        /// Searches and returns the first row.
        /// </summary>
        /// <param name="orders">The sort order - used to determine which record is the first one</param>
        /// <param name="criterias">The criteria expression</param>
        /// <returns>A <c>targetType</c> instance or <c>null</c></returns>
        public static T FindFirst(Order[] orders, params ICriterion[] criterias)
        {
            T[] result = SlicedFindAll(0, 1, orders, criterias);
            return (result != default(T) && result.Length > 0 ? result[0] : default(T));
        }

        /// <summary>
        /// Searches and returns the first row.
        /// </summary>
        /// <param name="criterias">The criteria expression</param>
        /// <returns>A <c>targetType</c> instance or <c>null</c></returns>
        public static T FindFirst(params ICriterion[] criterias)
        {
            return FindFirst(null, criterias);
        }
        #endregion

        #region FindOne
        /// <summary>
        /// Searches and returns the a row. If more than one is found, 
        /// throws <see cref="ActiveRecordException"/>
        /// </summary>
        /// <param name="criterias">The criteria expression</param>
        /// <returns>A <c>targetType</c> instance or <c>null</c></returns>
        public static T FindOne(params ICriterion[] criterias)
        {
            T[] result = SlicedFindAll(0, 2, criterias);

            if (result.Length > 1)
            {
                throw new ActiveRecordException(typeof(T).Name + ".FindOne returned " + result.Length + " rows. Expecting one or none");
            }

            return (result.Length == 0) ? default(T) : result[0];
        }
        #endregion

        #region SlicedFindAll
        /// <summary>
        /// Returns a portion of the query results (sliced)
        /// </summary>
        public static T[] SlicedFindAll(int firstResult, int maxresults, Order[] orders, params ICriterion[] criterias)
        {
            EnsureInitialized();

            Type type = typeof(T);

            ISession session = holder.CreateSession(type);
            try
            {
                ICriteria criteria = session.CreateCriteria(type);

                foreach (ICriterion cond in criterias)
                {
                    criteria.Add(cond);
                }

                if (orders != null)
                {
                    foreach (Order order in orders)
                    {
                        criteria.AddOrder(order);
                    }
                }

                criteria.SetFirstResult(firstResult);
                criteria.SetMaxResults(maxresults);

                return CreateReturnArray(criteria);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ActiveRecordException("Could not perform SlicedFindAll for " + type.Name, ex);
            }
            finally
            {
                holder.ReleaseSession(session);
            }
        }

        /// <summary>
        /// Returns a portion of the query results (sliced)
        /// </summary>
        public static T[] SlicedFindAll(int firstResult, int maxresults, params ICriterion[] criterias)
        {
            return SlicedFindAll(firstResult, maxresults, null, criterias);
        }
        #endregion

        #endregion

        #region Create
        /// <summary>
        /// Creates (Saves) a new instance to the database.
        /// </summary>
        /// <param name="instance"></param>
        public static void Create(T instance)
        {
            Type type = typeof(T);

            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            EnsureInitialized();

            ISession session = holder.CreateSession(type);

            try
            {
                session.Save(instance);
                session.Flush();
            }
            catch (Exception ex)
            {
                // NHibernate catches our ValidationException, and as such it is the innerexception here
                if (ex.InnerException is ValidationException)
                {
                    throw ex.InnerException;
                }
                else
                {
                    throw new ActiveRecordException("Could not perform Create for " + instance.GetType().Name, ex);
                }
            }
            finally
            {
                holder.ReleaseSession(session);
            }
        }
        #endregion

        #region Update
        /// <summary>
        /// Persists the modification on the instance state to the database.
        /// </summary>
        /// <param name="instance"></param>
        public static void Update(T instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            Type type = typeof(T);

            EnsureInitialized();

            ISession session = holder.CreateSession(type);

            try
            {
                session.Update(instance);
                session.Flush();
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ActiveRecordException("Could not perform Update for " + type.Name, ex);
            }
            finally
            {
                holder.ReleaseSession(session);
            }
        }
        #endregion

        #region Save
        /// <summary>
        /// Saves the instance to the database
        /// </summary>
        /// <param name="instance"></param>
        public static void Save(T instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            Type type = typeof(T);

            EnsureInitialized();

            ISession session = holder.CreateSession(type);

            try
            {
                session.SaveOrUpdate(instance);
                session.Flush();
            }
            catch (Exception ex)
            {
                // NHibernate catches our ValidationException, and as such it is the innerexception here
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
        #endregion

        #region Delete
        /// <summary>
        /// Deletes the instance from the database.
        /// </summary>
        /// <param name="instance"></param>
        public static void Delete(T instance)
        {
            Type type = typeof(T);

            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            EnsureInitialized();

            ISession session = holder.CreateSession(type);

            try
            {
                session.Delete(instance);
                session.Flush();
            }
            catch (Exception ex)
            {
                // NHibernate catches our ValidationException, and as such it is the innerexception here
                if (ex.InnerException is ValidationException)
                {
                    throw ex.InnerException;
                }
                else
                {
                    throw new ActiveRecordException("Could not perform Delete for " + instance.GetType().Name, ex);
                }
            }
            finally
            {
                holder.ReleaseSession(session);
            }
        }
        #endregion

		public static void Refresh(T instance)
		{
			ActiveRecordBase.Refresh(instance);
		}

        #region DeleteAll
        public static void DeleteAll()
        {
            Type type = typeof(T);

            EnsureInitialized();

            ISession session = holder.CreateSession(type);

            try
            {
                session.Delete(String.Format("from {0}", type.Name));
                session.Flush();
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ActiveRecordException("Could not perform DeleteAll for " + type.Name, ex);
            }
            finally
            {
                holder.ReleaseSession(session);
            }
        }

        /// <summary>
        /// Deletes all records of the specified type, using conditions in HQL.
        /// </summary>
        /// <param name="conditions ">HQL conditions</param>
        public static void DeleteAll(string conditions)
        {
            Type type = typeof(T);

            EnsureInitialized();

            ISession session = holder.CreateSession(type);

            try
            {
                session.Delete(String.Format("from {0} where {1}", type.Name, conditions));
                session.Flush();
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ActiveRecordException("Could not perform DeleteAll for " + type.Name, ex);
            }
            finally
            {
                holder.ReleaseSession(session);
            }
        }

        /// <summary>
        /// Removes all records with matching identifiers (typically primarykeys)
        /// </summary>
        /// <param name="pkValues"></param>
        /// <returns>Number of records deleted</returns>
        public static int DeleteAll<PkType>(IEnumerable<PkType> pkValues)
        {
            if (pkValues == null)
            {
                return 0;
            }

            Type type = typeof(T);

            int counter = 0;
            foreach (PkType pk in pkValues)
            {
                T obj = FindByPrimaryKey(pk, false);
                if (obj != null)
                {
                    ActiveRecordBase<T> arBase = obj as ActiveRecordBase<T>;
                    if (arBase != null)
                    {
                        arBase.Delete(); // in order to allow override of the virtual "Delete()" method
                    }
                    else
                    {
                        ActiveRecordMediator<T>.Delete(obj);
                    }
                    counter++;
                }
            }

            return counter;
        }
        #endregion

        #region ExecuteQuery
        public static object ExecuteQuery(IActiveRecordQuery query)
        {
            Type targetType = query.Target;
            EnsureInitialized();

            ISession session = holder.CreateSession(targetType);

            try
            {
                return query.Execute(session);
            }
            catch (Exception ex)
            {
                throw new ActiveRecordException("Could not perform Execute for " + targetType.Name, ex);
            }
            finally
            {
                holder.ReleaseSession(session);
            }
        }

        public static R ExecuteQuery2<R>(IActiveRecordQuery<R> query)
        {
            Type targetType = query.Target;

            ActiveRecordBase.EnsureInitialized(targetType);

            ISessionFactoryHolder holder = ActiveRecordMediator.GetSessionFactoryHolder();
            ISession session = holder.CreateSession(targetType);

            try
            {
                return query.Execute(session);
            }
            catch (Exception ex)
            {
                throw new ActiveRecordException("Could not perform Execute for " + targetType.Name, ex);
            }
            finally
            {
                holder.ReleaseSession(session);
            }
        }
        #endregion

        #region Exists
        /// <summary>
        /// Check if the <paramref name="id"/> exists in the datastore.
        /// </summary>
        /// <typeparam name="PkType">The <c>System.Type</c> of the PrimaryKey</typeparam>
        /// <param name="id">The id to check on</param>
        /// <returns>True if the ID exists; otherwise false.</returns>
        public static bool Exists<PkType>(PkType id)
        {
            Type arType = typeof(T);
            ScalarQuery<int> query = new ScalarQuery<int>(arType, String.Format(
                "select count(*) from {0} ar where ar.id = ?", arType.Name), id);
            return ExecuteQuery2(query) > 0;
        }
         #endregion

        #endregion

        #region protected internal static
      
        #region Execute
        /// <summary>
        /// Invokes the specified delegate passing a valid 
        /// NHibernate session. Used for custom NHibernate queries.
        /// </summary>
        /// <param name="call">The delegate instance</param>
        /// <param name="instance">The ActiveRecord instance</param>
        /// <returns>Whatever is returned by the delegate invocation</returns>
        protected internal static object Execute( NHibernateDelegate call, T instance)
        {
            if (instance == default(T)) throw new ArgumentNullException("targetType", "Target type must be informed");
            if (call == null) throw new ArgumentNullException("call", "Delegate must be passed");

            Type type = typeof(T);

            EnsureInitialized();

            ISession session = holder.CreateSession(type);

            try
            {
                return call(session, instance);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ActiveRecordException("Error performing Execute for " + type.Name, ex);
            }
            finally
            {
                holder.ReleaseSession(session);
            }
        }

        #endregion               

        #region FindByPrimaryKey
        /// <summary>
        /// Finds an object instance by a unique ID
        /// </summary>
        /// <param name="id">ID value</param>
        /// <returns></returns>
        protected internal static T FindByPrimaryKey(object id)
        {
            return FindByPrimaryKey(id, true);
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
        protected internal static T FindByPrimaryKey(object id, bool throwOnNotFound)
        {
            Type type = typeof(T);
            EnsureInitialized();

            ISession session = holder.CreateSession(type);

            try
            {
                return session.Load(type, id) as T;
            }
            catch (ObjectNotFoundException ex)
            {
                if (throwOnNotFound)
                {
                    String message = String.Format("Could not find {0} with id {1}", type.Name, id);
                    throw new NotFoundException(message, ex);
                }

                return null;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ActiveRecordException("Could not perform Load (Find by id) for " + type.Name, ex);
            }
            finally
            {
                holder.ReleaseSession(session);
            }
        }
        #endregion        

        #endregion

        #region private static
        private static T[] CreateReturnArray(ICriteria criteria)
        {
            System.Collections.IList result = criteria.List();
            T[] array = (T[])Array.CreateInstance(typeof(T), result.Count);

            result.CopyTo(array, 0);

            return array;
        }

        #endregion

        #region protected internal
        /// <summary>
        /// Invokes the specified delegate passing a valid 
        /// NHibernate session. Used for custom NHibernate queries.
        /// </summary>
        /// <param name="call">The delegate instance</param>
        /// <returns>Whatever is returned by the delegate invocation</returns>
        protected internal object Execute(NHibernateDelegate call)
        {
            return Execute(call, this as T);
        }
        #endregion

        #region public virtual
        /// <summary>
        /// Saves the instance information to the database.
        /// May Create or Update the instance depending 
        /// on whether it has a valid ID.
        /// </summary>
        public virtual void Save()
        {
            ActiveRecordBase.Save(this);
        }

        /// <summary>
        /// Creates (Saves) a new instance to the database.
        /// </summary>
        public virtual void Create()
        {
            ActiveRecordBase.Create(this);
        }

        /// <summary>
        /// Persists the modification on the instance
        /// state to the database.
        /// </summary>
        public virtual void Update()
        {
            ActiveRecordBase.Update(this);
        }

		/// <summary>
        /// Refresh the instance from the database.
        /// </summary>
        public virtual void Refresh()
        {
            ActiveRecordBase.Refresh(this);
        }

        /// <summary>
        /// Deletes the instance from the database.
        /// </summary>
        public virtual void Delete()
        {
            ActiveRecordBase.Delete(this);
        }
        #endregion

        #region public override
        public override String ToString()
        {
            Framework.Internal.ActiveRecordModel model = Framework.Internal.ActiveRecordModel.GetModel(typeof(T));

            if (model == null || model.Ids.Count != 1)
            {
                return base.ToString();
            }

            Framework.Internal.PrimaryKeyModel pkModel = (Framework.Internal.PrimaryKeyModel)model.Ids[0];

            object pkVal = pkModel.Property.GetValue(this, null);

            return base.ToString() + "#" + pkVal;
        }
        #endregion
    }
}
#endif
