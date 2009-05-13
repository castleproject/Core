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

namespace Castle.ActiveRecord.Framework.Scopes
{
	using System;
	using System.Collections.Generic;
	using System.Data;

	using NHibernate;

	/// <summary>
	/// Still very experimental and it's not bullet proof
	/// for all situations
	/// </summary>
	public class DifferentDatabaseScope : AbstractScope
	{
		private readonly IDbConnection connection;
		private readonly SessionScope parentSimpleScope;
		private readonly TransactionScope parentTransactionScope;

		/// <summary>
		/// Initializes a new instance of the <see cref="DifferentDatabaseScope"/> class.
		/// </summary>
		/// <param name="connection">The connection.</param>
		public DifferentDatabaseScope(IDbConnection connection) : this(connection, FlushAction.Auto)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DifferentDatabaseScope"/> class.
		/// </summary>
		/// <param name="connection">The connection.</param>
		/// <param name="flushAction">The flush action.</param>
		public DifferentDatabaseScope(IDbConnection connection, FlushAction flushAction) : base(flushAction, SessionScopeType.Custom)
		{
			if (connection == null) throw new ArgumentNullException("connection");

			this.connection = connection;
			
			ISessionScope parentScope = ScopeUtil.FindPreviousScope(this, true); 

			if (parentScope != null)
			{
				if (parentScope.ScopeType == SessionScopeType.Simple)
				{
					parentSimpleScope = (SessionScope) parentScope;
				}
				else if (parentScope.ScopeType == SessionScopeType.Transactional)
				{
					parentTransactionScope = (TransactionScope) parentScope;

					parentTransactionScope.OnTransactionCompleted += OnTransactionCompleted;
				}
				else
				{
					// Not supported?
				}
			}
		}

		/// <summary>
		/// We want to be in charge of creating the session
		/// </summary>
		/// <value></value>
		public override bool WantsToCreateTheSession
		{
			get { return true; }
		}

		/// <summary>
		/// This method is invoked when no session was available
		/// at and the <see cref="Castle.ActiveRecord.Framework.ISessionFactoryHolder"/>
		/// just created one. So it registers the session created
		/// within this scope using a key. The scope implementation
		/// shouldn't make any assumption on what the key
		/// actually is as we reserve the right to change it
		/// <seealso cref="IsKeyKnown"/>
		/// </summary>
		/// <param name="key">an object instance</param>
		/// <param name="session">An instance of <c>ISession</c></param>
		public override void RegisterSession(object key, ISession session)
		{
			if (parentTransactionScope != null)
			{
				// parentTransactionalScope.EnsureHasTransaction(session);
				parentTransactionScope.RegisterSession(new KeyHolder(key, connection.ConnectionString, connection.GetHashCode()), session);
			}
			else if (parentSimpleScope != null)
			{
				parentSimpleScope.RegisterSession(new KeyHolder(key, connection.ConnectionString, connection.GetHashCode()), session);
			}

			base.RegisterSession(key, session);
		}

		/// <summary>
		/// This method is invoked when the
		/// <see cref="Castle.ActiveRecord.Framework.ISessionFactoryHolder"/>
		/// instance needs a session instance. Instead of creating one it interrogates
		/// the active scope for one. The scope implementation must check if it
		/// has a session registered for the given key.
		/// <seealso cref="RegisterSession"/>
		/// </summary>
		/// <param name="key">an object instance</param>
		/// <returns>
		/// 	<c>true</c> if the key exists within this scope instance
		/// </returns>
		public override bool IsKeyKnown(object key)
		{
			if (parentTransactionScope != null)
			{
				return parentTransactionScope.IsKeyKnown(new KeyHolder(key, connection.ConnectionString, connection.GetHashCode()));
			}
			
			bool keyKnown = false;

			if (parentSimpleScope != null)
			{
				keyKnown = parentSimpleScope.IsKeyKnown(new KeyHolder(key, connection.ConnectionString, connection.GetHashCode()));
			}

			return keyKnown ? true : base.IsKeyKnown(key);
		}

		/// <summary>
		/// This method should return the session instance associated with the key.
		/// </summary>
		/// <param name="key">an object instance</param>
		/// <returns>
		/// the session instance or null if none was found
		/// </returns>
		public override ISession GetSession(object key)
		{
			if (parentTransactionScope != null)
			{
				return parentTransactionScope.GetSession(new KeyHolder(key, connection.ConnectionString, connection.GetHashCode()));
			}

			ISession session = null;

			if (parentSimpleScope != null)
			{
				session = parentSimpleScope.GetSession(new KeyHolder(key, connection.ConnectionString, connection.GetHashCode()));
			}

			session = session ?? base.GetSession(key);

			return session;
		}

		/// <summary>
		/// Performs the disposal.
		/// </summary>
		/// <param name="sessions">The sessions.</param>
		protected override void PerformDisposal(ICollection<ISession> sessions)
		{
			if (parentTransactionScope == null && parentSimpleScope == null)
			{
				PerformDisposal(sessions, true, true);
			}
		}

		/// <summary>
		/// If the <see cref="WantsToCreateTheSession"/> returned
		/// <c>true</c> then this method is invoked to allow
		/// the scope to create a properly configured session
		/// </summary>
		/// <param name="sessionFactory">From where to open the session</param>
		/// <param name="interceptor">the NHibernate interceptor</param>
		/// <returns>the newly created session</returns>
		public override ISession OpenSession(ISessionFactory sessionFactory, IInterceptor interceptor)
		{
			return sessionFactory.OpenSession(connection, interceptor);
		}

		/// <summary>
		/// This is called when a session has a failure
		/// </summary>
		/// <param name="session">the session</param>
		public override void FailSession(ISession session)
		{
			if (parentTransactionScope != null)
			{
				parentTransactionScope.VoteRollBack();
			}
			else
			{
				session.Clear();
			}
		}

		private void OnTransactionCompleted(object sender, EventArgs e)
		{
			TransactionScope scope = (sender as TransactionScope);
			scope.DiscardSessions( GetSessions() );
		}
	}

	class KeyHolder
	{
		private readonly object inner;
		private readonly String connectionString;
		private readonly int connectionHashCode;

		public KeyHolder(object inner, String connectionString, int connectionHashCode)
		{
			this.inner = inner;
			this.connectionHashCode = connectionHashCode;
			this.connectionString = connectionString;
		}

		public override bool Equals(object obj)
		{
			KeyHolder other = obj as KeyHolder;

			if (other != null)
			{
				return ReferenceEquals(inner, other.inner) && 
				       connectionString == other.connectionString &&
					   connectionHashCode == other.connectionHashCode;
			}

			return false;
		}

		public override int GetHashCode()
		{
			return inner.GetHashCode() ^ connectionString.GetHashCode() ^ connectionHashCode;
		}
	}
}
