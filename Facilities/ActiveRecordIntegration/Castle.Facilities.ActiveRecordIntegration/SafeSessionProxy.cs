// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.Facilities.ActiveRecordIntegration
{
	using System;
	using System.Collections;
	using System.Data;

	using NHibernate;
	using NHibernate.Stat;
	using NHibernate.Type;

	using Castle.ActiveRecord.Framework;

	/// <summary>
	/// This class implements <see cref="ISession"/>
	/// and delegates <see cref="Close"/> and 
	/// <see cref="Dispose"/> to <see cref="ISessionFactoryHolder.ReleaseSession"/>
	/// as the session is in fact managed by ActiveRecord framework
	/// </summary>
	public class SafeSessionProxy : ISession, IDisposable
	{
		private readonly ISession inner;
		private readonly ISessionFactoryHolder holder;
		
		private bool wasClosed;

		public SafeSessionProxy(ISessionFactoryHolder holder, ISession innerSession)
		{
			if (innerSession == null) throw new ArgumentNullException("innerSession");

			this.inner = innerSession;
			this.holder = holder;
		}

		public FlushMode FlushMode
		{
			get { return inner.FlushMode; }
			set { inner.FlushMode = value; }
		}

		public CacheMode CacheMode
		{
			get { return inner.CacheMode; }
			set { inner.CacheMode = value; }
		}

		public ISessionFactory SessionFactory
		{
			get { return inner.SessionFactory; }
		}

		public IDbConnection Connection
		{
			get { return inner.Connection; }
		}

		public bool IsOpen
		{
			get { return inner.IsOpen; }
		}

		public bool IsConnected
		{
			get { return inner.IsConnected; }
		}

		public ITransaction Transaction
		{
			get { return inner.Transaction; }
		}

		public ISessionStatistics Statistics
		{
			get { return inner.Statistics; }
		}

		public void Flush()
		{
			inner.Flush();
		}

		public IDbConnection Disconnect()
		{
			return inner.Disconnect();
		}

		public void Reconnect()
		{
			inner.Reconnect();
		}

		public void Reconnect(IDbConnection connection)
		{
			inner.Reconnect(connection);
		}

		public IDbConnection Close()
		{
			if (!wasClosed)
			{
				wasClosed = true;
				holder.ReleaseSession( inner );
				return null;
			}
			else
			{
				throw new InvalidOperationException("Session was closed");
			}
		}

		public void CancelQuery()
		{
			inner.CancelQuery();
		}

		public bool IsDirty()
		{
			return inner.IsDirty();
		}

		public object GetIdentifier(object obj)
		{
			return inner.GetIdentifier(obj);
		}

		public bool Contains(object obj)
		{
			return inner.Contains(obj);
		}

		public void Evict(object obj)
		{
			inner.Evict(obj);
		}

		public object Load(Type theType, object id, LockMode lockMode)
		{
			return inner.Load(theType, id, lockMode);
		}

		public object Load(Type theType, object id)
		{
			return inner.Load(theType, id);
		}

		public T Load<T>(object id, LockMode lockMode)
		{
			return inner.Load<T>(id, lockMode);
		}

		public T Load<T>(object id)
		{
			return inner.Load<T>(id);
		}

		public void Load(object obj, object id)
		{
			inner.Load(obj, id);
		}

		public object Get(Type clazz, object id)
		{
			return inner.Get(clazz, id);
		}

		public object Get(Type clazz, object id, LockMode lockMode)
		{
			return inner.Get(clazz, id, lockMode);
		}

		public T Get<T>(object id)
		{
			return inner.Get<T>(id);
		}

		public T Get<T>(object id, LockMode lockMode)
		{
			return inner.Get<T>(id, lockMode);
		}

		public string GetEntityName(object obj)
		{
			return inner.GetEntityName(obj);
		}

		public IFilter EnableFilter(string filterName)
		{
			return inner.EnableFilter(filterName);
		}

		public IFilter GetEnabledFilter(string filterName)
		{
			return inner.GetEnabledFilter(filterName);
		}

		public void DisableFilter(string filterName)
		{
			inner.DisableFilter(filterName);
		}

		public IMultiQuery CreateMultiQuery()
		{
			return inner.CreateMultiQuery();
		}

		public ISession SetBatchSize(int batchSize)
		{
			return inner.SetBatchSize(batchSize);
		}

		public void Replicate(object obj, ReplicationMode replicationMode)
		{
			inner.Replicate(obj, replicationMode);
		}

		public object Save(object obj)
		{
			return inner.Save(obj);
		}

		public void Save(object obj, object id)
		{
			inner.Save(obj, id);
		}

		public void SaveOrUpdate(object obj)
		{
			inner.SaveOrUpdate(obj);
		}

		public void Update(object obj)
		{
			inner.Update(obj);
		}

		public void Update(object obj, object id)
		{
			inner.Update(obj, id);
		}

		public object SaveOrUpdateCopy(object obj)
		{
			return inner.SaveOrUpdateCopy(obj);
		}

		public object SaveOrUpdateCopy(object obj, object id)
		{
			return inner.SaveOrUpdateCopy(obj, id);
		}

		public void Delete(object obj)
		{
			inner.Delete(obj);
		}

		public IList Find(String query)
		{
			return inner.CreateQuery(query).List();
		}

		public IList Find(String query, object value, IType type)
		{
			// TODO: This is deprecated. Use ISession.CreateQuery().SetXYZ().List()
			return inner.Find(query, value, type);
		}

		public IList Find(String query, object[] values, IType[] types)
		{
			// TODO: This is deprecated. Use ISession.CreateQuery().SetXYZ().List()
			return inner.Find(query, values, types);
		}

		public IEnumerable Enumerable(String query)
		{
			// TODO: This is deprecated. Use ISession.CreateQuery().SetXYZ().List()
			return inner.Enumerable(query);
		}

		public IEnumerable Enumerable(String query, object value, IType type)
		{
			// TODO: This is deprecated. Use ISession.CreateQuery().SetXYZ().List()
			return inner.Enumerable(query, value, type);
		}

		public IEnumerable Enumerable(String query, object[] values, IType[] types)
		{
			// TODO: This is deprecated. Use ISession.CreateQuery().SetXYZ().List()
			return inner.Enumerable(query, values, types);
		}

		public ICollection Filter(object collection, String filter)
		{
			// TODO: This is deprecated. Use ISession.CreateQuery().SetXYZ().List()
			return inner.Filter(collection, filter);
		}

		public ICollection Filter(object collection, String filter, object value, IType type)
		{
			// TODO: This is deprecated. Use ISession.CreateQuery().SetXYZ().List()
			return inner.Filter(collection, filter, value, type);
		}

		public ICollection Filter(object collection, String filter, object[] values, IType[] types)
		{
			// TODO: This is deprecated. Use ISession.CreateQuery().SetXYZ().List()
			return inner.Filter(collection, filter, values, types);
		}

		public int Delete(String query)
		{
			return inner.Delete(query);
		}

		public int Delete(String query, object value, IType type)
		{
			return inner.Delete(query, value, type);
		}

		public int Delete(String query, object[] values, IType[] types)
		{
			return inner.Delete(query, values, types);
		}

		public void Lock(object obj, LockMode lockMode)
		{
			inner.Lock(obj, lockMode);
		}

		public void Refresh(object obj)
		{
			inner.Refresh(obj);
		}

		public void Refresh(object obj, LockMode lockMode)
		{
			inner.Refresh(obj,lockMode);
		}

		public LockMode GetCurrentLockMode(object obj)
		{
			return inner.GetCurrentLockMode(obj);
		}

		public ITransaction BeginTransaction()
		{
			return inner.BeginTransaction();
		}

		public ITransaction BeginTransaction(IsolationLevel isolationLevel)
		{
			return inner.BeginTransaction(isolationLevel);
		}

		public ICriteria CreateCriteria(Type persistentClass)
		{
			return inner.CreateCriteria(persistentClass);
		}

		public ICriteria CreateCriteria(Type persistentClass, string alias)
		{
			return inner.CreateCriteria(persistentClass, alias);
		}

		public IQuery CreateQuery(String queryString)
		{
			return inner.CreateQuery(queryString);
		}

		public IQuery CreateFilter(object collection, String queryString)
		{
			return inner.CreateFilter(collection, queryString);
		}

		public IQuery GetNamedQuery(String queryName)
		{
			return inner.GetNamedQuery(queryName);
		}

		public ISQLQuery CreateSQLQuery(string queryString)
		{
			return inner.CreateSQLQuery(queryString);
		}

		public IQuery CreateSQLQuery(String sql, String returnAlias, Type returnClass)
		{
			return inner.CreateSQLQuery(sql, returnAlias, returnClass);
		}

		public IQuery CreateSQLQuery(String sql, String[] returnAliases, Type[] returnClasses)
		{
			return inner.CreateSQLQuery(sql, returnAliases, returnClasses);
		}

		public void Clear()
		{
			inner.Clear();
		}

		public void Dispose()
		{
			if (!wasClosed)
			{
				holder.ReleaseSession( inner );
			}
			else
			{
				throw new InvalidOperationException("Session was closed");
			}
		}
		
		public NHibernate.Engine.ISessionImplementor GetSessionImplementation()
		{
			return inner.GetSessionImplementation();
		}

		public IMultiCriteria CreateMultiCriteria()
		{
			return inner.CreateMultiCriteria();
		}
	}
}
