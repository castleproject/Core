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

namespace Castle.Facilities.NHibernateIntegration.Components.Dao
{
	using System;
	using System.Collections;
	using NHibernate;
	using NHibernate.Collection;
	using NHibernate.Criterion;
	using NHibernate.Proxy;
	using Util;

	/// <summary>
	/// Summary description for GenericDao.
	/// </summary>
	/// <remarks>
	/// Contributed by Steve Degosserie &lt;steve.degosserie@vn.netika.com&gt;
	/// </remarks>
	public class NHibernateGenericDao : INHibernateGenericDao
	{
		private readonly ISessionManager sessionManager;
		private string sessionFactoryAlias = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="NHibernateGenericDao"/> class.
		/// </summary>
		/// <param name="sessionManager">The session manager.</param>
		public NHibernateGenericDao(ISessionManager sessionManager)
		{
			this.sessionManager = sessionManager;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NHibernateGenericDao"/> class.
		/// </summary>
		/// <param name="sessionManager">The session manager.</param>
		/// <param name="sessionFactoryAlias">The session factory alias.</param>
		public NHibernateGenericDao(ISessionManager sessionManager, string sessionFactoryAlias) : this(sessionManager)
		{
			this.sessionFactoryAlias = sessionFactoryAlias;
		}

		/// <summary>
		/// Gets the session manager.
		/// </summary>
		/// <value>The session manager.</value>
		protected ISessionManager SessionManager
		{
			get { return this.sessionManager; }
		}

		/// <summary>
		/// Gets or sets the session factory alias.
		/// </summary>
		/// <value>The session factory alias.</value>
		public string SessionFactoryAlias
		{
			get { return this.sessionFactoryAlias; }
			set { this.sessionFactoryAlias = value; }
		}

		#region IGenericDAO Members

		/// <summary>
		/// Returns all instances found for the specified type.
		/// </summary>
		/// <param name="type">The target type.</param>
		/// <returns>The <see cref="Array"/> of results</returns>
		public virtual Array FindAll(Type type)
		{
			return this.FindAll(type, int.MinValue, int.MinValue);
		}

		/// <summary>
		/// Returns a portion of the query results (sliced)
		/// </summary>
		/// <param name="type">The target type.</param>
		/// <param name="firstRow">The number of the first row to retrieve.</param>
		/// <param name="maxRows">The maximum number of results retrieved.</param>
		/// <returns>The <see cref="Array"/> of results</returns>
		public virtual Array FindAll(Type type, int firstRow, int maxRows)
		{
			using(ISession session = this.GetSession())
			{
				try
				{
					ICriteria criteria = session.CreateCriteria(type);

					if (firstRow != int.MinValue) criteria.SetFirstResult(firstRow);
					if (maxRows != int.MinValue) criteria.SetMaxResults(maxRows);
					IList result = criteria.List();

					Array array = Array.CreateInstance(type, result.Count);
					result.CopyTo(array, 0);

					return array;
				}
				catch(Exception ex)
				{
					throw new DataException("Could not perform FindAll for " + type.Name, ex);
				}
			}
		}

		/// <summary>
		/// Finds an object instance by an unique ID
		/// </summary>
		/// <param name="type">The AR subclass type</param>
		/// <param name="id">ID value</param>
		/// <returns>The object instance.</returns>
		public virtual object FindById(Type type, object id)
		{
			using(ISession session = this.GetSession())
			{
				try
				{
					return session.Load(type, id);
				}
				catch(ObjectNotFoundException)
				{
					throw;
				}
				catch(Exception ex)
				{
					throw new DataException("Could not perform FindByPrimaryKey for " + type.Name, ex);
				}
			}
		}

		/// <summary>
		/// Creates (Saves) a new instance to the database.
		/// </summary>
		/// <param name="instance">The instance to be created on the database</param>
		/// <returns>The instance</returns>
		public virtual object Create(object instance)
		{
			using(ISession session = this.GetSession())
			{
				try
				{
					return session.Save(instance);
				}
				catch(Exception ex)
				{
					throw new DataException("Could not perform Create for " + instance.GetType().Name, ex);
				}
			}
		}

		/// <summary>
		/// Deletes the instance from the database.
		/// </summary>
		/// <param name="instance">The instance to be deleted from the database</param>
		public virtual void Delete(object instance)
		{
			using(ISession session = this.GetSession())
			{
				try
				{
					session.Delete(instance);
				}
				catch(Exception ex)
				{
					throw new DataException("Could not perform Delete for " + instance.GetType().Name, ex);
				}
			}
		}

		/// <summary>
		/// Persists the modification on the instance
		/// state to the database.
		/// </summary>
		/// <param name="instance">The instance to be updated on the database</param>
		public virtual void Update(object instance)
		{
			using(ISession session = this.GetSession())
			{
				try
				{
					session.Update(instance);
				}
				catch(Exception ex)
				{
					throw new DataException("Could not perform Update for " + instance.GetType().Name, ex);
				}
			}
		}

		/// <summary>
		/// Deletes all rows for the specified type
		/// </summary>
		/// <param name="type">type on which the rows on the database should be deleted</param>
		public virtual void DeleteAll(Type type)
		{
			using(ISession session = this.GetSession())
			{
				try
				{
					session.Delete(String.Format("from {0}", type.Name));
				}
				catch(Exception ex)
				{
					throw new DataException("Could not perform DeleteAll for " + type.Name, ex);
				}
			}
		}

		/// <summary>
		/// Saves the instance to the database. If the primary key is unitialized
		/// it creates the instance on the database. Otherwise it updates it.
		/// <para>
		/// If the primary key is assigned, then you must invoke <see cref="Create"/>
		/// or <see cref="Update"/> instead.
		/// </para>
		/// </summary>
		/// <param name="instance">The instance to be saved</param>
		public virtual void Save(object instance)
		{
			using(ISession session = this.GetSession())
			{
				try
				{
					session.SaveOrUpdate(instance);
				}
				catch(Exception ex)
				{
					throw new DataException("Could not perform Save for " + instance.GetType().Name, ex);
				}
			}
		}

		#endregion

		#region INHibernateGenericDAO Members

		/// <summary>
		/// Returns all instances found for the specified type
		/// using criteria.
		/// </summary>
		/// <param name="type">The target type.</param>
		/// <param name="criterias">The criteria expression</param>
		/// <returns>The <see cref="Array"/> of results.</returns>
		public virtual Array FindAll(Type type, ICriterion[] criterias)
		{
			return this.FindAll(type, criterias, null, int.MinValue, int.MinValue);
		}

		/// <summary>
		/// Returns all instances found for the specified type
		/// using criteria.
		/// </summary>
		/// <param name="type">The target type.</param>
		/// <param name="criterias">The criteria expression</param>
		/// <param name="firstRow">The number of the first row to retrieve.</param>
		/// <param name="maxRows">The maximum number of results retrieved.</param>
		/// <returns>The <see cref="Array"/> of results.</returns>
		public virtual Array FindAll(Type type, ICriterion[] criterias, int firstRow, int maxRows)
		{
			return this.FindAll(type, criterias, null, firstRow, maxRows);
		}

		/// <summary>
		/// Returns all instances found for the specified type
		/// using criteria.
		/// </summary>
		/// <param name="type">The target type.</param>
		/// <param name="criterias">The criteria expression</param>
		/// <param name="sortItems">An <see cref="Array"/> of <see cref="Order"/> objects.</param>
		/// <returns>The <see cref="Array"/> of results.</returns>
		public virtual Array FindAll(Type type, ICriterion[] criterias, Order[] sortItems)
		{
			return this.FindAll(type, criterias, sortItems, int.MinValue, int.MinValue);
		}

		/// <summary>
		/// Returns all instances found for the specified type
		/// using criteria.
		/// </summary>
		/// <param name="type">The target type.</param>
		/// <param name="criterias">The criteria expression</param>
		/// <param name="sortItems">An <see cref="Array"/> of <see cref="Order"/> objects.</param>
		/// <param name="firstRow">The number of the first row to retrieve.</param>
		/// <param name="maxRows">The maximum number of results retrieved.</param>
		/// <returns>The <see cref="Array"/> of results.</returns>
		public virtual Array FindAll(Type type, ICriterion[] criterias, Order[] sortItems, int firstRow, int maxRows)
		{
			using(ISession session = this.GetSession())
			{
				try
				{
					ICriteria criteria = session.CreateCriteria(type);

					if (criterias != null)
					{
						foreach(ICriterion cond in criterias)
						{
							criteria.Add(cond);
						}
					}

					if (sortItems != null)
					{
						foreach(Order order in sortItems)
						{
							criteria.AddOrder(order);
						}
					}

					if (firstRow != int.MinValue) criteria.SetFirstResult(firstRow);
					if (maxRows != int.MinValue) criteria.SetMaxResults(maxRows);
					IList result = criteria.List();

					Array array = Array.CreateInstance(type, result.Count);
					result.CopyTo(array, 0);

					return array;
				}
				catch(Exception ex)
				{
					throw new DataException("Could not perform FindAll for " + type.Name, ex);
				}
			}
		}

		/// <summary>
		/// Finds all with custom query.
		/// </summary>
		/// <param name="queryString">The query string.</param>
		/// <returns></returns>
		public virtual Array FindAllWithCustomQuery(string queryString)
		{
			return this.FindAllWithCustomQuery(queryString, int.MinValue, int.MinValue);
		}

		/// <summary>
		/// Finds all with custom HQL query.
		/// </summary>
		/// <param name="queryString">The query string.</param>
		/// <param name="firstRow">The number of the first row to retrieve.</param>
		/// <param name="maxRows">The maximum number of results retrieved.</param>
		/// <returns></returns>
		public virtual Array FindAllWithCustomQuery(string queryString, int firstRow, int maxRows)
		{
			if (string.IsNullOrEmpty(queryString)) throw new ArgumentNullException("queryString");

			using(ISession session = this.GetSession())
			{
				try
				{
					IQuery query = session.CreateQuery(queryString);

					if (firstRow != int.MinValue) query.SetFirstResult(firstRow);
					if (maxRows != int.MinValue) query.SetMaxResults(maxRows);
					IList result = query.List();
					if (result == null || result.Count == 0) return null;

					Array array = Array.CreateInstance(result[0].GetType(), result.Count);
					result.CopyTo(array, 0);

					return array;
				}
				catch(Exception ex)
				{
					throw new DataException("Could not perform Find for custom query : " + queryString, ex);
				}
			}
		}


		/// <summary>
		/// Finds all with named HQL query.
		/// </summary>
		/// <param name="namedQuery">The named query.</param>
		/// <returns></returns>
		public virtual Array FindAllWithNamedQuery(string namedQuery)
		{
			return this.FindAllWithNamedQuery(namedQuery, int.MinValue, int.MinValue);
		}

		/// <summary>
		/// Finds all with named HQL query.
		/// </summary>
		/// <param name="namedQuery">The named query.</param>
		/// <param name="firstRow">The number of the first row to retrieve.</param>
		/// <param name="maxRows">The maximum number of results retrieved.</param>
		/// <returns></returns>
		public virtual Array FindAllWithNamedQuery(string namedQuery, int firstRow, int maxRows)
		{
			if (string.IsNullOrEmpty(namedQuery)) throw new ArgumentNullException("namedQuery");

			using(ISession session = this.GetSession())
			{
				try
				{
					IQuery query = session.GetNamedQuery(namedQuery);
					if (query == null) throw new ArgumentException("Cannot find named query", "namedQuery");

					if (firstRow != int.MinValue) query.SetFirstResult(firstRow);
					if (maxRows != int.MinValue) query.SetMaxResults(maxRows);
					IList result = query.List();
					if (result == null || result.Count == 0) return null;

					Array array = Array.CreateInstance(result[0].GetType(), result.Count);
					result.CopyTo(array, 0);

					return array;
				}
				catch(Exception ex)
				{
					throw new DataException("Could not perform Find for named query : " + namedQuery, ex);
				}
			}
		}

		/// <summary>
		/// Initializes the lazy properties.
		/// </summary>
		/// <param name="instance">The instance.</param>
		public void InitializeLazyProperties(object instance)
		{
			if (instance == null) throw new ArgumentNullException("instance");

			using(ISession session = this.GetSession())
			{
				foreach(object val in ReflectionUtility.GetPropertiesDictionary(instance).Values)
				{
					if (val is INHibernateProxy || val is IPersistentCollection)
					{
						if (!NHibernateUtil.IsInitialized(val))
						{
							session.Lock(instance, LockMode.None);
							NHibernateUtil.Initialize(val);
						}
					}
				}
			}
		}

		/// <summary>
		/// Initializes the lazy property.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <param name="propertyName">Name of the property.</param>
		public void InitializeLazyProperty(object instance, string propertyName)
		{
			if (instance == null) throw new ArgumentNullException("instance");
			if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException("propertyName");

			var properties = ReflectionUtility.GetPropertiesDictionary(instance);
			if (! properties.ContainsKey(propertyName))
				throw new ArgumentOutOfRangeException("propertyName", "Property "
				                                                      + propertyName + " doest not exist for type "
				                                                      + instance.GetType().ToString() + ".");

			using(ISession session = this.GetSession())
			{
				object val = properties[propertyName];

				if (val is INHibernateProxy || val is IPersistentCollection)
				{
					if (!NHibernateUtil.IsInitialized(val))
					{
						session.Lock(instance, LockMode.None);
						NHibernateUtil.Initialize(val);
					}
				}
			}
		}

		#endregion

		#region Private methods

		private ISession GetSession()
		{
			if (string.IsNullOrEmpty(this.sessionFactoryAlias))
			{
				return this.sessionManager.OpenSession();
			}
			else
			{
				return this.sessionManager.OpenSession(this.sessionFactoryAlias);
			}
		}

		#endregion
	}
}