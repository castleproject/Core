// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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
	using System.Threading;
	using System.Collections;
	using System.Runtime.CompilerServices;

	using NHibernate;
	using NHibernate.Cfg;

	using Castle.ActiveRecord.Framework.Scopes;

	/// <summary>
	/// Default implementation of <seealso cref="ISessionFactoryHolder"/>
	/// </summary>
	/// <remarks>
	/// This class is thread safe
	/// </remarks>
	public class SessionFactoryHolder : ISessionFactoryHolder
	{
		private Hashtable type2Conf = Hashtable.Synchronized(new Hashtable());
		private Hashtable type2SessFactory = Hashtable.Synchronized(new Hashtable());
		private ReaderWriterLock readerWriterLock = new ReaderWriterLock();
		private IThreadScopeInfo threadScopeInfo;
		
		public SessionFactoryHolder()
		{
		}

		public event RootTypeHandler OnRootTypeRegistered;

		public void Register(Type rootType, Configuration cfg)
		{
			type2Conf.Add(rootType, cfg);

			if (OnRootTypeRegistered != null)
			{
				OnRootTypeRegistered(this, rootType);
			}
		}

		public Configuration GetConfiguration(Type type)
		{
			return type2Conf[type] as Configuration;
		}

		/// <summary>
		/// Optimized with reader/writer lock.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public ISessionFactory GetSessionFactory(Type type)
		{
			Type normalizedtype = GetRootType(type);

			if (normalizedtype == null)
			{
				throw new ActiveRecordException("No configuration for ActiveRecord found in the type hierarchy -> " + type.FullName);
			}

			readerWriterLock.AcquireReaderLock(-1);

			ISessionFactory sessFactory = null;

			if (type2SessFactory.Contains(normalizedtype))
			{
				sessFactory = type2SessFactory[normalizedtype] as ISessionFactory;
			}

			if (sessFactory != null)
			{
				readerWriterLock.ReleaseReaderLock();

				return sessFactory;
			}

			readerWriterLock.UpgradeToWriterLock(-1);

			try
			{
				Configuration cfg = GetConfiguration(normalizedtype);

				sessFactory = cfg.BuildSessionFactory();

				type2SessFactory[normalizedtype] = sessFactory;

				return sessFactory;
			}
			finally
			{
				readerWriterLock.ReleaseWriterLock();
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public ISession CreateSession(Type type)
		{
			if (threadScopeInfo.HasInitializedScope)
			{
				return CreateScopeSession(type);
			}

			ISessionFactory sessionFactory = GetSessionFactory(type);

			ISession session = OpenSession(sessionFactory);

			System.Diagnostics.Debug.Assert( session != null );

			return session;
		}

		public Type GetRootType(Type type)
		{
			while(type != typeof(object))
			{
				if (type2Conf.ContainsKey(type)) return type;

				type = type.BaseType;
			}

			return typeof(ActiveRecordBase);
		}

		private static ISession OpenSession(ISessionFactory sessionFactory)
		{
			lock(sessionFactory)
			{
				return sessionFactory.OpenSession( HookDispatcher.Instance );
			}
		}

		public void ReleaseSession(ISession session)
		{
			if (threadScopeInfo.HasInitializedScope)
			{
				ReleaseScopedSession(session);
			}
			else
			{
				session.Close();
			}
		}

		public IThreadScopeInfo ThreadScopeInfo
		{
			get { return threadScopeInfo; }
			set
			{
				ThreadScopeAccessor.Instance.ScopeInfo = value;
				threadScopeInfo = value;
			}
		}

		private ISession CreateScopeSession(Type type)
		{
			ISessionScope scope = threadScopeInfo.GetRegisteredScope();
			ISessionFactory sessionFactory = GetSessionFactory(type);

#if DEBUG
			System.Diagnostics.Debug.Assert( scope != null );
			System.Diagnostics.Debug.Assert( sessionFactory != null );
#endif

			if (scope.IsKeyKnown(sessionFactory))
			{
				return scope.GetSession(sessionFactory);
			}
			else
			{
				ISession session = OpenSession(sessionFactory);
#if DEBUG
				System.Diagnostics.Debug.Assert( session != null );
#endif
				scope.RegisterSession(sessionFactory, session);

				return session;
			}
		}

		private void ReleaseScopedSession(ISession session)
		{
			
		}
	}
}