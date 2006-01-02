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
	using NHibernate.Connection;
	using NHibernate.Dialect;
	using NHibernate.Metadata;

	using Castle.ActiveRecord.Framework;

	/// <summary>
	/// Implements <see cref="ISessionFactory"/> allowing 
	/// it to be used by the container as an ordinary component.
	/// However only <see cref="ISessionFactory.OpenSession"/>
	/// is implemented
	/// </summary>
	public sealed class SessionFactoryDelegate : ISessionFactory
	{
		private readonly Type arRootType;
		private readonly ISessionFactoryHolder holder;

		public SessionFactoryDelegate(ISessionFactoryHolder holder, Type arRootType)
		{
			this.arRootType = arRootType;
			this.holder = holder;
		}

		public ISession OpenSession(IDbConnection conn)
		{
			throw new NotImplementedException("SessionFactoryDelegate: not implemented");
		}

		public ISession OpenSession(IInterceptor interceptor)
		{
			throw new NotImplementedException("SessionFactoryDelegate: not implemented");
		}

		public ISession OpenSession(IDbConnection conn, IInterceptor interceptor)
		{
			throw new NotImplementedException("SessionFactoryDelegate: not implemented");
		}

		public ISession OpenSession()
		{
			ISession realSession = holder.CreateSession( arRootType );

			return new SafeSessionProxy(holder, realSession);
		}

		public IDatabinder OpenDatabinder()
		{
			throw new NotImplementedException("SessionFactoryDelegate: not implemented");
		}

		public IClassMetadata GetClassMetadata(Type persistentType)
		{
			throw new NotImplementedException("SessionFactoryDelegate: not implemented");
		}

		public ICollectionMetadata GetCollectionMetadata(string roleName)
		{
			throw new NotImplementedException("SessionFactoryDelegate: not implemented");
		}

		public IDictionary GetAllClassMetadata()
		{
			throw new NotImplementedException("SessionFactoryDelegate: not implemented");
		}

		public IDictionary GetAllCollectionMetadata()
		{
			throw new NotImplementedException("SessionFactoryDelegate: not implemented");
		}

		public void Close()
		{
			throw new NotImplementedException("SessionFactoryDelegate: not implemented");
		}

		public void Evict(Type persistentClass)
		{
			throw new NotImplementedException("SessionFactoryDelegate: not implemented");
		}

		public void Evict(Type persistentClass, object id)
		{
			throw new NotImplementedException("SessionFactoryDelegate: not implemented");
		}

		public void EvictCollection(string roleName)
		{
			throw new NotImplementedException("SessionFactoryDelegate: not implemented");
		}

		public void EvictCollection(string roleName, object id)
		{
			throw new NotImplementedException("SessionFactoryDelegate: not implemented");
		}

		public void EvictQueries()
		{
			throw new NotImplementedException("SessionFactoryDelegate: not implemented");
		}

		public void EvictQueries(string cacheRegion)
		{
			throw new NotImplementedException("SessionFactoryDelegate: not implemented");
		}

		public IConnectionProvider ConnectionProvider
		{
			get { throw new NotImplementedException("SessionFactoryDelegate: not implemented"); }
		}

		public Dialect Dialect
		{
			get { throw new NotImplementedException("SessionFactoryDelegate: not implemented"); }
		}
	}
}
