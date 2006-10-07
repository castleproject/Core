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

namespace Castle.ActiveRecord.Framework
{
	using System;
	using System.Threading;
	using System.Collections;
	using System.Runtime.CompilerServices;
	
	using Iesi.Collections;
	
	using NHibernate;
	using NHibernate.Cfg;

	using Castle.ActiveRecord.Framework.Scopes;

	/// <summary>
	/// Default implementation of <seealso cref="ISessionFactoryHolder"/>
	/// </summary>
	/// <remarks>
	/// This class is thread safe
	/// </remarks>
	public class SessionFactoryHolder : MarshalByRefObject, ISessionFactoryHolder
	{
		private Hashtable type2Conf = Hashtable.Synchronized(new Hashtable());
		private Hashtable type2SessFactory = Hashtable.Synchronized(new Hashtable());
		private ReaderWriterLock readerWriterLock = new ReaderWriterLock();
		private IThreadScopeInfo threadScopeInfo;
		
		/// <summary>
		/// Raised when a root type is registered.
		/// </summary>
		public event RootTypeHandler OnRootTypeRegistered;

		/// <summary>
		/// Associates a Configuration object to a root type
		/// </summary>
		/// <param name="rootType"></param>
		/// <param name="cfg"></param>
		public void Register(Type rootType, Configuration cfg)
		{
			type2Conf.Add(rootType, cfg);

			if (OnRootTypeRegistered != null)
			{
				OnRootTypeRegistered(this, rootType);
			}
		}

		/// <summary>
		/// Requests the Configuration associated to the type.
		/// </summary>
		public Configuration GetConfiguration(Type type)
		{
			return type2Conf[type] as Configuration;
		}

		/// <summary>
		/// Pendent
		/// </summary>
		public Configuration[] GetAllConfigurations()
		{
			HashedSet set = new HashedSet(type2Conf.Values);

			Configuration[] confs = new Configuration[set.Count];

			set.CopyTo(confs, 0);

			return confs;
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

			try
			{
				
				ISessionFactory sessFactory = null;

				if (type2SessFactory.Contains(normalizedtype))
				{
					sessFactory = type2SessFactory[normalizedtype] as ISessionFactory;
				}

				if (sessFactory != null)
				{
					return sessFactory;
				}

				LockCookie lc = readerWriterLock.UpgradeToWriterLock(-1);

				try
				{
					Configuration cfg = GetConfiguration(normalizedtype);

					sessFactory = cfg.BuildSessionFactory();

					type2SessFactory[normalizedtype] = sessFactory;

					return sessFactory;
				}
				finally
				{
					readerWriterLock.DowngradeFromWriterLock(ref lc);
				}
			}
			finally
			{
				readerWriterLock.ReleaseReaderLock();
			}
		}

		/// <summary>
		/// Creates a session for the associated type
		/// </summary>
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

		/// <summary>
		/// Gets the type of the root.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public Type GetRootType(Type type)
		{
			while(type != typeof(object))
			{
				if (type2Conf.ContainsKey(type)) return type;

				type = type.BaseType;

#if DOTNET2
				//to enable multiple database support for generic types
				if (type.IsGenericType)
				{
					Type genericTypeDef = type.GetGenericTypeDefinition();

					if (type2Conf.ContainsKey(genericTypeDef))
					{
						return genericTypeDef;
					}
				}
#endif
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

		private static ISession OpenSessionWithScope(ISessionScope scope, ISessionFactory sessionFactory)
		{
			lock(sessionFactory)
			{
				return scope.OpenSession( sessionFactory, HookDispatcher.Instance );
			}
		}

		/// <summary>
		/// Releases the specified session
		/// </summary>
		/// <param name="session"></param>
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

		/// <summary>
		/// Gets or sets the implementation of <see cref="IThreadScopeInfo"/>
		/// </summary>
		/// <value></value>
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
				ISession session = null;

				if (scope.WantsToCreateTheSession)
				{
					session = OpenSessionWithScope(scope, sessionFactory);
				}
				else
				{
					session = OpenSession(sessionFactory);
				}
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