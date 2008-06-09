// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
	using System.Collections.Generic;
	using System.Data;

	using NHibernate;
	using NHibernate.Cfg;
	using NHibernate.Connection;
	using NHibernate.Dialect;
	using NHibernate.Engine;
	using NHibernate.Metadata;

	using Castle.ActiveRecord.Framework;
	using NHibernate.Stat;

	/// <summary>
	/// Implements <see cref="ISessionFactory"/> allowing 
	/// it to be used by the container as an ordinary component.
	/// However only <see cref="ISessionFactory.OpenSession(IDbConnection)"/>
	/// is implemented
	/// </summary>
	public sealed class SessionFactoryDelegate : ISessionFactory
	{
		private readonly Type arRootType;
		private readonly ISessionFactoryHolder holder;

		/// <summary>
		/// Initializes a new instance of the <see cref="SessionFactoryDelegate"/> class.
		/// </summary>
		/// <param name="holder">The holder.</param>
		/// <param name="arRootType">Type of the ar root.</param>
		public SessionFactoryDelegate(ISessionFactoryHolder holder, Type arRootType)
		{
			this.arRootType = arRootType;
			this.holder = holder;
		}

		/// <summary>
		/// Open a <c>ISession</c> on the given connection
		/// </summary>
		/// <param name="conn">A connection provided by the application</param>
		/// <returns>A session</returns>
		/// <remarks>
		/// Note that the second-level cache will be disabled if you
		/// supply a ADO.NET connection. NHibernate will not be able to track
		/// any statements you might have executed in the same transaction.
		/// Consider implementing your own <see cref="T:NHibernate.Connection.IConnectionProvider"/>.
		/// </remarks>
		public ISession OpenSession(IDbConnection conn)
		{
			throw new NotImplementedException("SessionFactoryDelegate: not implemented");
		}

		/// <summary>
		/// Create database connection and open a <c>ISession</c> on it, specifying an interceptor
		/// </summary>
		/// <param name="interceptor">A session-scoped interceptor</param>
		/// <returns>A session</returns>
		public ISession OpenSession(IInterceptor interceptor)
		{
			throw new NotImplementedException("SessionFactoryDelegate: not implemented");
		}

		/// <summary>
		/// Open a <c>ISession</c> on the given connection, specifying an interceptor
		/// </summary>
		/// <param name="conn">A connection provided by the application</param>
		/// <param name="interceptor">A session-scoped interceptor</param>
		/// <returns>A session</returns>
		/// <remarks>
		/// Note that the second-level cache will be disabled if you
		/// supply a ADO.NET connection. NHibernate will not be able to track
		/// any statements you might have executed in the same transaction.
		/// Consider implementing your own <see cref="T:NHibernate.Connection.IConnectionProvider"/>.
		/// </remarks>
		public ISession OpenSession(IDbConnection conn, IInterceptor interceptor)
		{
			throw new NotImplementedException("SessionFactoryDelegate: not implemented");
		}

		/// <summary>
		/// Create a database connection and open a <c>ISession</c> on it
		/// </summary>
		/// <returns></returns>
		public ISession OpenSession()
		{
			ISession realSession = holder.CreateSession( arRootType );

			return new SafeSessionProxy(holder, realSession);
		}

		/// <summary>
		/// Create a new databinder.
		/// </summary>
		/// <returns></returns>
		public IDatabinder OpenDatabinder()
		{
			throw new NotImplementedException("SessionFactoryDelegate: not implemented");
		}

		/// <summary>
		/// Get the <c>ClassMetadata</c> associated with the given entity class
		/// </summary>
		/// <param name="persistentType"></param>
		/// <returns></returns>
		public IClassMetadata GetClassMetadata(Type persistentType)
		{
			throw new NotImplementedException("SessionFactoryDelegate: not implemented");
		}

		#region ISessionFactory Members

		/// <summary>
		/// Get the <see cref="T:NHibernate.Metadata.IClassMetadata"/> associated with the given entity name
		/// </summary>
		/// <param name="entityName">the given entity name.</param>
		/// <returns>
		/// The class metadata or <see langword="null"/> if not found.
		/// </returns>
		/// <seealso cref="T:NHibernate.Metadata.IClassMetadata"/>
		public IClassMetadata GetClassMetadata(string entityName)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Get the <c>CollectionMetadata</c> associated with the named collection role
		/// </summary>
		/// <param name="roleName"></param>
		/// <returns></returns>
		public ICollectionMetadata GetCollectionMetadata(string roleName)
		{
			throw new NotImplementedException("SessionFactoryDelegate: not implemented");
		}

		IDictionary<string, IClassMetadata> ISessionFactory.GetAllClassMetadata()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Get all <c>ClassMetadata</c> as a <c>IDictionary</c> from <c>Type</c>
		/// to metadata object
		/// </summary>
		/// <returns></returns>
		public IDictionary GetAllClassMetadata()
		{
			throw new NotImplementedException("SessionFactoryDelegate: not implemented");
		}

		/// <summary>
		/// Get all <c>CollectionMetadata</c> as a <c>IDictionary</c> from role name
		/// to metadata object
		/// </summary>
		/// <returns></returns>
		public IDictionary GetAllCollectionMetadata()
		{
			throw new NotImplementedException("SessionFactoryDelegate: not implemented");
		}

		/// <summary>
		/// Obtains the current session.
		/// </summary>
		/// <returns>The current session.</returns>
		/// <remarks>Needed for NHibernate 1.2 from trunk</remarks>
		public ISession GetCurrentSession()
		{
			throw new NotImplementedException("SessionFactoryDelegate: not implemented");
		}

		/// <summary>
		/// Destroy this <c>SessionFactory</c> and release all resources
		/// connection pools, etc). It is the responsibility of the application
		/// to ensure that there are no open <c>Session</c>s before calling
		/// <c>close()</c>.
		/// </summary>
		public void Close()
		{
			throw new NotImplementedException("SessionFactoryDelegate: not implemented");
		}

		/// <summary>
		/// Evict all entries from the process-level cache.  This method occurs outside
		/// of any transaction; it performs an immediate "hard" remove, so does not respect
		/// any transaction isolation semantics of the usage strategy.  Use with care.
		/// </summary>
		/// <param name="persistentClass"></param>
		public void Evict(Type persistentClass)
		{
			throw new NotImplementedException("SessionFactoryDelegate: not implemented");
		}

		/// <summary>
		/// Evict an entry from the process-level cache.  This method occurs outside
		/// of any transaction; it performs an immediate "hard" remove, so does not respect
		/// any transaction isolation semantics of the usage strategy.  Use with care.
		/// </summary>
		/// <param name="persistentClass"></param>
		/// <param name="id"></param>
		public void Evict(Type persistentClass, object id)
		{
			throw new NotImplementedException("SessionFactoryDelegate: not implemented");
		}

		/// <summary>
		/// Evict all entries from the process-level cache.  This method occurs outside
		/// of any transaction; it performs an immediate "hard" remove, so does not respect
		/// any transaction isolation semantics of the usage strategy.  Use with care.
		/// </summary>
		/// <param name="roleName"></param>
		public void EvictCollection(string roleName)
		{
			throw new NotImplementedException("SessionFactoryDelegate: not implemented");
		}

		/// <summary>
		/// Evict an entry from the process-level cache.  This method occurs outside
		/// of any transaction; it performs an immediate "hard" remove, so does not respect
		/// any transaction isolation semantics of the usage strategy.  Use with care.
		/// </summary>
		/// <param name="roleName"></param>
		/// <param name="id"></param>
		public void EvictCollection(string roleName, object id)
		{
			throw new NotImplementedException("SessionFactoryDelegate: not implemented");
		}

		/// <summary>
		/// Evict any query result sets cached in the default query cache region.
		/// </summary>
		public void EvictQueries()
		{
			throw new NotImplementedException("SessionFactoryDelegate: not implemented");
		}

		/// <summary>
		/// Evict any query result sets cached in the named query cache region.
		/// </summary>
		/// <param name="cacheRegion"></param>
		public void EvictQueries(string cacheRegion)
		{
			throw new NotImplementedException("SessionFactoryDelegate: not implemented");
		}

		/// <summary>
		/// Obtain the definition of a filter by name.
		/// </summary>
		/// <param name="filterName">The name of the filter for which to obtain the definition.</param>
		/// <returns></returns>
		/// <return>The filter definition.</return>
		public FilterDefinition GetFilterDefinition(string filterName)
		{
			throw new NotImplementedException("SessionFactoryDelegate: not implemented");
		}

		/// <summary>
		/// Get the <see cref="T:NHibernate.Connection.IConnectionProvider"/> used.
		/// </summary>
		/// <value></value>
		public IConnectionProvider ConnectionProvider
		{
			get { throw new NotImplementedException("SessionFactoryDelegate: not implemented"); }
		}

		/// <summary>
		/// Get the SQL <c>Dialect</c>
		/// </summary>
		/// <value></value>
		public Dialect Dialect
		{
			get { throw new NotImplementedException("SessionFactoryDelegate: not implemented"); }
		}

		/// <summary>
		/// Evict all entries from the second-level cache. This method occurs outside
		/// of any transaction; it performs an immediate "hard" remove, so does not respect
		/// any transaction isolation semantics of the usage strategy. Use with care.
		/// </summary>
		/// <param name="entityName"></param>
		public void EvictEntity(string entityName)
		{
			throw new NotImplementedException("SessionFactoryDelegate: not implemented");
		}

		/// <summary>
		/// Evict an entry from the second-level  cache. This method occurs outside
		/// of any transaction; it performs an immediate "hard" remove, so does not respect
		/// any transaction isolation semantics of the usage strategy. Use with care.
		/// </summary>
		/// <param name="entityName"></param>
		/// <param name="id"></param>
		public void EvictEntity(string entityName, object id)
		{
			throw new NotImplementedException("SessionFactoryDelegate: not implemented");
		}

		/// <summary>
		/// Get a new stateless session.
		/// </summary>
		/// <returns></returns>
		public IStatelessSession OpenStatelessSession()
		{
			throw new NotImplementedException("SessionFactoryDelegate: not implemented");
		}

		/// <summary>
		/// Get a new stateless session for the given ADO.NET connection.
		/// </summary>
		/// <param name="connection"></param>
		/// <returns></returns>
		public IStatelessSession OpenStatelessSession(IDbConnection connection)
		{
			throw new NotImplementedException("SessionFactoryDelegate: not implemented");
		}

		/// <summary>
		/// Obtain a set of the names of all filters defined on this SessionFactory.
		/// </summary>
		/// <value></value>
		/// <return>The set of filter names.</return>
		public ICollection<string> DefinedFilterNames
		{
			get { throw new NotImplementedException("SessionFactoryDelegate: not implemented"); }
		}

		/// <summary>
		/// Gets the settings.
		/// </summary>
		/// <value>The settings.</value>
		public Settings Settings
		{
			get { throw new NotImplementedException("SessionFactoryDelegate: not implemented"); }
		}

		/// <summary>
		/// This collections allows external libraries
		/// to add their own configuration to the NHibernate session factory.
		/// This is needed in such cases where the library is tightly coupled to NHibernate, such
		/// as the case of NHibernate Search
		/// </summary>
		/// <value></value>
		public IDictionary Items
		{
			get { throw new NotImplementedException("SessionFactoryDelegate: not implemented"); }
		}

		/// <summary>
		/// Get the statistics for this session factory
		/// </summary>
		/// <value></value>
		public IStatistics Statistics
		{
			get { throw new NotImplementedException("SessionFactoryDelegate: not implemented"); }
		}

		/// <summary>
		/// Was this <see cref="T:NHibernate.ISessionFactory"/> already closed?
		/// </summary>
		/// <value></value>
		public bool IsClosed
		{
			get { throw new NotImplementedException("SessionFactoryDelegate: not implemented"); }
		}

		#endregion

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
		}

		IDictionary<string, ICollectionMetadata> ISessionFactory.GetAllCollectionMetadata()
		{
			throw new NotImplementedException("SessionFactoryDelegate: not implemented");
		}
	}
}
