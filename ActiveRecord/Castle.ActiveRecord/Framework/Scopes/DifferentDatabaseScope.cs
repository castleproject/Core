namespace Castle.ActiveRecord.Framework.Scopes
{
	using System;
	using System.Collections;
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

		public DifferentDatabaseScope(IDbConnection connection) : this(connection, FlushAction.Auto)
		{
		}
		
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

					parentTransactionScope.OnTransactionCompleted += new EventHandler(OnTransactionCompleted);
				}
				else
				{
					// Not supported?
				}
			}
		}

		public override bool WantsToCreateTheSession
		{
			get { return true; }
		}

		public override void RegisterSession(object key, ISession session)
		{
			if (parentTransactionScope != null)
			{
				// parentTransactionalScope.EnsureHasTransaction(session);
				parentTransactionScope.RegisterSession(new KeyHolder(key, connection.ConnectionString), session);
			}
			else if (parentSimpleScope != null)
			{
				parentSimpleScope.RegisterSession(new KeyHolder(key, connection.ConnectionString), session);
			}

			base.RegisterSession(key, session);
		}

		public override bool IsKeyKnown(object key)
		{
			if (parentTransactionScope != null)
			{
				return parentTransactionScope.IsKeyKnown(new KeyHolder(key, connection.ConnectionString));
			}
			
			bool keyKnown = false;

			if (parentSimpleScope != null)
			{
				keyKnown = parentSimpleScope.IsKeyKnown(new KeyHolder(key, connection.ConnectionString));
			}

			return keyKnown ? true : base.IsKeyKnown(key);
		}

		public override ISession GetSession(object key)
		{
			if (parentTransactionScope != null)
			{
				return parentTransactionScope.GetSession(new KeyHolder(key, connection.ConnectionString));
			}

			ISession session = null;

			if (parentSimpleScope != null)
			{
				session = parentSimpleScope.GetSession(new KeyHolder(key, connection.ConnectionString));
			}

			session = session != null ? session : base.GetSession(key);

			return session;
		}

		/// <summary>
		/// </summary>
		/// <param name="sessions"></param>
		protected override void PerformDisposal(ICollection sessions)
		{
			if (parentTransactionScope == null && parentSimpleScope == null)
			{
				PerformDisposal(sessions, true, true);
			}
		}

		public override ISession OpenSession(ISessionFactory sessionFactory, IInterceptor interceptor)
		{
			return sessionFactory.OpenSession(connection, interceptor);
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

		public KeyHolder(object inner, String connectionString)
		{
			this.inner = inner;
			this.connectionString = connectionString;
		}

		public override bool Equals(object obj)
		{
			KeyHolder other = obj as KeyHolder;

			if (other != null)
			{
				return Object.ReferenceEquals(inner, other.inner) && 
				       connectionString == other.connectionString;
			}

			return false;
		}

		public override int GetHashCode()
		{
			return inner.GetHashCode() ^ connectionString.GetHashCode();
		}
	}
}
