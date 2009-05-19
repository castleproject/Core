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

namespace Castle.ActiveRecord.Framework
{
	using System;
	using NHibernate;

	/// <summary>
	/// Wraps a NHibernate.IStatelessSession and provides an interface of type
	/// <see cref="NHibernate.ISession"/> for it.
	/// </summary>
	public class StatelessSessionWrapper : ISession
	{
		/// <summary>
		/// The stateless session to delegate to.
		/// </summary>
		protected IStatelessSession statelessSession;

		/// <summary>
		/// Builds a StatelessSessionWrapper.
		/// </summary>
		/// <param name="statelessSession">The stateless session to delegate to.</param>
		public StatelessSessionWrapper(IStatelessSession statelessSession)
		{
			this.statelessSession = statelessSession;
		}

#pragma warning disable 1591

		#region Supported

		public System.Data.IDbConnection Connection
		{
			get { return statelessSession.Connection; }
		}

		public bool IsConnected
		{
			get { return statelessSession.Connection != null; }
		}

		public ITransaction Transaction
		{
			get { return statelessSession.Transaction; }
		}

		public ITransaction BeginTransaction()
		{
			return statelessSession.BeginTransaction();
		}

		public System.Data.IDbConnection Close()
		{
			statelessSession.Close();
			return null;
		}
		
		public ICriteria CreateCriteria(string entityName, string alias)
		{
			return statelessSession.CreateCriteria(entityName, alias);
		}

		public ICriteria CreateCriteria(string entityName)
		{
			return statelessSession.CreateCriteria(entityName);
		}

		public ICriteria CreateCriteria(Type persistentClass, string alias)
		{
			return statelessSession.CreateCriteria(persistentClass, alias);
		}

		public ICriteria CreateCriteria(Type persistentClass)
		{
			return statelessSession.CreateCriteria(persistentClass);
		}

		public ICriteria CreateCriteria<T>(string alias) where T : class
		{
			return statelessSession.CreateCriteria<T>(alias);
		}

		public ICriteria CreateCriteria<T>() where T : class
		{
			return statelessSession.CreateCriteria<T>();
		}

		public IQuery CreateQuery(string queryString)
		{
			return statelessSession.CreateQuery(queryString);
		}

		public ISQLQuery CreateSQLQuery(string queryString)
		{
			return statelessSession.CreateSQLQuery(queryString);
		}

		public void Delete(string entityName, object obj)
		{
			statelessSession.Delete(entityName, obj);
		}

		public void Delete(object obj)
		{
			statelessSession.Delete(obj);
		}

		public object Get(Type clazz, object id)
		{
			return statelessSession.Get(clazz.FullName, id);
		}

		public T Get<T>(object id, LockMode lockMode)
		{
			return (T)statelessSession.Get(typeof(T).FullName, id, lockMode);
		}

		public T Get<T>(object id)
		{
			return (T)statelessSession.Get(typeof(T).FullName,id);
		}

		public object Get(string entityName, object id)
		{
			return statelessSession.Get(entityName, id);
		}

		public object Get(Type clazz, object id, LockMode lockMode)
		{
			return statelessSession.Get(clazz.FullName, id, lockMode);
		}

		public IQuery GetNamedQuery(string queryName)
		{
			return statelessSession.GetNamedQuery(queryName);
		}

		public NHibernate.Engine.ISessionImplementor GetSessionImplementation()
		{
			return ((NHibernate.Engine.ISessionImplementor)statelessSession);
		}

		public object Load(string entityName, object id)
		{
			return statelessSession.Get(entityName, id);
		}

		public T Load<T>(object id)
		{
			return (T)statelessSession.Get(typeof(T).FullName, id);
		}

		public T Load<T>(object id, LockMode lockMode)
		{
			return (T)statelessSession.Get(typeof(T).FullName, id, lockMode);
		}

		public object Load(Type theType, object id)
		{
			return statelessSession.Get(theType.FullName, id);
		}

		public object Load(string entityName, object id, LockMode lockMode)
		{
			return statelessSession.Get(entityName, id, lockMode);
		}

		public object Load(Type theType, object id, LockMode lockMode)
		{
			return statelessSession.Get(theType.FullName, id);
		}

		public object Save(string entityName, object obj)
		{
			return statelessSession.Insert(entityName, obj);
		}

		public object Save(object obj)
		{
			return statelessSession.Insert(obj);
		}

		public void Update(string entityName, object obj)
		{
			statelessSession.Update(entityName, obj);
		}

		public void Update(object obj)
		{
			statelessSession.Update(obj);
		}

		#endregion

		#region Unsupported
		public EntityMode ActiveEntityMode
		{
			get { throw new NotWrappedException(); }
		}

		public ITransaction BeginTransaction(System.Data.IsolationLevel isolationLevel)
		{
			throw new NotWrappedException();
		}

		public CacheMode CacheMode
		{
			get
			{
				throw new NotWrappedException();
			}
			set
			{
				throw new NotWrappedException();
			}
		}

		public void CancelQuery()
		{
			throw new NotWrappedException();
		}

		public void Clear()
		{
			throw new NotWrappedException();
		}

		public bool Contains(object obj)
		{
			throw new NotWrappedException();
		}

		public IQuery CreateFilter(object collection, string queryString)
		{
			throw new NotWrappedException();
		}

		public IMultiCriteria CreateMultiCriteria()
		{
			throw new NotWrappedException();
		}

		public IMultiQuery CreateMultiQuery()
		{
			throw new NotWrappedException();
		}

		public IQuery CreateSQLQuery(string sql, string[] returnAliases, Type[] returnClasses)
		{
			throw new NotWrappedException();
		}

		public IQuery CreateSQLQuery(string sql, string returnAlias, Type returnClass)
		{
			throw new NotWrappedException();
		}

		public int Delete(string query, object[] values, NHibernate.Type.IType[] types)
		{
			throw new NotWrappedException();
		}

		public int Delete(string query, object value, NHibernate.Type.IType type)
		{
			throw new NotWrappedException();
		}

		public int Delete(string query)
		{
			throw new NotWrappedException();
		}

		public void DisableFilter(string filterName)
		{
			throw new NotWrappedException();
		}

		public System.Data.IDbConnection Disconnect()
		{
			throw new NotWrappedException();
		}

		public IFilter EnableFilter(string filterName)
		{
			throw new NotWrappedException();
		}

		public System.Collections.IEnumerable Enumerable(string query, object[] values, NHibernate.Type.IType[] types)
		{
			throw new NotWrappedException();
		}

		public System.Collections.IEnumerable Enumerable(string query, object value, NHibernate.Type.IType type)
		{
			throw new NotWrappedException();
		}

		public System.Collections.IEnumerable Enumerable(string query)
		{
			throw new NotWrappedException();
		}

		public void Evict(object obj)
		{
			throw new NotWrappedException();
		}

		public System.Collections.ICollection Filter(object collection, string filter, object[] values, NHibernate.Type.IType[] types)
		{
			throw new NotWrappedException();
		}

		public System.Collections.ICollection Filter(object collection, string filter, object value, NHibernate.Type.IType type)
		{
			throw new NotWrappedException();
		}

		public System.Collections.ICollection Filter(object collection, string filter)
		{
			throw new NotWrappedException();
		}

		public System.Collections.IList Find(string query, object[] values, NHibernate.Type.IType[] types)
		{
			throw new NotWrappedException();
		}

		public System.Collections.IList Find(string query, object value, NHibernate.Type.IType type)
		{
			throw new NotWrappedException();
		}

		public System.Collections.IList Find(string query)
		{
			throw new NotWrappedException();
		}

		public void Flush()
		{
			throw new NotWrappedException();
		}

		public FlushMode FlushMode
		{
			get
			{
				throw new NotWrappedException();
			}
			set
			{
				throw new NotWrappedException();
			}
		}

		public LockMode GetCurrentLockMode(object obj)
		{
			throw new NotWrappedException();
		}

		public IFilter GetEnabledFilter(string filterName)
		{
			throw new NotWrappedException();
		}

		public string GetEntityName(object obj)
		{
			throw new NotWrappedException();
		}

		public object GetIdentifier(object obj)
		{
			throw new NotWrappedException();
		}

		public ISession GetSession(EntityMode entityMode)
		{
			throw new NotWrappedException();
		}

		public bool IsDirty()
		{
			throw new NotWrappedException();
		}

		public bool IsOpen
		{
			get { throw new NotWrappedException(); }
		}

		public void Load(object obj, object id)
		{
			throw new NotWrappedException();
		}

		public void Lock(string entityName, object obj, LockMode lockMode)
		{
			throw new NotWrappedException();
		}

		public void Lock(object obj, LockMode lockMode)
		{
			throw new NotWrappedException();
		}

		public object Merge(string entityName, object obj)
		{
			throw new NotWrappedException();
		}

		public object Merge(object obj)
		{
			throw new NotWrappedException();
		}

		public void Persist(string entityName, object obj)
		{
			throw new NotWrappedException();
		}

		public void Persist(object obj)
		{
			throw new NotWrappedException();
		}

		public void Reconnect(System.Data.IDbConnection connection)
		{
			throw new NotWrappedException();
		}

		public void Reconnect()
		{
			throw new NotWrappedException();
		}

		public void Refresh(object obj, LockMode lockMode)
		{
			throw new NotWrappedException();
		}

		public void Refresh(object obj)
		{
			throw new NotWrappedException();
		}

		public void Replicate(string entityName, object obj, ReplicationMode replicationMode)
		{
			throw new NotWrappedException();
		}

		public void Replicate(object obj, ReplicationMode replicationMode)
		{
			throw new NotWrappedException();
		}

		public void Save(object obj, object id)
		{
			throw new NotWrappedException();
		}

		public void SaveOrUpdate(string entityName, object obj)
		{
			throw new NotWrappedException();
		}

		public void SaveOrUpdate(object obj)
		{
			throw new NotWrappedException();
		}

		public object SaveOrUpdateCopy(object obj, object id)
		{
			throw new NotWrappedException();
		}

		public object SaveOrUpdateCopy(object obj)
		{
			throw new NotWrappedException();
		}

		public ISessionFactory SessionFactory
		{
			get { throw new NotWrappedException(); }
		}

		public ISession SetBatchSize(int batchSize)
		{
			throw new NotWrappedException();
		}

		public NHibernate.Stat.ISessionStatistics Statistics
		{
			get { throw new NotWrappedException(); }
		}

		public void Update(object obj, object id)
		{
			throw new NotWrappedException();
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			statelessSession.Dispose();
		}

		#endregion
#pragma warning restore 1591
	}

	/// <summary>
	/// Wraps a NotImplementedException with a preconfigured Catle-like
	/// excweption message.
	/// </summary>
	public class NotWrappedException : NotImplementedException
	{
		/// <summary>
		/// Calls the base class' constructor with the preconfigured message.
		/// </summary>
		public NotWrappedException()
			: base(@"The called method is not supported.
ActiveRecord is currently running within a StatelessSessionScope. Stateless sessions are faster than normal sessions, but they do not support all methods and properties that a normal session allows. 
Please check the stacktrace and change your code accordingly.") { }
	}
}
