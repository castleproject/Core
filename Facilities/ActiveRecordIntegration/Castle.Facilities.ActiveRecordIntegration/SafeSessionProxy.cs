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

namespace Castle.Facilities.ActiveRecordIntegration
{
	using System;
	using System.Collections;
	using System.Data;

	using NHibernate;
	using NHibernate.Type;

	using Castle.ActiveRecord.Framework;

	/// <summary>
	/// This class implements <see cref="ISession"/>
	/// and delegates <see cref="ISession.Close"/> and 
	/// <see cref="ISession.Dispose"/> to <see cref="ISessionFactoryHolder.ReleaseSession"/>
	/// as the session is in fact managed by ActiveRecord framework
	/// </summary>
	public class SafeSessionProxy : ISession
	{
		private readonly ISession innerSession;
		private readonly ISessionFactoryHolder holder;
		
		private bool wasClosed;

		public SafeSessionProxy(ISessionFactoryHolder holder, ISession innerSession)
		{
			if (innerSession == null) throw new ArgumentNullException("innerSession");

			this.innerSession = innerSession;
			this.holder = holder;
		}

		public FlushMode FlushMode
		{
			get { return innerSession.FlushMode; }
			set { innerSession.FlushMode = value; }
		}

		public ISessionFactory SessionFactory
		{
			get { return innerSession.SessionFactory; }
		}

		public IDbConnection Connection
		{
			get { return innerSession.Connection; }
		}

		public bool IsOpen
		{
			get { return innerSession.IsOpen; }
		}

		public bool IsConnected
		{
			get { return innerSession.IsConnected; }
		}

		public ITransaction Transaction
		{
			get { return innerSession.Transaction; }
		}

		public void Flush()
		{
			innerSession.Flush();
		}

		public IDbConnection Disconnect()
		{
			return innerSession.Disconnect();
		}

		public void Reconnect()
		{
			innerSession.Reconnect();
		}

		public void Reconnect(IDbConnection connection)
		{
			innerSession.Reconnect(connection);
		}

		public IDbConnection Close()
		{
			if (!wasClosed)
			{
				wasClosed = true;
				holder.ReleaseSession( innerSession );
				return null;
			}
			else
			{
				throw new InvalidOperationException("Session was closed");
			}
		}

		public void CancelQuery()
		{
			innerSession.CancelQuery();
		}

		public bool IsDirty()
		{
			return innerSession.IsDirty();
		}

		public object GetIdentifier(object obj)
		{
			return innerSession.GetIdentifier(obj);
		}

		public bool Contains(object obj)
		{
			return innerSession.Contains(obj);
		}

		public void Evict(object obj)
		{
			innerSession.Evict(obj);
		}

		public object Load(Type theType, object id, LockMode lockMode)
		{
			return innerSession.Load(theType, id, lockMode);
		}

		public object Load(Type theType, object id)
		{
			return innerSession.Load(theType, id);
		}

		public void Load(object obj, object id)
		{
			innerSession.Load(obj, id);
		}

		public object Get(Type clazz, object id)
		{
			return innerSession.Get(clazz, id);
		}

		public object Get(Type clazz, object id, LockMode lockMode)
		{
			return innerSession.Get(clazz, id, lockMode);
		}

		public void Replicate(object obj, ReplicationMode replicationMode)
		{
			innerSession.Replicate(obj, replicationMode);
		}

		public object Save(object obj)
		{
			return innerSession.Save(obj);
		}

		public void Save(object obj, object id)
		{
			innerSession.Save(obj, id);
		}

		public void SaveOrUpdate(object obj)
		{
			innerSession.SaveOrUpdate(obj);
		}

		public void Update(object obj)
		{
			innerSession.Update(obj);
		}

		public void Update(object obj, object id)
		{
			innerSession.Update(obj, id);
		}

		public object SaveOrUpdateCopy(object obj)
		{
			return innerSession.SaveOrUpdateCopy(obj);
		}

		public object SaveOrUpdateCopy(object obj, object id)
		{
			return innerSession.SaveOrUpdateCopy(obj, id);
		}

		public void Delete(object obj)
		{
			innerSession.Delete(obj);
		}

		public IList Find(String query)
		{
			return innerSession.Find(query);
		}

		public IList Find(String query, object value, IType type)
		{
			return innerSession.Find(query, value, type);
		}

		public IList Find(String query, object[] values, IType[] types)
		{
			return innerSession.Find(query, values, types);
		}

		public IEnumerable Enumerable(String query)
		{
			return innerSession.Enumerable(query);
		}

		public IEnumerable Enumerable(String query, object value, IType type)
		{
			return innerSession.Enumerable(query, value, type);
		}

		public IEnumerable Enumerable(String query, object[] values, IType[] types)
		{
			return innerSession.Enumerable(query, values, types);
		}

		public ICollection Filter(object collection, String filter)
		{
			return innerSession.Filter(collection, filter);
		}

		public ICollection Filter(object collection, String filter, object value, IType type)
		{
			return innerSession.Filter(collection, filter, value, type);
		}

		public ICollection Filter(object collection, String filter, object[] values, IType[] types)
		{
			return innerSession.Filter(collection, filter, values, types);
		}

		public int Delete(String query)
		{
			return innerSession.Delete(query);
		}

		public int Delete(String query, object value, IType type)
		{
			return innerSession.Delete(query, value, type);
		}

		public int Delete(String query, object[] values, IType[] types)
		{
			return innerSession.Delete(query, values, types);
		}

		public void Lock(object obj, LockMode lockMode)
		{
			innerSession.Lock(obj, lockMode);
		}

		public void Refresh(object obj)
		{
			innerSession.Refresh(obj);
		}

		public void Refresh(object obj, LockMode lockMode)
		{
			innerSession.Refresh(obj,lockMode);
		}

		public LockMode GetCurrentLockMode(object obj)
		{
			return innerSession.GetCurrentLockMode(obj);
		}

		public ITransaction BeginTransaction()
		{
			return innerSession.BeginTransaction();
		}

		public ITransaction BeginTransaction(IsolationLevel isolationLevel)
		{
			return innerSession.BeginTransaction(isolationLevel);
		}

		public ICriteria CreateCriteria(Type persistentClass)
		{
			return innerSession.CreateCriteria(persistentClass);
		}

		public IQuery CreateQuery(String queryString)
		{
			return innerSession.CreateQuery(queryString);
		}

		public IQuery CreateFilter(object collection, String queryString)
		{
			return innerSession.CreateFilter(collection, queryString);
		}

		public IQuery GetNamedQuery(String queryName)
		{
			return innerSession.GetNamedQuery(queryName);
		}

		public IQuery CreateSQLQuery(String sql, String returnAlias, Type returnClass)
		{
			return innerSession.CreateSQLQuery(sql, returnAlias, returnClass);
		}

		public IQuery CreateSQLQuery(String sql, String[] returnAliases, Type[] returnClasses)
		{
			return innerSession.CreateSQLQuery(sql, returnAliases, returnClasses);
		}

		public void Clear()
		{
			innerSession.Clear();
		}

		public void Dispose()
		{
			if (!wasClosed)
			{
				holder.ReleaseSession( innerSession );
			}
			else
			{
				throw new InvalidOperationException("Session was closed");
			}
		}
	}
}
